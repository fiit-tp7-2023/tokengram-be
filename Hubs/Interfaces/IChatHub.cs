using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Hubs.Interfaces
{
    public interface IChatHub
    {
        Task ReceivedChatInvitation(ReceivedChatInvitationResponseDTO chat);

        Task UserJoinedChat(long chatId, UserResponseDTO joiningUser);

        Task UserLeftChat(long chatId, UserResponseDTO leavingUser);

        Task UserDeclinedChatInvitation(long chatId, UserResponseDTO decliningUser);

        Task ReceivedMessage(long chatId, ChatMessageResponseDTO message);

        Task DeletedMessage(long chatId, long chatMessageId);

        Task AdminDeletedChat(long chatId);

        Task AdminInvitedUser(long chatId, UserResponseDTO invitedUser);

        Task NewAdmin(long chatId, UserResponseDTO newAdmin);

        Task ChatProfileDeviceSync(UserChatProfileResponseDTO chatProfile);
    }
}
