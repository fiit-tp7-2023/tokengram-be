using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.WS.Requests;
using Tokengram.Hubs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Models.Hubs;
using Tokengram.Infrastructure.ActionFilterAttributes;
using Tokengram.Enums;

namespace Tokengram.Hubs
{
    [Authorize]
    public partial class ChatHub : BaseHub<IChatHub>
    {
        public async Task<ChatResponseDTO> CreateChat(ChatRequestDTO request)
        {
            User sender = await GetUser();

            if (!request.UserAddresses.Contains(sender.Address))
                request.UserAddresses.Add(sender.Address);

            Chat chat =
                new()
                {
                    Name = request.Name,
                    AdminAddress = sender.Address,
                    Type = request.UserAddresses.Count > 2 ? ChatTypeEnum.GROUP : ChatTypeEnum.PRIVATE
                };
            chat.Users.Add(sender);

            IEnumerable<User> users = await _dbContext.Users
                .Where(x => request.UserAddresses.Contains(x.Address))
                .ToListAsync();

            foreach (User user in users)
            {
                chat.ChatInvitations.Add(
                    new ChatInvitation
                    {
                        User = user,
                        Sender = user.Address == sender.Address ? null : sender,
                        JoinedAt = user.Address == sender.Address ? DateTime.UtcNow : null
                    }
                );
            }

            await _dbContext.Chats.AddAsync(chat);
            await _dbContext.SaveChangesAsync();

            ReceivedChatInvitationResponseDTO chatInvitationResponse =
                new()
                {
                    Chat = _mapper.Map<BasicChatResponseDTO>(chat),
                    Sender = _mapper.Map<BasicUserResponseDTO>(sender)
                };

            await Clients
                .AllExcept(
                    _connectedUsers
                        .Where(
                            x => !request.UserAddresses.Where(address => address != sender.Address).Contains(x.Address)
                        )
                        .Select(x => x.ConnectionId)
                )
                .ReceivedChatInvitation(chatInvitationResponse);

            await Clients
                .AllExcept(
                    _connectedUsers
                        .Where(x => x.Address != sender.Address || x.ConnectionId == Context.ConnectionId)
                        .Select(x => x.ConnectionId)
                )
                .CreatedChatFromAnotherDevice(_mapper.Map<ChatResponseDTO>(chat));
            await AddUserConnectionsToChatGroup(chat);

            return _mapper.Map<ChatResponseDTO>(chat);
        }

        [BindChatHub]
        [BindUserHub(UserMethodKey = "invitedUserAddress", ItemKey = "invitedUser")]
        public async Task<ChatInvitationResponseDTO> InviteToChat(long chatId, long invitedUserAddress)
        {
            Chat chat = (Context.Items["chat"] as Chat)!;
            User invitedUser = (Context.Items["invitedUser"] as User)!;

            chat = await _dbContext.Chats.Include(x => x.Users).FirstAsync(x => x.Id == chat.Id);
            User sender = await GetUser();
            bool isChatAdmin = chat.AdminAddress == sender.Address;
            bool invitationExists = chat.ChatInvitations.Any(x => x.UserAddress == invitedUser.Address);

            if (chat.Type == ChatTypeEnum.PRIVATE)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_TO_PRIVATE_CHAT);
            if (sender.Address == invitedUser.Address)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_TO_SELF);
            if (!isChatAdmin)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_NOT_ADMIN);
            if (invitationExists)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_EXISTS);

            ChatInvitation chatInvitation =
                new()
                {
                    Sender = sender,
                    User = invitedUser,
                    Chat = chat
                };
            await _dbContext.ChatInvitations.AddAsync(chatInvitation);
            await _dbContext.SaveChangesAsync();

            await Clients
                .AllExcept(_connectedUsers.Where(x => x.Address != invitedUser.Address).Select(x => x.ConnectionId))
                .ReceivedChatInvitation(_mapper.Map<ReceivedChatInvitationResponseDTO>(chatInvitation));
            await Clients
                .OthersInGroup(chat.Id.ToString())
                .AdminInvitedUser(chat.Id, _mapper.Map<ChatInvitationResponseDTO>(chatInvitation));

            return _mapper.Map<ChatInvitationResponseDTO>(chatInvitation);
        }

        [BindChatHub]
        public async Task<ChatResponseDTO?> RespondToChatInvitation(
            long chatId,
            ChatInvitationResponseRequestDTO request
        )
        {
            Chat chat = (Context.Items["chat"] as Chat)!;

            User respondingUser = await GetUser();
            chat = await _dbContext.Chats
                .Include(x => x.Users)
                .Include(x => x.ChatMessages.OrderByDescending(y => y.CreatedAt).Take(1))
                .ThenInclude(x => x.Sender)
                .FirstAsync(x => x.Id == chat.Id);
            ChatInvitation? chatInvitation = chat.ChatInvitations.FirstOrDefault(
                x => x.UserAddress == respondingUser.Address
            );

            if (chatInvitation == null)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_RESPONSE_NOT_INVITED);
            if (chatInvitation.JoinedAt != null)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_RESPONSE_EXISTS);

            if (request.Accept)
            {
                chatInvitation.JoinedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                await Clients
                    .Group(chat.Id.ToString())
                    .UserJoinedChat(chat.Id, _mapper.Map<BasicUserResponseDTO>(respondingUser));
                await Clients
                    .AllExcept(
                        _connectedUsers
                            .Where(x => x.Address != respondingUser.Address || x.ConnectionId == Context.ConnectionId)
                            .Select(x => x.ConnectionId)
                    )
                    .JoinedChatFromAnotherDevice(_mapper.Map<ChatResponseDTO>(chat));
                await AddUserConnectionsToChatGroup(chat);

                return _mapper.Map<ChatResponseDTO>(chat);
            }
            else
            {
                _dbContext.ChatInvitations.Remove(chatInvitation);
                await _dbContext.SaveChangesAsync();
                await Clients.Group(chat.Id.ToString()).UserDeclinedChatInvitation(chat.Id, respondingUser.Address);
                await Clients
                    .AllExcept(
                        _connectedUsers
                            .Where(x => x.Address != respondingUser.Address || x.ConnectionId == Context.ConnectionId)
                            .Select(x => x.ConnectionId)
                    )
                    .DeclinedChatInvitationFromAnotherDevice(chat.Id);

                return null;
            }
        }

        [BindChatHub]
        [BindUserHub(UserMethodKey = "adminAddress", ItemKey = "admin")]
        public async Task PromoteToAdmin(long chatId, long adminAddress)
        {
            Chat chat = (Context.Items["chat"] as Chat)!;
            User admin = (Context.Items["invitedUser"] as User)!;

            chat = await _dbContext.Chats
                .Include(x => x.Users)
                .Include(x => x.ChatMessages.OrderByDescending(y => y.CreatedAt).Take(1))
                .ThenInclude(x => x.Sender)
                .FirstAsync(x => x.Id == chat.Id);
            User promotingUser = await GetUser();
            bool isAdminMember = await _dbContext.ChatInvitations.AnyAsync(
                x => x.UserAddress == promotingUser.Address && x.JoinedAt != null
            );
            bool isPromoterAdmin = chat.AdminAddress == promotingUser.Address;

            if (chat.Type == ChatTypeEnum.PRIVATE)
                throw new HubException(Constants.ErrorMessages.CHAT_PROMOTE_ADMIN_PRIVATE_CHAT);
            if (isAdminMember)
                throw new HubException(Constants.ErrorMessages.CHAT_PROMOTE_ADMIN_NOT_MEMBER);
            if (!isAdminMember)
                throw new HubException(Constants.ErrorMessages.CHAT_PROMOTE_ADMIN_NOT_ADMIN);

            chat.AdminAddress = admin.Address;
            await _dbContext.SaveChangesAsync();

            await Clients.OthersInGroup(chat.Id.ToString()).NewAdmin(chat.Id, _mapper.Map<BasicUserResponseDTO>(admin));
        }

        [BindChatHub]
        public async Task LeaveChat(long chatId)
        {
            Chat chat = (Context.Items["chat"] as Chat)!;

            User leavingUser = await GetUser();
            List<ChatInvitation> chatInvitations = await _dbContext.ChatInvitations
                .Include(x => x.User)
                .Where(x => x.ChatId == chat.Id)
                .ToListAsync();
            List<ChatInvitation> pendingChatInvitations = chatInvitations.Where(x => x.JoinedAt == null).ToList();
            List<ChatInvitation> acceptedChatInvitations = chatInvitations.Where(x => x.JoinedAt != null).ToList();
            ChatInvitation? leavingUserChatInvitation = acceptedChatInvitations.FirstOrDefault(
                x => x.UserAddress == leavingUser.Address
            );
            bool isAdmin = chat.AdminAddress == leavingUser.Address;
            bool chatDeleted = false;
            bool newAdminPromoted = false;

            if (leavingUserChatInvitation == null)
                throw new HubException(Constants.ErrorMessages.CHAT_NOT_MEMBER);

            if (isAdmin)
            {
                if (chat.Type == ChatTypeEnum.PRIVATE && acceptedChatInvitations.Count > 1)
                {
                    chat.Admin = acceptedChatInvitations.First(x => x.UserAddress != leavingUser.Address).User;
                    newAdminPromoted = true;
                }
                else
                {
                    _dbContext.Chats.Remove(chat);
                    chatDeleted = true;
                }
            }

            _dbContext.ChatInvitations.Remove(leavingUserChatInvitation);
            await _dbContext.SaveChangesAsync();

            await Clients
                .AllExcept(
                    _connectedUsers
                        .Where(x => x.Address != leavingUser.Address || x.ConnectionId == Context.ConnectionId)
                        .Select(x => x.ConnectionId)
                )
                .LeftChatFromAnotherDevice(chat.Id);

            await RemoveUserConnectionsFromChatGroup(chat);

            if (chatDeleted)
            {
                await Clients.Group(chat.Id.ToString()).AdminDeletedChat(chat.Id);
                ChatGroup? chatGroup = _chatGroups.FirstOrDefault(x => x.ChatId == chat.Id);
                if (chatGroup != null)
                {
                    foreach (ConnectedUser connection in chatGroup.ConnectedUsers)
                        await RemoveUserConnectionFromChatGroup(chat, connection);
                }
                await Clients
                    .AllExcept(
                        _connectedUsers
                            .Where(x => !pendingChatInvitations.Select(x => x.User.Address).Contains(x.Address))
                            .Select(x => x.ConnectionId)
                    )
                    .AdminDeletedChatInvitation(chat.Id);
            }
            else
            {
                if (newAdminPromoted)
                {
                    await Clients
                        .Group(chat.Id.ToString())
                        .NewAdmin(chat.Id, _mapper.Map<BasicUserResponseDTO>(chat.Admin));
                }
                await Clients.Group(chat.Id.ToString()).UserLeftChat(chat.Id, leavingUser.Address);
            }
        }
    }
}
