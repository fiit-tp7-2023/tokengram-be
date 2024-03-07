using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Services.Interfaces;

namespace Tokengram.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ChatController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IChatService _chatService;

        public ChatController(IMapper mapper, IChatService chatService)
        {
            _mapper = mapper;
            _chatService = chatService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserChatProfileResponseDTO>> GetChatProfile()
        {
            var result = await _chatService.GetUserChatProfile(GetUserAddress());

            return Ok(_mapper.Map<UserChatProfileResponseDTO>(result));
        }

        [HttpGet("{chatId}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessageResponseDTO>>> GetChatMessages(
            long chatId,
            [FromQuery] ChatMessageSearchRequestDTO request
        )
        {
            var result = await _chatService.GetChatMessages(GetUserAddress(), chatId, request);

            return Ok(_mapper.Map<ChatMessageResponseDTO>(result));
        }
    }
}
