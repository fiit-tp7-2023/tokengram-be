using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tokengram.Database.Postgres.Entities;
using Tokengram.Models.DTOS.WS.Requests;
using Tokengram.Hubs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Hubs
{
    [Authorize]
    public partial class ChatHub : BaseHub<IChatHub>
    {
        public async Task<ChatMessageResponseDTO> SendMessage(ChatMessageRequestDTO request)
        {
            User sender = await GetUser();
            Chat chat = await _dbContext.Chats.Include(x => x.ChatInvitations).FirstAsync(x => x.Id == request.ChatId);
            ChatInvitation? chatInvitation = chat.ChatInvitations.FirstOrDefault(x => x.UserAddress == sender.Address);

            if (chatInvitation == null)
                throw new HubException(Constants.ErrorMessages.CHAT_INVITATION_RESPONSE_NOT_INVITED);

            bool joinedNow = chatInvitation.JoinedAt == null;
            if (joinedNow)
                chatInvitation.JoinedAt = DateTime.UtcNow;

            ChatMessage chatMessage =
                new()
                {
                    Content = request.Content,
                    Sender = sender,
                    ChatId = request.ChatId,
                    ParentMessageId = request.ParentMessageId,
                };
            await _dbContext.ChatMessages.AddAsync(chatMessage);
            await _dbContext.SaveChangesAsync();

            if (joinedNow)
            {
                await Clients.Group(chat.Id.ToString()).UserJoinedChat(chat.Id, _mapper.Map<UserResponseDTO>(sender));
                await SendChatProfileDeviceSync();
                await AddUserConnectionsToChatGroup(chat);
            }

            ChatMessageResponseDTO chatMessageResponse = _mapper.Map<ChatMessageResponseDTO>(chatMessage);

            await Clients.OthersInGroup(chat.Id.ToString()).ReceivedMessage(chat.Id, chatMessageResponse);

            return chatMessageResponse;
        }

        public async Task DeleteMessage(ChatMessageDeleteRequestDTO request)
        {
            string userAddress = GetUserAddress();
            ChatMessage chatMessage = await _dbContext.ChatMessages
                .Include(x => x.MessageReplies)
                .FirstAsync(x => x.Id == request.ChatMessageId);
            bool isSender = chatMessage.SenderAddress == userAddress;

            if (!isSender)
                throw new HubException(Constants.ErrorMessages.CHAT_MESSAGE_NOT_SENDER);

            _dbContext.ChatMessages.Remove(chatMessage);
            await _dbContext.SaveChangesAsync();

            await Clients
                .OthersInGroup(chatMessage.ChatId.ToString())
                .DeletedMessage(chatMessage.ChatId, chatMessage.Id);
        }
    }
}
