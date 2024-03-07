using FluentValidation;
using Tokengram.Constants;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class PaginationRequestDTOValidator<T> : AbstractValidator<T>
        where T : PaginationRequestDTO
    {
        public PaginationRequestDTOValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(PaginationSettings.MIN_PAGE_NUMBER);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(PaginationSettings.MIN_PAGE_SIZE)
                .LessThanOrEqualTo(PaginationSettings.MAX_PAGE_SIZE);
        }
    }
}
