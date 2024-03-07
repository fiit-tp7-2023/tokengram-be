using Microsoft.AspNetCore.Mvc;
using Tokengram.Constants;

namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public abstract class PaginationRequestDTO
    {
        [FromQuery]
        public int PageNumber { get; set; } = PaginationSettings.DEFAULT_PAGE_NUMBER;

        [FromQuery]
        public int PageSize { get; set; } = PaginationSettings.DEFAULT_PAGE_SIZE;
    }
}
