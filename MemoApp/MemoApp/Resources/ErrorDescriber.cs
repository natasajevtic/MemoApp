using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace MemoApp.Resources
{
    public class ErrorDescriber : IdentityErrorDescriber
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ErrorDescriber(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = string.Format(_localizer["Email {0} is already taken."], email)
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = string.Format(_localizer["Role name {0} is already taken."], role)
            };
        }
    }
}
