using FluentValidation;

namespace ECommerce.Application.Usecases.Users.Queries.GetUser
{
    public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidator()
        {
            RuleFor(u => u.Id)
                .NotEmpty()
                .WithMessage("this field is Required");
        }
    }
}
