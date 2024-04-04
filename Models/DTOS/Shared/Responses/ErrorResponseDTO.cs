using System.Net;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class ErrorResponseDTO
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; } = null!;

        public IEnumerable<ValidationError> Errors { get; set; } = new List<ValidationError>();
    }
}
