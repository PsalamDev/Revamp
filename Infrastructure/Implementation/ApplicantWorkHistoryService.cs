using AutoMapper;
using Core.Common.Model;
using Core.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using HRShared.Common;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation
{
    public class ApplicantWorkHistoryService : IApplicantWorkHistory
    {
        private readonly IAsyncRepository<ApplicantWorkHistory, Guid> _applicantWorkHistoryRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicantWorkHistoryService> _logger;
        private readonly ApplicationDbContext _context;

        public ApplicantWorkHistoryService(IAsyncRepository<ApplicantWorkHistory, Guid> applicantWorkHistoryRepository, ICurrentUser currentUser, IMapper mapper,
            ILogger<ApplicantWorkHistoryService> logger, ApplicationDbContext context)
        {
            _applicantWorkHistoryRepository = applicantWorkHistoryRepository;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
            _context = context;
        }


        public async Task<ResponseModel<ApplicantHistoryResponse>> CreateAsync(ApplicantHistoryRequest request)
        {
            try
            {

                var companyId = Guid.Parse(_currentUser.GetCompany());

                var applicantHistory = new ApplicantWorkHistory()
                {
                    Id = SequentialGuid.Create(),
                    CompanyName = request.CompanyName,
                    Department = request.Department,
                    JobTitle = request.JobTitle,
                    JobDescription = request.JobDescription,
                    Grade = request.Grade,
                    ApplicantId = request.ApplicantId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedByName = _currentUser.GetFullname(),
                    CreatedDate = DateTime.Now,
                    IsCurrent = request.IsCurrent
                };

                _applicantWorkHistoryRepository.Add(applicantHistory);
                await _applicantWorkHistoryRepository.SaveChangesAsync();

                return ResponseModel<ApplicantHistoryResponse>.Success(_mapper.Map<ApplicantHistoryResponse>(applicantHistory));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving employee work history: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<ApplicantHistoryResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<ApplicantHistoryResponse>> UpdateAsync(UpdateApplicantHistoryRequest request)
        {
            try
            {

                var empHistory = await _applicantWorkHistoryRepository.GetByAsync(x => x.Id == request.Id);
                if (empHistory == null)
                {
                    return ResponseModel<ApplicantHistoryResponse>.Failure($"{request.ApplicantId} history record not found");
                }

                empHistory.CompanyName = request.CompanyName;
                empHistory.Department = request.Department;
                empHistory.JobTitle = request.JobTitle;
                empHistory.JobDescription = request.JobDescription;
                empHistory.Grade = request.Grade;
                empHistory.ApplicantId = request.ApplicantId;
                empHistory.StartDate = request.StartDate;
                empHistory.EndDate = request.EndDate;
                empHistory.ModifiedBy = _currentUser.GetFullname();
                empHistory.ModifiedDate = DateTime.Now;
                empHistory.IsCurrent = request.IsCurrent;


                _applicantWorkHistoryRepository.Update(empHistory);
                await _applicantWorkHistoryRepository.SaveChangesAsync();



                return ResponseModel<ApplicantHistoryResponse>.Success(_mapper.Map<ApplicantHistoryResponse>(empHistory));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving applicant history: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ApplicantHistoryResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<ApplicantHistoryResponse>> GetSingleAsync(Guid id)
        {
            try
            {
                using (_context)
                {
                    var appHistory = await _context.ApplicantWorkHistories.Where(x => x.ApplicantId == id).FirstOrDefaultAsync();
                    if (appHistory != null)
                    {
                        return ResponseModel<ApplicantHistoryResponse>.Failure($"cannot find history record for this employee");
                    }



                    return ResponseModel<ApplicantHistoryResponse>.Success(_mapper.Map<ApplicantHistoryResponse>(appHistory));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting employee history: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<ApplicantHistoryResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<CustomPagination<List<ApplicantHistoryResponse>>>> GetAllListAsync(GetHistoryRequestList request)
        {
            try
            {
                using (_context)
                {
                    var applicantHistories = await (from emphist in _context.ApplicantWorkHistories
                                                    where emphist.ApplicantId == request.ApplcantId && emphist.IsDeleted == false
                                                    select new ApplicantHistoryResponse()
                                                    {

                                                        Id = emphist.Id,
                                                        ApplicantId = emphist.ApplicantId,
                                                        EndDate = emphist.EndDate,
                                                        StartDate = emphist.StartDate,
                                                        CompanyName = emphist.CompanyName,
                                                        Department = emphist.Department,
                                                        CreatedDate = emphist.CreatedDate,
                                                        JobTitle = emphist.JobTitle,
                                                        Grade = emphist.Grade,
                                                        IsCurrent = emphist.IsCurrent,
                                                        JobDescription = emphist.JobDescription,
                                                        ModifiedDate = emphist.ModifiedDate,
                                                        ModifiedByIp = emphist.ModifiedByIp,
                                                        ModifiedBy = emphist.ModifiedBy,
                                                        CreatedByIp = emphist.CreatedByIp,
                                                        IsDeleted = emphist.IsDeleted,
                                                        CreatedBy = emphist.CreatedBy

                                                    }).ToListAsync();


                    CustomPagination<List<ApplicantHistoryResponse>> model = new CustomPagination<List<ApplicantHistoryResponse>>
                    {

                        modelresult = applicantHistories,
                        TotalCount = applicantHistories.Count,
                        pageNumber = 0,
                        pageSize = 0

                    };

                    return ResponseModel<CustomPagination<List<ApplicantHistoryResponse>>>.Success(model);

                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant history: {ex.Message}", nameof(GetAllListAsync));
                return ResponseModel<CustomPagination<List<ApplicantHistoryResponse>>>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                using (_context)
                {
                    var record = await _context.ApplicantWorkHistories.Where(x => x.Id == id).FirstOrDefaultAsync();
                    record.IsDeleted = true;
                    _context.ApplicantWorkHistories.Update(record);
                    await _context.SaveChangesAsync();

                    return ResponseModel<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating applicant work history: {ex.Message}", nameof(DeleteAsync));
                return ResponseModel<bool>.Exception("Exception error " + ex.Message);
            }
        }
    }
}