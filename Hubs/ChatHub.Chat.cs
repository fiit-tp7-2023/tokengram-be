using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Neo4jClient.Extensions;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.WS.Requests;
using Tokengram.Hubs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Models.Hubs;

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
                    Type = request.UserAddresses.Count > 2 ? Enums.ChatTypeEnum.GROUP : Enums.ChatTypeEnum.PRIVATE
                };
            chat.Users.Add(sender);

            foreach (string userAddress in request.UserAddresses)
            {
                chat.ChatInvitations.Add(
                    new ChatInvitation
                    {
                        UserAddress = userAddress,
                        Sender = userAddress == sender.Address ? null : sender,
                        JoinedAt = userAddress == sender.Address ? DateTime.UtcNow : null
                    }
                );
            }

            await _dbContext.Chats.AddAsync(chat);
            await _dbContext.SaveChangesAsync();

            ReceivedChatInvitationResponseDTO chatInvitationResponse =
                new() { Chat = _mapper.Map<BasicChatResponseDTO>(chat), Sender = _mapper.Map<UserResponseDTO>(sender) };

            await Clients
                .AllExcept(
                    _connectedUsers
                        .Where(x => x.Address.NotIn(request.UserAddresses.Where(address => address != sender.Address)))
                        .Select(x => x.ConnectionId)
                )
                .ReceivedChatInvitation(chatInvitationResponse);

            await SendChatProfileDeviceSync();
            await AddUserConnectionsToChatGroup(chat);

            return _mapper.Map<ChatResponseDTO>(chat);
        }

        public async Task InviteToChat(ChatInvitationRequestDTO request)
        {
            User sender = await GetUser();
            User invitedUser = await _dbContext.Users.FirstAsync(x => x.Address == request.UserAddress);
            Chat chat = await _dbContext.Chats.Include(x => x.Users).FirstAsync(x => x.Id == request.ChatId);
            bool isChatAdmin = chat.AdminAddress == sender.Address;
            bool invitationExists = chat.ChatInvitations.Any(x => x.UserAddress == request.UserAddress);

            if (chat.Type == Enums.ChatTypeEnum.PRIVATE)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_TO_PRIVATE_CHAT);
            if (sender.Address == request.UserAddress)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_TO_SELF);
            if (!isChatAdmin)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_NOT_ADMIN);
            if (invitationExists)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_EXISTS);

            ChatInvitation chatInvitation =
                new()
                {
                    Sender = sender,
                    UserAddress = request.UserAddress,
                    Chat = chat
                };
            await _dbContext.ChatInvitations.AddAsync(chatInvitation);
            await _dbContext.SaveChangesAsync();

            await Clients
                .AllExcept(_connectedUsers.Where(x => x.Address != request.UserAddress).Select(x => x.ConnectionId))
                .ReceivedChatInvitation(_mapper.Map<ReceivedChatInvitationResponseDTO>(chatInvitation));
            await Clients
                .OthersInGroup(chat.Id.ToString())
                .AdminInvitedUser(chat.Id, _mapper.Map<UserResponseDTO>(invitedUser));
        }

        public async Task<ChatResponseDTO?> RespondToChatInvitation(ChatInvitationResponseRequestDTO request)
        {
            User respondingUser = await GetUser();
            Chat chat = await _dbContext.Chats
                .Include(x => x.Users)
                .Include(x => x.ChatMessages.OrderByDescending(y => y.CreatedAt).Take(1))
                .ThenInclude(x => x.Sender)
                .FirstAsync(x => x.Id == request.ChatId);
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
                    .UserJoinedChat(chat.Id, _mapper.Map<UserResponseDTO>(respondingUser));
                await SendChatProfileDeviceSync();
                await AddUserConnectionsToChatGroup(chat);

                return _mapper.Map<ChatResponseDTO>(chat);
            }
            else
            {
                _dbContext.ChatInvitations.Remove(chatInvitation);
                await _dbContext.SaveChangesAsync();
                await Clients
                    .Group(chat.Id.ToString())
                    .UserDeclinedChatInvitation(chat.Id, _mapper.Map<UserResponseDTO>(respondingUser));
                await SendChatProfileDeviceSync();

                return null;
            }
        }

        public async Task PromoteToAdmin(ChatPromoteToAdminRequestDTO request)
        {
            User promotingUser = await GetUser();
            User newAdmin = await _dbContext.Users.FirstAsync(x => x.Address == request.AdminAddress);
            Chat chat = await _dbContext.Chats.Include(x => x.ChatInvitations).FirstAsync(x => x.Id == request.ChatId);
            ChatInvitation? chatInvitation = chat.ChatInvitations.FirstOrDefault(
                x => x.UserAddress == promotingUser.Address && x.JoinedAt != null
            );
            bool isAdmin = chat.AdminAddress == promotingUser.Address;

            if (chatInvitation == null)
                throw new HubException(Constants.ErrorMessages.CHAT_NOT_MEMBER);
            if (chat.Type == Enums.ChatTypeEnum.PRIVATE)
                throw new HubException(Constants.ErrorMessages.CHAT_PROMOTE_ADMIN_PRIVATE_CHAT);
            if (!isAdmin)
                throw new HubException(Constants.ErrorMessages.CHAT_PROMOTE_ADMIN_NOT_ADMIN);

            chat.AdminAddress = request.AdminAddress;
            await _dbContext.SaveChangesAsync();

            await Clients.OthersInGroup(chat.Id.ToString()).NewAdmin(chat.Id, _mapper.Map<UserResponseDTO>(newAdmin));
        }

        public async Task LeaveChat(ChatLeaveRequestDTO request)
        {
            User leavingUser = await GetUser();
            Chat chat = await _dbContext.Chats.Include(x => x.ChatInvitations).FirstAsync(x => x.Id == request.ChatId);
            ChatInvitation? chatInvitation = chat.ChatInvitations.FirstOrDefault(
                x => x.UserAddress == leavingUser.Address && x.JoinedAt != null
            );
            bool isAdmin = chat.AdminAddress == leavingUser.Address;

            if (chatInvitation == null)
                throw new HubException(Constants.ErrorMessages.CHAT_NOT_MEMBER);
            if (chat.Type == Enums.ChatTypeEnum.PRIVATE)
                throw new HubException(Constants.ErrorMessages.CHAT_LEAVE_PRIVATE_CHAT);

            if (isAdmin)
                _dbContext.Chats.Remove(chat);

            _dbContext.ChatInvitations.Remove(chatInvitation);
            await _dbContext.SaveChangesAsync();

            await RemoveUserConnectionsFromChatGroup(chat);

            if (isAdmin)
            {
                await Clients.Group(chat.Id.ToString()).AdminDeletedChat(chat.Id);
                ChatGroup chatGroup = _chatGroups.First(x => x.ChatId == chat.Id);
                foreach (ConnectedUser connection in chatGroup.ConnectedUsers)
                {
                    await RemoveUserConnectionFromChatGroup(chat, connection);
                }
            }
            else
                await Clients
                    .Group(chat.Id.ToString())
                    .UserLeftChat(chat.Id, _mapper.Map<UserResponseDTO>(leavingUser));

            await SendChatProfileDeviceSync();
        }
    }
}
