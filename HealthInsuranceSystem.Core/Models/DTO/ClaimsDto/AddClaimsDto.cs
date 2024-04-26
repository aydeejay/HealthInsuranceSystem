using FluentValidation;

namespace HealthInsuranceSystem.Core.Models.DTO.ClaimsDto
{
    public class AddClaimDto
    {
        public string PolicyHolderId { get; set; }
        public string NationalID { get; set; }
        public decimal ExpenseAmount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string ExpenseDescription { get; set; }
    }
    public class AddClaimDtoValidator : AbstractValidator<AddClaimDto>
    {
        public AddClaimDtoValidator()
        {
            RuleFor(x => x.PolicyHolderId).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.NationalID).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.ExpenseDate).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.ExpenseAmount).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.ExpenseDescription).NotEmpty().WithMessage("{PropertyName} is required");
        }
    }
}