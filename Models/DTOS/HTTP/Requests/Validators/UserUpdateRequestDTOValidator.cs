using FluentValidation;
using Tokengram.Constants;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class UserUpdateRequestDTOValidator : BaseValidator<UserUpdateRequest>
    {
        public UserUpdateRequestDTOValidator()
        {
            When(
                x => x.Username != null,
                () =>
                {
                    RuleFor(x => x.Username)
                        .Length(ProfileSettings.MIN_USERNAME_LENGTH, ProfileSettings.MAX_USERNAME_LENGTH);
                }
            );

            When(
                x => x.ProfilePicture != null,
                () =>
                {
                    RuleFor(x => x.ProfilePicture!).SetValidator(new ProfilePictureFileValidator());
                }
            );
        }
    }

    public class ProfilePictureFileValidator : AbstractValidator<IFormFile>
    {
        public ProfilePictureFileValidator()
        {
            RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(ProfileSettings.MAX_PROFILE_PIC_SIZE);
            RuleFor(x => x.FileName.Length).LessThanOrEqualTo(100);
            RuleFor(x => Path.GetExtension(x.FileName)).Length(4, 5);
            RuleFor(x => x.ContentType)
                .NotNull()
                .Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"));
        }
    }
}
