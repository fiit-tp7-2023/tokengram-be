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

        [HttpGet("{chatId}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessageResponseDTO>>> GetChatMessages(
            long chatId,
            [FromQuery] PaginationRequestDTO request
        )
        {
            var result = await _chatService.GetChatMessages(GetUserAddress(), chatId, request);

            return Ok(_mapper.Map<IEnumerable<ChatMessageResponseDTO>>(result));
        }
    }
}
