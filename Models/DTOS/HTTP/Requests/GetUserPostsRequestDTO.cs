namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public class GetUserPostsRequestDTO : PaginationAbstractRequestDTO
    {
        public bool IsVisible { get; set; } = true;
    }
}
