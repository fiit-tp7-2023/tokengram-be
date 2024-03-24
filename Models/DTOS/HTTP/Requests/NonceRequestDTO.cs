using Microsoft.AspNetCore.Mvc;

namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public class NonceRequestDTO
    {
        [FromQuery]
        public string Address { get; set; } = null!;
    }
}
