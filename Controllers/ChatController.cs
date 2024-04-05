using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Infrastructure.ActionFilters;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Services.Interfaces;

namespace Tokengram.Controllers
{
    [Authorize]
    [ApiController]
    [Route("chats")]
    public class ChatController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IChatService _chatService;

        public ChatController(IMapper mapper, IChatService chatService)
        {
            _mapper = mapper;
            _chatService = chatService;
        }

        [HttpGet("")]
        public async Task<ActionResult<UserChatProfileResponseDTO>> GetUserChatProfile()
        {
            var result = await _chatService.GetUserChatProfile(GetUserAddress());

            return Ok(_mapper.Map<UserChatProfileResponseDTO>(result));
        }

        [HttpGet("{chatId}/messages")]
        [BindChat]
        public async Task<ActionResult<IEnumerable<ChatMessageResponseDTO>>> GetChatMessages(
            long chatId,
            [FromQuery] PaginationRequestDTO request
        )
        {
            Chat chat = (HttpContext.Items["chat"] as Chat)!;

            var result = await _chatService.GetChatMessages(GetUserAddress(), chat, request);

            return Ok(_mapper.Map<IEnumerable<ChatMessageResponseDTO>>(result));
        }
    }
}
