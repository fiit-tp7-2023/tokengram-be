using Microsoft.AspNetCore.Authorization;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Hubs.Interfaces;
using Tokengram.Models.Hubs;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Hubs
{
    [Authorize]
    public partial class ChatHub : BaseHub<IChatHub>
    {
        private readonly TokengramDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly List<ConnectedUser> _connectedUsers;
        private readonly List<ChatGroup> _chatGroups;

        public ChatHub(
            TokengramDbContext dbContext,
            IMapper mapper,
            List<ConnectedUser> connectedUsers,
            List<ChatGroup> chatGroups
        )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _connectedUsers = connectedUsers;
            _chatGroups = chatGroups;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            User user = await _dbContext.Users
                .Include(x => x.Chats.Where(x => x.ChatInvitations.Any(x => x.JoinedAt != null)))
                .FirstAsync(x => x.Address == GetUserAddress());
            ConnectedUser connection = new() { Address = user.Address, ConnectionId = Context.ConnectionId };
            _connectedUsers.Add(connection);
            foreach (Chat chat in user.Chats)
            {
                await AddUserConnectionToChatGroup(chat, connection);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            User user = await _dbContext.Users
                .Include(x => x.Chats.Where(x => x.ChatInvitations.Any(x => x.JoinedAt != null)))
                .FirstAsync(x => x.Address == GetUserAddress());
            ConnectedUser connection = new() { Address = user.Address, ConnectionId = Context.ConnectionId };
            _connectedUsers.Add(connection);
            foreach (Chat chat in user.Chats)
            {
                await AddUserConnectionToChatGroup(chat, connection);
            }
            _connectedUsers.RemoveAll(u => u.ConnectionId == Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        private async Task<User> GetUser()
        {
            return await _dbContext.Users.FirstAsync(x => x.Address == GetUserAddress());
        }

        private async Task AddUserConnectionToChatGroup(Chat chat, ConnectedUser connection)
        {
            ChatGroup chatGroup =
                _chatGroups.FirstOrDefault(x => x.ChatId == chat.Id)
                ?? new ChatGroup { ChatId = chat.Id, ConnectedUsers = new List<ConnectedUser>() };

            if (!chatGroup.ConnectedUsers.Any(x => x.ConnectionId == connection.ConnectionId))
                chatGroup.ConnectedUsers.Add(connection);

            await Groups.AddToGroupAsync(connection.ConnectionId, chat.Id.ToString());
        }

        private async Task AddUserConnectionsToChatGroup(Chat chat)
        {
            var userConnections = _connectedUsers.Where(x => x.Address == GetUserAddress());
            foreach (ConnectedUser connection in userConnections)
                await AddUserConnectionToChatGroup(chat, connection);
        }

        private async Task RemoveUserConnectionFromChatGroup(Chat chat, ConnectedUser connection)
        {
            ChatGroup? chatGroup = _chatGroups.FirstOrDefault(x => x.ChatId == chat.Id);

            if (chatGroup != null && chatGroup.ConnectedUsers.Any(x => x.ConnectionId == connection.ConnectionId))
            {
                chatGroup.ConnectedUsers.Remove(connection);
                if (chatGroup.ConnectedUsers.Count == 0)
                    _chatGroups.Remove(chatGroup);
            }

            await Groups.RemoveFromGroupAsync(connection.ConnectionId, chat.Id.ToString());
        }

        private async Task RemoveUserConnectionsFromChatGroup(Chat chat)
        {
            var userConnections = _connectedUsers.Where(x => x.Address == GetUserAddress());
            foreach (ConnectedUser connection in userConnections)
                await RemoveUserConnectionFromChatGroup(chat, connection);
        }
    }
}
