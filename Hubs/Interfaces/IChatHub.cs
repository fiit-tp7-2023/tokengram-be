using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Hubs.Interfaces
{
    public interface IChatHub
    {
        Task ReceivedChatInvitation(ReceivedChatInvitationResponseDTO chat);

        Task UserJoinedChat(long chatId, BasicUserResponseDTO joiningUser);

        Task UserLeftChat(long chatId, BasicUserResponseDTO leavingUser);

        Task UserDeclinedChatInvitation(long chatId, BasicUserResponseDTO decliningUser);

        Task ReceivedMessage(long chatId, ChatMessageResponseDTO message);

        Task DeletedMessage(long chatId, long chatMessageId);

        Task AdminDeletedChat(long chatId);

        Task AdminInvitedUser(long chatId, BasicUserResponseDTO invitedUser);

        Task NewAdmin(long chatId, BasicUserResponseDTO newAdmin);

        Task ChatProfileDeviceSync(UserChatProfileResponseDTO chatProfile);
    }
}
