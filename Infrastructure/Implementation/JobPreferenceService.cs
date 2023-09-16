using AutoMapper;
using Core.Common.Model;
using Core.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using HRShared.Common;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation
{
    public class JobPreferenceService : IJobPreferenceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<JobPreferenceService> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IAsyncRepository<JobPreference, Guid> _jobPreferenceRepository;
        private readonly IMapper _mapper;
        private readonly Guid companyId;
        public JobPreferenceService(ApplicationDbContext dbContext, ILogger<JobPreferenceService> logger, ICurrentUser currentUser,
                                    IAsyncRepository<JobPreference, Guid> jobPreferenceRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currentUser = currentUser;
            _mapper = mapper;
            _jobPreferenceRepository = jobPreferenceRepository;
            companyId = Guid.Parse(_currentUser.GetCompany());
        }

        public async Task<ResponseModel<JobPreferenceModel>> CreateAsync(CreateJobPreferenceDto request)
        {
            try
            {
                var record = _mapper.Map<JobPreference>(request);

                var applicantProfile = await _dbContext.ApplicantProfiles
                                    .FirstOrDefaultAsync(x => x.Id == request.ApplicantId && x.IsDeleted == false);

                if (applicantProfile == null)
                {
                    return ResponseModel<JobPreferenceModel>.Failure("Applicant Not Found");
                }

                record.Id = SequentialGuid.Create();
                record.CompanyId = companyId;
                record.ApplicantprofileId = _currentUser.GetUserId();
                record.CreatedBy = _currentUser.GetUserId();
                record.CreatedByIp = _currentUser.GetFullname();
                record.CreatedDate = DateTime.Now;
                record.ApplicantProfile = applicantProfile;
                record.Experiencelevel = record.Experiencelevel;

                await _jobPreferenceRepository.AddAsync(record);
                await _jobPreferenceRepository.SaveChangesAsync();

                return ResponseModel<JobPreferenceModel>.Success(_mapper.Map<JobPreferenceModel>(record));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while creating Score Card record: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<JobPreferenceModel>.Failure("Exception error");
            }
        }
        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var jobPreferenceExist = await _jobPreferenceRepository.GetByAsync(x => x.Id == id && x.IsDeleted == false);

                if (jobPreferenceExist == null)
                {
                    return ResponseModel<bool>.Failure("Score Card does exist");
                }

                jobPreferenceExist.IsDeleted = true;
                _jobPreferenceRepository.Update(jobPreferenceExist);


                await _jobPreferenceRepository.SaveChangesAsync();
                return ResponseModel<bool>.Success(_mapper.Map<bool>(true));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting Score Card record by id: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<CustomPagination<List<JobPreferenceDto>>>> GetAllAsync(JobPreferenceFilter filter)
        {
            var result = new List<JobPreferenceDto>();
            try
            {
                result = await _dbContext.JobPreferences
                    .Where(s => s.IsDeleted == false && s.CompanyId == companyId && s.ApplicantprofileId == filter.ApplicantId)
                    .Select(s => new JobPreferenceDto
                    {
                        Id = s.Id,
                        JobTitle = s.JobTitle,
                        SalaryRangeFrom = s.SalaryRangeFrom,
                        SalaryRangeTo = s.SalaryRangeTo,
                        ApplicantprofileId = s.ApplicantprofileId,
                        CompanyId = s.CompanyId,
                        EmploymentType = s.EmploymentType,
                        EmploymentTypeText = s.EmploymentType.GetDescription(),
                        Experiencelevel = s.Experiencelevel
                    }).ToListAsync();

                CustomPagination<List<JobPreferenceDto>> response = new CustomPagination<List<JobPreferenceDto>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count
                };

                return ResponseModel<CustomPagination<List<JobPreferenceDto>>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobPreferenceDto>>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<JobPreferenceDto>> GetSingleAsync(Guid id)
        {
            try
            {

                var record = await _dbContext.JobPreferences
                     .Where(s => s.IsDeleted == false && s.CompanyId == companyId && s.IsDeleted == false)
                     .Select(s => new JobPreferenceDto
                     {
                         Id = s.Id,
                         JobTitle = s.JobTitle,
                         SalaryRangeTo = s.SalaryRangeTo,
                         SalaryRangeFrom = s.SalaryRangeFrom,
                         ApplicantprofileId = s.ApplicantprofileId,
                         CompanyId = s.CompanyId,
                         EmploymentType = s.EmploymentType,
                         EmploymentTypeText = s.EmploymentType.GetDescription(),
                         Experiencelevel = s.Experiencelevel
                     }).FirstOrDefaultAsync();

                return ResponseModel<JobPreferenceDto>.Success(record);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting job reference list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<JobPreferenceDto>.Failure("Exception error");
            }
        }


        public async Task<ResponseModel<JobPreferenceModel>> UpdateAsync(UpdateJobPreferencDto request)
        {
            try
            {

                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<JobPreferenceModel>.Failure("Invalid score Card identifier");
                }

                var jobPreference = await _jobPreferenceRepository.GetByAsync(x => x.Id == request.Id && x.IsDeleted == false);

                if (jobPreference == null)
                {
                    return ResponseModel<JobPreferenceModel>.Failure("No record of score card with Identifier found");
                }



                jobPreference.JobTitle = request.JobTitle;
                jobPreference.CompanyId = companyId;
                jobPreference.EmploymentType = request.EmploymentType;
                jobPreference.SalaryRangeFrom = request.SalaryRangeFrom;
                jobPreference.SalaryRangeTo = request.SalaryRangeTo;
                jobPreference.ModifiedBy = _currentUser.GetFullname();
                jobPreference.ModifiedDate = DateTime.Now;
                jobPreference.Experiencelevel = request.Experiencelevel;
                _jobPreferenceRepository.Update(jobPreference);
                await _jobPreferenceRepository.SaveChangesAsync();


                return ResponseModel<JobPreferenceModel>.Success(_mapper.Map<JobPreferenceModel>(jobPreference));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while Job preference: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<JobPreferenceModel>.Failure("Exception error");
            }
        }
    }
}
