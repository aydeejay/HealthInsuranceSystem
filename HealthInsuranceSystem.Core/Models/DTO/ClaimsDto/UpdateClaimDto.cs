using FluentValidation;

using HealthInsuranceSystem.Core.Models.Domain;

namespace HealthInsuranceSystem.Core.Models.DTO.ClaimsDto
{
    public class UpdateClaimDto
    {
        public int ClaimId { get; set; }
        public ClaimStatus Status { get; set; }
        public string Comment { get; set; }
    }
    public class UpdateClaimDtoValidator : AbstractValidator<UpdateClaimDto>
    {
        public UpdateClaimDtoValidator()
        {
            RuleFor(x => x.ClaimId).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.Status).NotEmpty().WithMessage("{PropertyName} is required");
            RuleFor(x => x.Comment).NotEmpty().WithMessage("{PropertyName} is required");
        }
    }
}