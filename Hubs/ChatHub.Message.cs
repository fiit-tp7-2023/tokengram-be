using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Models.DTOS.WS.Requests;
using Tokengram.Hubs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Infrastructure.ActionFilters;
using Tokengram.Infrastructure.ActionFilterAttributes;

namespace Tokengram.Hubs
{
    [Authorize]
    public partial class ChatHub : BaseHub<IChatHub>
    {
        [BindChatHub]
        public async Task<ChatMessageResponseDTO> SendMessage(long chatId, ChatMessageRequestDTO request)
        {
            Chat chat = (Context.Items["chat"] as Chat)!;
            User sender = await GetUser();
            ChatInvitation? chatInvitation = await _dbContext.ChatInvitations.FirstOrDefaultAsync(
                x => x.UserAddress == sender.Address
            );

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
                    Chat = chat,
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

        [BindChatMessageHub]
        public async Task DeleteMessage(long chatId, long chatMessageId)
        {
            ChatMessage chatMessage = (Context.Items["chat"] as ChatMessage)!;
            string userAddress = GetUserAddress();
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
