using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Extensions;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.Exceptions;
using Tokengram.Services.Interfaces;

namespace Tokengram.Services
{
    public class ChatService : IChatService
    {
        private readonly TokengramDbContext _dbContext;

        public ChatService(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserChatProfile(string userAddress)
        {
            return await _dbContext.Users
                .Include(x => x.Chats)
                .ThenInclude(x => x.Users)
                .Include(x => x.Chats)
                .ThenInclude(x => x.ChatMessages.OrderByDescending(y => y.CreatedAt).Take(1))
                .ThenInclude(x => x.Sender)
                .FirstAsync(x => x.Address == userAddress);
        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessages(
            string userAddress,
            long chatId,
            PaginationRequestDTO request
        )
        {
            Chat? chat = await _dbContext.Chats
                .Include(x => x.ChatInvitations)
                .FirstOrDefaultAsync(x => x.Id == chatId);

            if (chat == null)
                throw new NotFoundException(Constants.ErrorMessages.CHAT_NOT_FOUND);

            bool isChatMember = chat.ChatInvitations.Any(x => x.UserAddress == userAddress && x.JoinedAt != null);

            if (!isChatMember)
                throw new ForbiddenException(Constants.ErrorMessages.CHAT_NOT_MEMBER);

            List<ChatMessage> chatMessages = await _dbContext.ChatMessages
                .Include(x => x.Sender)
                .Where(x => x.ChatId == chat.Id)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(request.PageNumber, request.PageSize)
                .ToListAsync();

            return chatMessages;
        }
    }
}
