using AutoMapper;

using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;
using HealthInsuranceSystem.Core.Services.IService;


namespace HealthInsuranceSystem.Core.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClaimService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ResponseModel>> AddClaim(AddClaimDto request)
        {
            var response = new ResponseModel();

            var user = await _unitOfWork.UserRepository.GetFirstOrDefault(x => (x.NationalID == request.NationalID));
            if (user == null)
                return Result.Failure<ResponseModel>($"{UserConstants.ErrorMessages.UserNotFoundWithID}");

            var claim = _mapper.Map<Claim>(request);
            claim.CurrentStatus = ClaimStatus.Submitted.ToString();
            claim.CreatedDate = DateTime.Now;
            claim.LastModifiedDate = DateTime.Now;
            try
            {
                await _unitOfWork.ClaimRepository.Add(claim);

                var claimsAudit = new ClaimsAudit()
                {
                    PolicyHolderId = user.Id,
                    NewStatus = ClaimStatus.Submitted.ToString(),
                    DateCreated = DateTime.UtcNow,
                    AssignedUserId = user.Id
                };
                await _unitOfWork.ClaimsAuditRepository.Add(claimsAudit);
                await _unitOfWork.SaveAsync();
                response.IsSuccessful = true;
                response.Message = ClaimConstants.Messages.ClaimsCreatedSuccessful;
            }
            catch (Exception ex)
            {
                return Result.Failure<ResponseModel>($"{ClaimConstants.ErrorMessages.ClaimsFailed} - {ex.Message} : {ex.InnerException}");
            }
            return response;
        }

        public async Task<Result<ResponseModel>> UpdateClaim(UpdateClaimDto request)
        {
            var response = new ResponseModel();
            bool IsInvalidRequest = true;
            string errorMessage = "";

            var claim = await _unitOfWork.ClaimRepository.GetFirstOrDefault(x => x.ClaimId == request.ClaimId);

            if (claim == null)
                return Result.Failure<ResponseModel>($"{ClaimConstants.ErrorMessages.ClaimsNotFound}");

            //Check if the assigned user is the request user where it is not the policyholder... give exception to admin

            var claimsAudit = new ClaimsAudit()
            {
                PolicyHolderId = claim.PolicyHolderId,
                Comment = request.Comment,
                OldStatus = claim.CurrentStatus,
                NewStatus = request.Status.ToString(),
                AssignedUserId = claim.PolicyHolderId, // Need to get user form authentication
                DateCreated = DateTime.UtcNow,
            };

            switch (request.Status)
            {
                case ClaimStatus.Submitted:
                    if (claim.CurrentStatus == ClaimStatus.Submitted.ToString())
                    {
                        errorMessage = "Status is already in the submitted state";
                    }
                    else if (claim.CurrentStatus == ClaimStatus.InReview.ToString())
                    {
                        IsInvalidRequest = false;
                        claim.CurrentStatus = request.Status.ToString();
                        claimsAudit.AssignedUserId = claim.PolicyHolderId;
                    }
                    else
                    {
                        errorMessage = $"Status cannot be modifies since it is in the {claim.CurrentStatus} state";
                    }

                    break;
                case ClaimStatus.InReview:
                    if (claim.CurrentStatus == ClaimStatus.Submitted.ToString())
                    {
                        IsInvalidRequest = false;
                        claim.CurrentStatus = request.Status.ToString();
                    }
                    else if (claim.CurrentStatus == ClaimStatus.InReview.ToString())
                    {
                        errorMessage = "Status is already in the In-Review state";
                    }
                    else
                    {
                        errorMessage = $"Status cannot be modifies since it is in the {claim.CurrentStatus} state";
                    }

                    break;
                case ClaimStatus.Approved:
                case ClaimStatus.Declined:
                    if (claim.CurrentStatus == ClaimStatus.Submitted.ToString())
                    {
                        errorMessage = "Status needs to be set as In-Review first";
                    }
                    else if (claim.CurrentStatus == ClaimStatus.InReview.ToString())
                    {
                        IsInvalidRequest = false;
                        claim.CurrentStatus = request.Status.ToString();
                    }
                    else
                    {
                        errorMessage = $"Status is already in it's final state {claim.CurrentStatus}";
                    }

                    break;

                default:
                    // do nothing since we have retrieve the default value already
                    break;
            }

            if (IsInvalidRequest)
            {
                return Result.Failure<ResponseModel>(errorMessage);
            }


            await _unitOfWork.ClaimsAuditRepository.Add(claimsAudit);

            claim.LastModifiedDate = DateTime.Now;
            _unitOfWork.ClaimRepository.Update(claim);


            response.Message = ClaimConstants.Messages.ClaimsUpdateSuccessful;
            response.IsSuccessful = true;

            try
            {
                await _unitOfWork.SaveAsync();

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return Result.Failure<ResponseModel>($"{ClaimConstants.ErrorMessages.ClaimsFailed} - {ex.Message} : {ex.InnerException}");
            }
        }
    }
}
