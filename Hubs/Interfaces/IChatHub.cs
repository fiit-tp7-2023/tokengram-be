using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Hubs.Interfaces
{
    public interface IChatHub
    {
        Task ReceivedChatInvitation(ReceivedChatInvitationResponseDTO chat);

        Task UserJoinedChat(long chatId, BasicUserResponseDTO joiningUser);

        Task UserLeftChat(long chatId, string leavingUserAddress);

        Task UserDeclinedChatInvitation(long chatId, string decliningUserAddress);

        Task ReceivedMessage(long chatId, ChatMessageResponseDTO message);

        Task DeletedMessage(long chatId, long chatMessageId);

        Task AdminDeletedChat(long chatId);

        Task AdminInvitedUser(long chatId, ChatInvitationResponseDTO chatInvitation);

        Task NewAdmin(long chatId, BasicUserResponseDTO newAdmin);

        Task CreatedChatFromAnotherDevice(ChatResponseDTO chat);

        Task JoinedChatFromAnotherDevice(ChatResponseDTO chat);

        Task DeclinedChatInvitationFromAnotherDevice(long chatId);

        Task LeftChatFromAnotherDevice(long chatId);

        Task AdminDeletedChatInvitation(long chatId);
    }
}
