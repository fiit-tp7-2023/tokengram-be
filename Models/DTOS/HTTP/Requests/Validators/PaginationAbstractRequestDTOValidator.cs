using FluentValidation;
using Tokengram.Constants;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class PaginationAbstractRequestDTOValidator<T> : AbstractValidator<T>
        where T : PaginationAbstractRequestDTO
    {
        public PaginationAbstractRequestDTOValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(PaginationSettings.MIN_PAGE_NUMBER);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(PaginationSettings.MIN_PAGE_SIZE, PaginationSettings.MAX_PAGE_SIZE);
        }
    }
}
