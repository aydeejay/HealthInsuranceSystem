using AutoMapper;

using CSharpFunctionalExtensions;

using HealthInsuranceSystem.Core.Data.PageQuery;
using HealthInsuranceSystem.Core.Data.Repository.IRepository;
using HealthInsuranceSystem.Core.Extensions;
using HealthInsuranceSystem.Core.Extensions.Constants;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;
using HealthInsuranceSystem.Core.Services.IService;

using Microsoft.EntityFrameworkCore;


namespace HealthInsuranceSystem.Core.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfigurationProvider _configurationProvider;

        public ClaimService(IUnitOfWork unitOfWork, IMapper mapper, IConfigurationProvider configurationProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configurationProvider = configurationProvider;
        }
        public async Task<Result<ResponseModel<PagedQueryResult<GetClaimDto>>>> GetAllClaims(PaginatedQuery query)
        {
            var result = new ResponseModel<PagedQueryResult<GetClaimDto>>();

            var claimQueryable = _unitOfWork.ClaimRepository.GetQueryable();
            var claims = claimQueryable
                .AsNoTracking()
                .Include(x => x.AssignedUser)
                .Include(x => x.PolicyHolder)
                .AsQueryable();

            var search = query.Search?.ToLower();

            if (!string.IsNullOrEmpty(search))
            {
                claims = claims.Where(x =>
                    x.Id.Equals(search) ||
                    x.CreatedDate.Equals(search) ||
                    x.ExpenseAmount.Equals(search) ||
                    x.PolicyHolderUserId.Equals(search));
            }

            if (query.PolicyHolderId != null && query.PolicyHolderId != 0)
            {
                claims = claims.Where(x => x.PolicyHolderUserId == query.PolicyHolderId);
            }

            result.Data = await claims.
                ToPagedResult<Claim, GetClaimDto>(query.PageQuery.PageNumber, query.PageQuery.PageSize, _mapper.ConfigurationProvider);

            return result;
        }

        public async Task<Result<ResponseModel>> AddClaim(AddClaimDto request)
        {
            var response = new ResponseModel();

            var user = await _unitOfWork.UserRepository.GetFirstOrDefault(x => (x.NationalID == request.NationalID && x.UserPolicyNumber == request.PolicyHolderId));
            if (user == null)
                return Result.Failure<ResponseModel>($"{UserConstants.ErrorMessages.UserNotFoundWithIDandPolicyNo}");

            var claim = _mapper.Map<Claim>(request);
            claim.CurrentStatus = ClaimStatus.Submitted.ToString();
            claim.CreatedDate = DateTime.Now;
            claim.LastModifiedDate = DateTime.Now;
            claim.PolicyHolderUserId = user.Id;
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

        public async Task<Result<ResponseModel>> ReviewClaim(ReviewClaimDto request)
        {
            var response = new ResponseModel();
            bool IsInvalidRequest = true;
            string errorMessage = "";

            var claim = await _unitOfWork.ClaimRepository.GetFirstOrDefault(x => x.Id == request.ClaimId);

            if (claim == null)
                return Result.Failure<ResponseModel>($"{ClaimConstants.ErrorMessages.ClaimsNotFound}");

            //Check if the assigned user is the request user where it is not the policyholder... give exception to admin

            var claimsAudit = new ClaimsAudit()
            {
                PolicyHolderId = claim.PolicyHolderUserId,
                Comment = request.Comment,
                OldStatus = claim.CurrentStatus,
                NewStatus = request.Status.ToString(),
                AssignedUserId = claim.PolicyHolderUserId, // Need to get user form authentication
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
                        claimsAudit.AssignedUserId = claim.PolicyHolderUserId;
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
                        errorMessage = "Status needs to be set as In-Review first - Status Id 3";
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


            response.Message = ClaimConstants.Messages.ClaimsStatusUpdateSuccessful;
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
