using FluentValidation;

using HealthInsuranceSystem.Core.Models.Domain;


namespace HealthInsuranceSystem.Core.Models.DTO.UserDto
{
    public class AddUserDto
    {
        public string NationalID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public DateTime DateOfBirth { get; set; }
        public RoleType RoleType { get; set; }
    }
    public class AddUserDtoValidator : AbstractValidator<AddUserDto>
    {
        public AddUserDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.NationalID).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.RoleType).NotEmpty().WithMessage("{PropertyName} is required");
        }
    }
}
