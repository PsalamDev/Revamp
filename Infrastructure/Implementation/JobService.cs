using AutoMapper;
using Grpc.Net.Client;
using HRShared.Common;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Domain.Entities;
using Domain.Interfaces;
using Core.Interfaces;
using Core.Common.Model.RecruitmentDto;
using Core.Common.Model;
using static Domain.Enums.Enum;

namespace Infrastructure.Implementation
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<JobService> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IAsyncRepository<Job, Guid> _repository;
        private readonly IAsyncRepository<JobReviewer, Guid> _jobReviewerRepository;
        private readonly IAsyncRepository<JobApplication, Guid> _jobApplicationRepository;
        private readonly IAsyncRepository<JobInterviewHistory, Guid> _jobInterviewHistoryRepository;
        private readonly IAsyncRepository<ApplicantQuizRecord, Guid> _applicantQuizRecordRepository;
        private readonly IAsyncRepository<JobScheduleInterview, Guid> _jobScheduleInterviewsRepository;
        private readonly IRecruitmentJobApplicationServices _jobApplicationServices;
        private readonly IMapper _mapper;
        private readonly Guid companyId;
        public JobService(ApplicationDbContext dbContext, ILogger<JobService> logger, ICurrentUser currentUser,
                                    IAsyncRepository<Job, Guid> repository, IMapper mapper,
                                    IAsyncRepository<JobReviewer, Guid> jobReviewerRepository,
                                    IAsyncRepository<JobApplication, Guid> jobApplicationRepository,
                                    IAsyncRepository<JobInterviewHistory, Guid> jobInterviewHistoryRepository,
                                    IAsyncRepository<ApplicantQuizRecord, Guid> applicantQuizRecordRepository,
                                    IAsyncRepository<JobScheduleInterview, Guid> jobScheduleInterviewsRepository,
                                    IRecruitmentJobApplicationServices jobApplicationServices)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currentUser = currentUser;
            _repository = repository;
            _mapper = mapper;
            companyId = Guid.Parse(_currentUser.GetCompany());
            _jobReviewerRepository = jobReviewerRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _jobInterviewHistoryRepository = jobInterviewHistoryRepository;
            _applicantQuizRecordRepository = applicantQuizRecordRepository;
            _jobScheduleInterviewsRepository = jobScheduleInterviewsRepository;
            _jobApplicationServices = jobApplicationServices;
        }

        public async Task<ResponseModel<JobModel>> CreateAsync(CreateJobDto request)
        {
            try
            {

                //if (request.JobPostStatus == JobPostStatus.Post)
                //{
                //    if (request.DatePosted != DateTime.Now)
                //    {
                //        return ResponseModel<JobModel>.Failure("The start date for this job posting is in the future, please use the schedule functionality for scheduled job postings");
                //    }
                //}

                if (request.JobPostStatus == JobPostStatus.Schedule)
                {
                    if (request.SchedulePostDate < DateTime.Now)
                    {
                        return ResponseModel<JobModel>.Failure("Schdule post date must be Future date");
                    }
                }

                var job = new Job()
                {
                    Id = SequentialGuid.Create(),
                    JobTitle = request.JobTitle,
                    JobAvailability = request.JobAvailability,
                    CompanyId = companyId,
                    CountryId = request.CountryId,
                    DepartmentId = request.DepartmentId,
                    Currency = request.Currency,
                    Description = request.Description,
                    EmploymentType = request.EmploymentType,
                    MaxEducation = request.MaxEducation,
                    MinEducation = request.MinEducation,
                    PostValidityFrom = request.PostValidityFrom,
                    QuizId = request.QuizId,
                    Requirements = request.Requirements,
                    Experience = request.Experience,
                    PostValidityTo = request.PostValidityTo,
                    ScoreCardId = request.ScoreCardId,
                    StateId = request.StateId,
                    SchedulePostDate = request.SchedulePostDate,
                    JobPostStatus = request.JobPostStatus,
                    DatePosted = request.JobPostStatus == JobPostStatus.Post ? DateTime.Now : null,
                    SalaryRange = $"{request.MinSalary} - {request.MaxSalary}",
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedByIp = _currentUser.GetFullname(),
                    CreatedDate = DateTime.Now,
                    SalaryRangeFrom = request.MinSalary,
                    SalaryRangeTo = request.MaxSalary,
                    CreatedByName = _currentUser.GetFullname()

                };
                await _repository.AddAsync(job);
                await _repository.SaveChangesAsync();

                if (request.JobReviewers != null)
                {
                    foreach (var reviwer in request.JobReviewers)
                    {
                        var jobReviwer = new JobReviewer
                        {
                            Id = SequentialGuid.Create(),
                            JobId = job.Id,
                            ReviewerName = reviwer.ReviewerName,
                            ReviewerId = reviwer.ReviewerId,
                            ReviewerEmail = reviwer.ReviewerEmail,
                            CreatedBy = _currentUser.GetUserId(),
                            IsReviewd = false,
                            Status = ReviwerStatus.Pending,
                            CreatedByIp = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now,
                            CompanyId = companyId,
                            CreatedByName = _currentUser.GetFullname()
                        };

                        await _jobReviewerRepository.AddAsync(jobReviwer);
                    }
                    await _jobReviewerRepository.SaveChangesAsync();
                }
                return ResponseModel<JobModel>.Success(_mapper.Map<JobModel>(job));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while creating Job record: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<JobModel>.Failure("Exception error");
            }
        }
        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {

            try
            {
                var jobExist = await _repository.GetByAsync(x => x.Id == id);

                if (jobExist == null)
                {
                    return ResponseModel<bool>.Failure("Job does exist");
                }

                jobExist.IsDeleted = true;
                _repository.Update(jobExist);
                await _repository.SaveChangesAsync();

                var jobReviewers = _dbContext.JobReviewers.Where(a => a.JobId == id).ToList();


                if (jobReviewers.Count() > 0)
                {
                    foreach (var deleteJobReviewer in jobReviewers)
                    {
                        deleteJobReviewer.IsDeleted = true;
                        deleteJobReviewer.ModifiedBy = _currentUser.GetFullname();
                        deleteJobReviewer.ModifiedDate = DateTime.Now;

                        _dbContext.JobReviewers.Update(deleteJobReviewer);
                    }

                    await _dbContext.SaveChangesAsync();
                }


                return ResponseModel<bool>.Success(true);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting stage record by id: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }
        public async Task<ResponseModel<CustomPagination<List<JobDto>>>> GetAllAsync(JobFilterDto filter)
        {
            var result = new List<JobDto>();
            try
            {
                result = await _dbContext.Jobs.Include(x => x.JobReviewers)
                              .Where(s => s.IsDeleted == false && s.CompanyId == companyId && s.IsDeleted == false && s.JobPostStatus == filter.JobPostStatus)
                              .Select(s => new JobDto
                              {
                                  Id = s.Id,
                                  JobTitle = s.JobTitle,
                                  Availability = s.JobAvailability,
                                  CompanyId = companyId,
                                  CountryId = s.CountryId,
                                  DepartmentId = s.DepartmentId,
                                  Currency = s.Currency,
                                  Description = s.Description,
                                  EmploymentType = s.EmploymentType,
                                  MaximumEducation = s.MaxEducation,
                                  MinimumEducation = s.MinEducation,
                                  PostValidityFrom = s.PostValidityFrom,
                                  QuizId = s.QuizId,
                                  Requirements = s.Requirements,
                                  Experience = s.Experience,
                                  PostValidityTo = s.PostValidityTo,
                                  ScoreCardId = s.ScoreCardId,
                                  StateId = s.StateId,
                                  IsDeleted = s.IsDeleted,
                                  DateCreated = s.CreatedDate,
                                  DatePosted = s.DatePosted,
                                  SalaryRange = s.SalaryRange,
                                  JobPostStatus = s.JobPostStatus,
                                  Status = s.JobStatus,
                                  EmploymentTypeText = s.EmploymentType.GetDescription(),
                                  JobPostStatusText = s.JobPostStatus.GetDescription(),
                                  AvailabilityText = s.JobAvailability.GetDescription(),
                                  JobStatusText = s.JobStatus.GetDescription(),
                                  SalaryRangeFrom = s.SalaryRangeFrom,
                                  SalaryRangeTo = s.SalaryRangeTo,
                                  SchedulePostDate = s.SchedulePostDate,
                                  JobReviewers = s.JobReviewers.Select(x => new JobReviewerModel
                                  {
                                      Id = x.Id,
                                      ReviewerId = x.ReviewerId,
                                      ReviewerEmail = x.ReviewerEmail,
                                      ReviewerName = x.ReviewerName,
                                      JobId = x.JobId,
                                      ReviwerStatus = x.Status,
                                      Comment = x.Comment,
                                      ReviwerStatusText = x.Status != null ? x.Status.GetDescription() : "",
                                      CreatedBy = x.CreatedBy
                                  }).ToList()
                              }).ToListAsync();


                if (!string.IsNullOrEmpty(filter.JobTitle))
                {
                    result = result.Where(x => x.JobTitle == filter.JobTitle).DefaultIfEmpty().ToList();
                }
                if (filter.Status.HasValue)
                {
                    result = result.Where(x => x.Status == filter.Status).DefaultIfEmpty().ToList();
                }
                if (filter.DatePosted.HasValue)
                {
                    result = result.Where(x => x.DateCreated.Date == filter.DatePosted).ToList();
                }
                if (filter.DepartmentId.HasValue && filter.DepartmentId != Guid.Empty)
                {
                    result = result.Where(x => x.DepartmentId == filter.DepartmentId).ToList();
                }

                CustomPagination<List<JobDto>> response = new CustomPagination<List<JobDto>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count
                };

                return ResponseModel<CustomPagination<List<JobDto>>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobDto>>>.Exception(ex.Message);
            }
        }
        public async Task<ResponseModel<JobDto>> GetSingleAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.Jobs.Include(x => x.JobReviewers)
                              .Where(s => s.IsDeleted == false
                              && s.CompanyId == companyId
                              && s.IsDeleted == false && s.Id == id)
                              .Select(s => new JobDto
                              {
                                  Id = s.Id,
                                  JobTitle = s.JobTitle,
                                  Availability = s.JobAvailability,
                                  CompanyId = companyId,
                                  CountryId = s.CountryId,
                                  DepartmentId = s.DepartmentId,
                                  Currency = s.Currency,
                                  Description = s.Description,
                                  EmploymentType = s.EmploymentType,
                                  MaximumEducation = s.MaxEducation,
                                  MinimumEducation = s.MinEducation,
                                  PostValidityFrom = s.PostValidityFrom,
                                  QuizId = s.QuizId,
                                  Requirements = s.Requirements,
                                  Experience = s.Experience,
                                  PostValidityTo = s.PostValidityTo,
                                  ScoreCardId = s.ScoreCardId,
                                  StateId = s.StateId,
                                  IsDeleted = s.IsDeleted,
                                  DateCreated = s.CreatedDate,
                                  SalaryRange = s.SalaryRange,
                                  Status = s.JobStatus,
                                  JobPostStatus = s.JobPostStatus,
                                  SchedulePostDate = s.SchedulePostDate,
                                  DatePosted = s.DatePosted,
                                  EmploymentTypeText = s.EmploymentType.GetDescription(),
                                  JobPostStatusText = s.JobPostStatus.GetDescription(),
                                  AvailabilityText = s.JobAvailability.GetDescription(),
                                  JobStatusText = s.JobStatus.GetDescription(),
                                  JobReviewers = s.JobReviewers.Select(x => new JobReviewerModel
                                  {
                                      Id = x.Id,
                                      ReviewerId = x.ReviewerId,
                                      ReviewerEmail = x.ReviewerEmail,
                                      ReviewerName = x.ReviewerName,
                                      ReviwerStatus = x.Status,
                                      JobId = x.JobId,
                                      Comment = x.Comment,
                                      ReviwerStatusText = x.Status != null ? x.Status.GetDescription() : "",
                                      CreatedBy = x.CreatedBy
                                  }).ToList()
                              }).FirstOrDefaultAsync();

                if (data != null)
                {
                    return ResponseModel<JobDto>.Success(data);
                }
                else
                {
                    return ResponseModel<JobDto>.Success(data, "No Result Found");
                }
            }


            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting job record list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<JobDto>.Failure("Exception error");
            }
        }
        public async Task<ResponseModel<JobModel>> UpdateAsync(UpdateJobDto request)
        {

            try
            {
                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<JobModel>.Failure("Invalid score Card identifier");
                }

                var job = await _repository.GetByAsync(x => x.Id == request.Id);

                if (job == null)
                {
                    return ResponseModel<JobModel>.Failure("No record of job with Identifier found");
                }

                if (job.JobPostStatus == JobPostStatus.Schedule)
                {
                    if (request.SchedulePostDate < DateTime.Now)
                    {
                        return ResponseModel<JobModel>.Failure("Schdule post date must be Future date");
                    }
                }

                job.JobTitle = job.JobTitle;
                job.JobAvailability = request.JobAvailability;
                job.CountryId = request.CountryId;
                job.DepartmentId = request.DepartmentId;
                job.Currency = request.Currency;
                job.Description = request.Description;
                job.EmploymentType = request.EmploymentType;
                job.MaxEducation = request.MaxEducation;
                job.SalaryRange = $"{request.MinSalary} - {request.MaxSalary}";
                job.MinEducation = request.MinEducation;
                job.PostValidityFrom = request.PostValidityFrom;
                job.QuizId = request.QuizId;
                job.Requirements = request.Requirements;
                job.Experience = request.Experience;
                job.PostValidityTo = request.PostValidityTo;
                job.ScoreCardId = request.ScoreCardId;
                job.StateId = request.StateId;
                job.JobTitle = request.JobTitle;
                job.CompanyId = companyId;
                job.Description = request.Description;
                job.ModifiedBy = _currentUser.GetFullname();
                job.ModifiedDate = DateTime.Now;
                job.JobStatus = request.JobStatus;
                job.JobPostStatus = request.JobPostStatus;
                job.DatePosted = request.JobPostStatus == JobPostStatus.Post ? DateTime.Now : request.PostValidityFrom;
                job.SchedulePostDate = request.JobPostStatus == JobPostStatus.Schedule ? request.SchedulePostDate : null;


                _repository.Update(job);
                await _repository.SaveChangesAsync();

                var existingJobReviewers = await _jobReviewerRepository.ListAsync(x => x.JobId == request.Id);

                if (existingJobReviewers.Count() == 0 || existingJobReviewers == null)
                {
                    if (request.JobReviewers != null)
                    {
                        foreach (var item in request.JobReviewers)
                        {
                            if (item != null)
                            {
                                var jobReviwer = new JobReviewer
                                {
                                    Id = SequentialGuid.Create(),
                                    ReviewerName = item.ReviewerName,
                                    ReviewerId = item.ReviewerId,
                                    CreatedBy = _currentUser.GetUserId(),
                                    JobId = job.Id,
                                    CreatedDate = DateTime.Now,
                                    ReviewerEmail = item.ReviewerEmail,
                                    CompanyId = companyId,
                                    Status = ReviwerStatus.Pending,
                                    IsReviewd = false,
                                    CreatedByName = _currentUser.GetFullname()
                                };

                                await _jobReviewerRepository.AddAsync(jobReviwer);
                                await _jobReviewerRepository.SaveChangesAsync();
                            }
                        }
                    }
                }
                else if (existingJobReviewers.Count() > 0)
                {
                    // Remove reviewers that are not in the incoming list
                    var reviewersToRemove = existingJobReviewers.Where(r => !request.JobReviewers.Any(x => x.Id == r.Id));
                    if (reviewersToRemove != null && reviewersToRemove.Any())
                    {
                        _jobReviewerRepository.DeleteList(reviewersToRemove.ToList());
                        await _jobReviewerRepository.SaveChangesAsync();
                    }

                    // Add new reviewers
                    if (request.JobReviewers != null)
                    {
                        foreach (var item in request.JobReviewers)
                        {
                            if (!existingJobReviewers.Any(x => x.Id == item.Id))
                            {
                                var jobReviewer = new JobReviewer
                                {
                                    Id = SequentialGuid.Create(),
                                    ReviewerName = item.ReviewerName,
                                    ReviewerId = item.ReviewerId,
                                    CreatedBy = _currentUser.GetUserId(),
                                    JobId = job.Id,
                                    CreatedDate = DateTime.Now,
                                    ReviewerEmail = item.ReviewerEmail,
                                    CompanyId = companyId,
                                    Status = ReviwerStatus.Pending,
                                    IsReviewd = false,
                                    CreatedByName = _currentUser.GetFullname()
                                };

                                await _jobReviewerRepository.AddAsync(jobReviewer);
                                await _jobReviewerRepository.SaveChangesAsync();
                            }
                        }
                    }
                }

                return ResponseModel<JobModel>.Success(_mapper.Map<JobModel>(job));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating score card: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<JobModel>.Failure("Exception error");
            }

            //try
            //{
            //    if (request.Id == Guid.Empty)
            //    {
            //        return ResponseModel<JobModel>.Failure("Invalid score Card identifier");
            //    }

            //    var job = await _repository.GetByAsync(x => x.Id == request.Id);

            //    if (job == null)
            //    {
            //        return ResponseModel<JobModel>.Failure("No record of job with Identifier found");
            //    }

            //    if (job.JobPostStatus == JobPostStatus.Schedule)
            //    {
            //        if (request.SchedulePostDate <= DateTime.Now)
            //        {
            //            return ResponseModel<JobModel>.Failure("Schdule post date must be Future date");
            //        }
            //        else
            //        {
            //            job.JobTitle = job.JobTitle;
            //            job.JobAvailability = request.JobAvailability;
            //            job.CountryId = request.CountryId;
            //            job.DepartmentId = request.DepartmentId;
            //            job.Currency = request.Currency;
            //            job.Description = request.Description;
            //            job.EmploymentType = request.EmploymentType;
            //            job.MaxEducation = request.MaxEducation;
            //            job.SalaryRange = $"{request.MinSalary} - {request.MaxSalary}";
            //            job.MinEducation = request.MinEducation;
            //            job.PostValidityFrom = request.PostValidityFrom;
            //            job.QuizId = request.QuizId;
            //            job.Requirements = request.Requirements;
            //            job.Experience = request.Experience;
            //            job.PostValidityTo = request.PostValidityTo;
            //            job.ScoreCardId = request.ScoreCardId;
            //            job.StateId = request.StateId;
            //            job.JobTitle = request.JobTitle;
            //            job.CompanyId = companyId;
            //            job.Description = request.Description;
            //            job.ModifiedBy = _currentUser.GetFullname();
            //            job.ModifiedDate = DateTime.Now;
            //            job.JobStatus = request.JobStatus;
            //            job.JobPostStatus = request.JobPostStatus;
            //            job.DatePosted = request.JobPostStatus == JobPostStatus.Post ? DateTime.Now : request.SchedulePostDate;
            //            job.SchedulePostDate = request.SchedulePostDate;

            //            _repository.Update(job);
            //            await _repository.SaveChangesAsync();

            //            var existingJobReviewers = await _jobReviewerRepository.ListAsync(x => x.JobId == request.Id);


            //            if (existingJobReviewers.Count() == 0 || existingJobReviewers == null)
            //            {
            //                if (request.JobReviewers != null)
            //                {
            //                    foreach (var item in request.JobReviewers)
            //                    {
            //                        if (item != null)
            //                        {
            //                            var jobReviwer = new JobReviewer
            //                            {
            //                                Id = SequentialGuid.Create(),
            //                                ReviewerName = item.ReviewerName,
            //                                ReviewerId = item.ReviewerId,
            //                                CreatedBy = _currentUser.GetUserId(),
            //                                JobId = job.Id,
            //                                CreatedDate = DateTime.Now,
            //                                ReviewerEmail = item.ReviewerEmail,
            //                                CompanyId = companyId,
            //                                Status = ReviwerStatus.Pending,
            //                                IsReviewd = false,
            //                                CreatedByName = _currentUser.GetFullname()
            //                            };

            //                            await _jobReviewerRepository.AddAsync(jobReviwer);
            //                            await _jobReviewerRepository.SaveChangesAsync();
            //                        }

            //                    }
            //                }

            //            }else if(existingJobReviewers.Count() > 0)
            //            {

            //            }

            //            if (existingJobReviewers.Count() > 0)
            //            {
            //                if (request.JobReviewers != null)
            //                {
            //                    foreach (var item in request.JobReviewers)
            //                    {
            //                        if (existingJobReviewers.Any(x => x.Id != item.Id))
            //                        {
            //                            if (item != null)
            //                            {
            //                                var jobReviwer = new JobReviewer
            //                                {
            //                                    Id = SequentialGuid.Create(),
            //                                    ReviewerName = item.ReviewerName,
            //                                    ReviewerId = item.ReviewerId,
            //                                    CreatedBy = _currentUser.GetUserId(),
            //                                    JobId = job.Id,
            //                                    CreatedDate = DateTime.Now,
            //                                    ReviewerEmail = item.ReviewerEmail,
            //                                    CompanyId = companyId,
            //                                    Status = ReviwerStatus.Pending,
            //                                    IsReviewd = false,
            //                                    CreatedByName = _currentUser.GetFullname()
            //                                };

            //                                await _jobReviewerRepository.AddAsync(jobReviwer);
            //                                await _jobReviewerRepository.SaveChangesAsync();
            //                            }
            //                        }
            //                    }
            //                }

            //            }
            //            //if (existingJobReviewer.Count() > 0)
            //            //{
            //            //    _jobReviewerRepository.DeleteList(existingJobReviewer.ToList());
            //            //    await _jobReviewerRepository.SaveChangesAsync();
            //            //}

            //            return ResponseModel<JobModel>.Success(_mapper.Map<JobModel>(job));
            //        }


            //    }

            //    job.JobTitle = job.JobTitle;
            //    job.JobAvailability = request.JobAvailability;
            //    job.CountryId = request.CountryId;
            //    job.DepartmentId = request.DepartmentId;
            //    job.Currency = request.Currency;
            //    job.Description = request.Description;
            //    job.EmploymentType = request.EmploymentType;
            //    job.MaxEducation = request.MaxEducation;
            //    job.SalaryRange = $"{request.MinSalary} - {request.MaxSalary}";
            //    job.MinEducation = request.MinEducation;
            //    job.PostValidityFrom = request.PostValidityFrom;
            //    job.QuizId = request.QuizId;
            //    job.Requirements = request.Requirements;
            //    job.Experience = request.Experience;
            //    job.PostValidityTo = request.PostValidityTo;
            //    job.ScoreCardId = request.ScoreCardId;
            //    job.StateId = request.StateId;
            //    job.JobTitle = request.JobTitle;
            //    job.CompanyId = companyId;
            //    job.Description = request.Description;
            //    job.ModifiedBy = _currentUser.GetFullname();
            //    job.ModifiedDate = DateTime.Now;
            //    job.JobStatus = request.JobStatus;
            //    job.JobPostStatus = request.JobPostStatus;

            //    _repository.Update(job);
            //    await _repository.SaveChangesAsync();

            //    var existingJobReviewer = await _jobReviewerRepository.ListAsync(x => x.JobId == request.Id);

            //    if (existingJobReviewer.Count() > 0)
            //    {
            //        if (request.JobReviewers != null)
            //        {
            //            foreach (var item in request.JobReviewers)
            //            {
            //                if (existingJobReviewer.Any(x => x.Id != item.Id))
            //                {
            //                    if (item != null)
            //                    {
            //                        var jobReviwer = new JobReviewer
            //                        {
            //                            Id = SequentialGuid.Create(),
            //                            ReviewerName = item.ReviewerName,
            //                            ReviewerId = item.ReviewerId,
            //                            CreatedBy = _currentUser.GetUserId(),
            //                            JobId = job.Id,
            //                            CreatedDate = DateTime.Now,
            //                            ReviewerEmail = item.ReviewerEmail,
            //                            CompanyId = companyId,
            //                            Status = ReviwerStatus.Pending,
            //                            IsReviewd = false,
            //                            CreatedByName = _currentUser.GetFullname()
            //                        };

            //                        await _jobReviewerRepository.AddAsync(jobReviwer);
            //                        await _jobReviewerRepository.SaveChangesAsync();
            //                    }
            //                }
            //            }
            //        }

            //    }
            //    //if (existingJobReviewer.Count() > 0)
            //    //{
            //    //    _jobReviewerRepository.DeleteList(existingJobReviewer.ToList());
            //    //    await _jobReviewerRepository.SaveChangesAsync();
            //    //}

            //    return ResponseModel<JobModel>.Success(_mapper.Map<JobModel>(job));
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogCritical($"Exception occured while updating score card: {ex.Message}", nameof(UpdateAsync));
            //    return ResponseModel<JobModel>.Failure("Exception error");
            //}
        }

        public async Task<ResponseModel<DashboardChartModel>> DashBoardData(DashBoardFilter filter)
        {
            var dashboardDto = new DashboardChartModel();

            IEnumerable<JobApplication> jobApplication = null;
            IEnumerable<Job> Jobs = null;


            var jobApplications = await _dbContext.JobApplications
                 .Include(x => x.Job)
                 .Include(x => x.JobApplicant)
                 .Where(x => x.CompanyId == companyId && x.IsDeleted == false)
                 .ToListAsync();

            var applicanSkillSet = await (from ap in _dbContext.ApplicantProfiles
                                          join r in _dbContext.ApplicantSkills
                                          on ap.Id equals r.ApplicantsId
                                          where ap.IsDeleted == false && ap.CompanyId == companyId
                                          select new { ap, r }).ToListAsync();


            var jobApplicationByJobTitle = jobApplications.GroupBy(x => x.Job.JobTitle).Select(x => new JobApplicationByJobTitleChart { Label = x.Key, Data = x.Count() }).ToList();
            var jobApplicationByAgeRange = jobApplications.GroupBy(x => x.JobApplicant.AgeRange).Select(x => new JobApplicationByAgeRange { Label = x.Key, Data = x.Count() }).ToList();


            dashboardDto.JobApplicationByJobTitles = jobApplicationByJobTitle;

            dashboardDto.JobApplicationByAgeRange = jobApplicationByAgeRange;

            return ResponseModel<DashboardChartModel>.Success(dashboardDto);

        }
        //start from fresh

        public async Task<ResponseModel<List<DashBoardListGraphData>>> OpenJobRolesByDepartment(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
        {
            var departmentlist = await _jobApplicationServices.GrpcDepartmentList();


            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(timePeriod))
            {
                startDate = GetStartDateForTimePeriod(timePeriod);

            }

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                startDate = selectedStartDate.Value;
                endDate = selectedEndDate.Value;
            }

            var jobs = await _dbContext.Jobs
                 .Where(x => x.CompanyId == companyId && x.IsDeleted == false
                 && x.JobStatus == JobStatus.Open && x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                 .ToListAsync();

            var openJobRolesByDepartment = jobs.GroupBy(x => x.DepartmentId)
                                .Select(x => new DashBoardListGraphData
                                {
                                    Label = GetCurrentDepartmentName(departmentlist, x.Key),
                                    Data = x.Count()
                                }).ToList();

            return ResponseModel<List<DashBoardListGraphData>>.Success(openJobRolesByDepartment);

        }

        public async Task<ResponseModel<List<DashBoardListGraphData>>> JobsPerRole(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
        {
            var departmentlist = await _jobApplicationServices.GrpcDepartmentList();

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(timePeriod))
            {
                startDate = GetStartDateForTimePeriod(timePeriod);

            }

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                startDate = selectedStartDate.Value;
                endDate = selectedEndDate.Value;
            }

            var jobs = await _dbContext.Jobs
                 .Where(x => x.CompanyId == companyId && x.IsDeleted == false
                 && x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                 .ToListAsync();

            var jobsPerRole = jobs.GroupBy(x => x.JobTitle)
                                .Select(x => new DashBoardListGraphData
                                {
                                    Label = x.Key,
                                    Data = x.Count()
                                }).ToList();

            return ResponseModel<List<DashBoardListGraphData>>.Success(jobsPerRole);

        }

        public async Task<ResponseModel<List<DashBoardListGraphData>>> ApplicantDistributionByAge(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(timePeriod))
            {
                startDate = GetStartDateForTimePeriod(timePeriod);

            }

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                startDate = selectedStartDate.Value;
                endDate = selectedEndDate.Value;
            }

            var departmentlist = await _jobApplicationServices.GrpcDepartmentList();


            var jobApplicants = await _dbContext.JobApplications
                .Include(x => x.JobApplicant)
                 .Where(x => x.CompanyId == companyId && x.IsDeleted == false
                          && x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                 .ToListAsync();

            var applicantDistributionByAge = jobApplicants.GroupBy(x => x.JobApplicant.AgeRange)
                                .Select(x => new DashBoardListGraphData
                                {
                                    Label = x.Key,
                                    Data = x.Count()
                                }).ToList();

            return ResponseModel<List<DashBoardListGraphData>>.Success(applicantDistributionByAge);

        }

        public async Task<ResponseModel<List<RoleSkill>>> DistributionBySkillSets(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(timePeriod))
            {
                startDate = GetStartDateForTimePeriod(timePeriod);

            }

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                startDate = selectedStartDate.Value;
                endDate = selectedEndDate.Value;
            }


            var result = await (from sc in _dbContext.ApplicantProfiles
                                join soc in _dbContext.ApplicantSkills
                                on sc.Id equals soc.ApplicantsId
                                where sc.CompanyId == companyId && sc.IsDeleted == false
                          && soc.CreatedDate >= startDate && soc.CreatedDate <= endDate && soc.CompanyId == companyId && soc.IsDeleted == false
                                select new { SomeClass = sc, SomeOtherClass = soc }).ToListAsync();


            var skillsByCount = result.Select(ap => ap.SomeOtherClass).GroupBy(skill => skill.SkillName).Distinct()
                                .Select(g => new RoleSkill
                                {
                                    SkillName = g.Key,
                                    SkillCount = g.Count(),
                                    PercentageShare = Math.Round(((double)g.Count() / result.Count()) * 100, 2)
                                }).ToList(); ;



            if (skillsByCount.Count() > 0)
            {
                return ResponseModel<List<RoleSkill>>.Success(skillsByCount);
            }


            return ResponseModel<List<RoleSkill>>.Success(skillsByCount, "No Result found");
        }

        public async Task<ResponseModel<EmployeeTurnOverDashBoardListGraphData>> EmployeeTurnOver(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
        {
            // Get department list from gRPC service
            var departmentlist = await _jobApplicationServices.GrpcDepartmentList();

            // Get all employees for the selected time period
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(timePeriod))
            {
                startDate = GetStartDateForTimePeriod(timePeriod);

            }

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                startDate = selectedStartDate.Value;
                endDate = selectedEndDate.Value;
            }

            DateTime currentDate = DateTime.Now;
            DateTime previousYearDate = currentDate.AddYears(-1);
            var currentYearEmployees = await _dbContext.JobApplications.Include(x => x.Job)
                .Where(e => e.IsHired == true && e.ModifiedDate.Value.Year == currentDate.Year
                && e.DateApplied >= startDate
                && e.DateApplied <= endDate)
                .GroupBy(e => e.Job.DepartmentId)
                .Select(g => new DashBoardListGraphData
                {
                    Label = GetCurrentDepartmentName(departmentlist, g.Key),
                    Data = g.Count()
                }).ToListAsync();

            var previousYearEmployees = await _dbContext.JobApplications.Include(x => x.Job)
                .Where(e => e.IsHired == true && e.ModifiedDate.Value.Year == previousYearDate.Year &&
                e.DateApplied >= startDate
                && e.DateApplied <= endDate)
                .GroupBy(e => e.Job.DepartmentId)
                .Select(g => new DashBoardListGraphData
                {
                    Label = GetCurrentDepartmentName(departmentlist, g.Key),
                    Data = g.Count()
                }).ToListAsync();

            var result = new EmployeeTurnOverDashBoardListGraphData
            {
                CurrentYearData = currentYearEmployees,
                PreviousYearData = previousYearEmployees
            };

            //var employees = await _dbContext.JobApplications
            //    .Where(e => e.CompanyId == companyId && e.IsDeleted == false
            //    && e.DateRecruited.HasValue
            //    && e.DateRecruited.Value >= startDate
            //    && e.DateRecruited.Value <= endDate)
            //    .Include(e => e.Job)
            //    .ToListAsync();

            //// Calculate turnover rate by department
            //var turnoverRateByDepartment = employees.GroupBy(e => e.Job.DepartmentId)
            //    .Select(g => new DashBoardListGraphData
            //    {
            //        Label = GetCurrentDepartmentName(departmentlist, g.Key),
            //        Data = (int)((double)g.Count() / GetTotalEmployeesInDepartment(g.Key))
            //    })
            //    .ToList();

            return ResponseModel<EmployeeTurnOverDashBoardListGraphData>.Success(result);
        }

        public async Task<ResponseModel<List<DashBoardListGraphData>>> SourceOfApplication(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(timePeriod))
            {
                startDate = GetStartDateForTimePeriod(timePeriod);

            }

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                startDate = selectedStartDate.Value;
                endDate = selectedEndDate.Value;
            }

            var applications = await _dbContext.JobApplications
                  .Where(e => e.CompanyId == companyId && e.IsDeleted == false
                && e.DateApplied >= startDate
                && e.DateApplied <= endDate)
                .GroupBy(ja => ja.Channel)
                .Select(g => new DashBoardListGraphData { Label = g.Key, Data = g.Count() })
                .ToListAsync();

            if (applications.Count > 0) { }

            return ResponseModel<List<DashBoardListGraphData>>.Success(applications);

        }

        public async Task<ResponseModel<CustomPagination<List<JobPostReviewDto>>>> JobPostReview(JobPostReviewfilterDto filter)
        {

            var emp = _currentUser.GetUserId().ToString();

            var employeeId = GetEmployeeData(emp).Result;


            if (employeeId == null)
            {
                return ResponseModel<CustomPagination<List<JobPostReviewDto>>>.Failure("No job to review");
            }

            var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

            try
            {

                var result = await _dbContext.JobReviewers
                               .Include(x => x.Job).Where(x => x.IsDeleted == false
                              && x.Status == filter.ReviwerStatus
                              && x.CompanyId == companyId && x.ReviewerId == interviewerEmployeeId)
              .Select(x => new JobPostReviewDto
              {
                  Id = x.Id,
                  JobReviwerId = x.ReviewerId,
                  JobId = x.JobId,
                  JobTitle = x.Job.JobTitle,
                  ReviwerStatus = x.Status,
                  CreatedBy = x.CreatedByName,
                  FormatDatePosted = x.CreatedDate.ToString("dd/MM/yyyy"),
                  ReviwerStatusText = x.Status.GetDescription(),
                  FormatTimePosted = x.CreatedDate.ToString("hh:mm tt"),
                  DatePosted = x.CreatedDate,
                  TimePosted = x.CreatedDate,
                  CreatedOn = x.CreatedDate
              }).ToListAsync();


                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    result = result.Where(x => x.CreatedOn >= filter.StartDate && x.CreatedOn <= filter.EndDate).ToList();
                }


                CustomPagination<List<JobPostReviewDto>> response = new CustomPagination<List<JobPostReviewDto>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count()
                };

                return ResponseModel<CustomPagination<List<JobPostReviewDto>>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobPostReviewDto>>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<string>> JobPostReviewComment(CreateJobReviwerComent request)
        {

            try
            {
                var job = await _dbContext.Jobs.Where(x => x.Id == request.JobId).SingleOrDefaultAsync();

                if (job == null)
                {
                    return ResponseModel<string>.Failure("Job not Found");
                }

                var jobReviwer = await _dbContext.JobReviewers.Where(x => x.Id == request.JobReviwerId).FirstOrDefaultAsync();

                if (jobReviwer == null)
                {
                    return ResponseModel<string>.Failure("Job reviewer not Found");
                }

                jobReviwer.Comment = request.Comment;
                jobReviwer.IsReviewd = true;
                jobReviwer.Status = ReviwerStatus.Reviewed;
                jobReviwer.ModifiedDate = DateTime.Now;
                jobReviwer.ModifiedBy = _currentUser.GetFullname();

                _dbContext.JobReviewers.Update(jobReviwer);
                await _dbContext.SaveChangesAsync();

                return ResponseModel<string>.Success("Successful Updated");
            }
            catch (Exception ex)
            {

                return ResponseModel<string>.Failure("Update Failed");
            }



        }

        public async Task<ResponseModel<List<DashBoardListGraphData>>> RecruitmentOverTime(string? timePeriod = null,
                                DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
        {
            DateTime currentDate = DateTime.Now;
            DateTime startDate = currentDate.AddDays(-3650);

            int daysInRange = (currentDate - startDate).Days;

            // Set date range based on user selection
            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                startDate = selectedStartDate.Value;
                currentDate = selectedEndDate.Value;
            }

            // Determine the time period for the query
            TimeSpan timeSpan;
            switch (timePeriod.ToLower())
            {
                case "past 10 years":
                    timeSpan = TimeSpan.FromDays(3650);
                    break;
                case "past 5 years":
                    timeSpan = TimeSpan.FromDays(1825);
                    break;
                case "past 3 years":
                    timeSpan = TimeSpan.FromDays(1095);
                    break;
                case "this year":
                    timeSpan = TimeSpan.FromDays(365);
                    break;
                default:
                    return ResponseModel<List<DashBoardListGraphData>>.Failure("Invalid time period input");
            }

            // Determine the grouping interval based on the date range
            string groupingInterval = "yearly";
            if (daysInRange <= 365)
            {
                groupingInterval = "monthly";
            }
            if (daysInRange <= 30)
            {
                groupingInterval = "weekly";
            }
            if (daysInRange <= 14)
            {
                groupingInterval = "daily";
            }

            // Query the database and group the results by the specified interval
            var query = await (from r in _dbContext.JobApplications
                               where r.DateRecruited >= startDate && r.DateRecruited <= currentDate
                               group r by new
                               {
                                   Year = "",
                                   Month = "",
                                   Week = "",
                                   Day = ""
                               } into g
                               select new DashBoardListGraphData
                               {
                                   Label = "",
                                   Data = g.Count()
                               }).ToListAsync();

            if (groupingInterval == "yearly")
            {
                query = await (from r in _dbContext.JobApplications
                               where r.DateRecruited >= startDate && r.DateRecruited <= currentDate
                               group r by new
                               {
                                   Year = r.DateRecruited.Value.Year.ToString(),
                                   Month = "",
                                   Week = "",
                                   Day = ""
                               } into g
                               select new DashBoardListGraphData
                               {
                                   Label = g.Key.Year,
                                   Data = g.Count()
                               }).ToListAsync();
            }
            else if (groupingInterval == "monthly")
            {
                query = await (from r in _dbContext.JobApplications
                               where r.DateRecruited >= startDate && r.DateRecruited <= currentDate
                               group r by new
                               {
                                   Year = r.DateRecruited.Value.Year.ToString(),
                                   Month = r.DateRecruited.Value.Month.ToString("00"),
                                   Week = "",
                                   Day = ""
                               } into g
                               select new DashBoardListGraphData
                               {
                                   Label = $"{g.Key.Year}/{g.Key.Month}",
                                   Data = g.Count()
                               }).ToListAsync();
            }
            else if (groupingInterval == "weekly")
            {
                query = await (from r in _dbContext.JobApplications
                               where r.DateRecruited >= startDate && r.DateRecruited <= currentDate
                               group r by new
                               {
                                   Year = r.DateRecruited.Value.Year.ToString(),
                                   Month = r.DateRecruited.Value.Month.ToString("00"),
                                   Week = $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(r.DateRecruited.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday):00}",
                                   Day = ""
                               } into g
                               select new DashBoardListGraphData
                               {
                                   Label = $"{g.Key.Year}/{g.Key.Month}/{g.Key.Week}",
                                   Data = g.Count()
                               }).ToListAsync();
            }
            else
            {
                query = await (from r in _dbContext.JobApplications
                               where r.DateRecruited >= startDate && r.DateRecruited <= currentDate
                               group r by new
                               {
                                   Year = r.DateRecruited.Value.Year.ToString(),
                                   Month = r.DateRecruited.Value.Month.ToString("00"),
                                   Week = $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(r.DateRecruited.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday):00}",
                                   Day = r.DateRecruited.Value.ToString("dd")
                               } into g
                               select new DashBoardListGraphData
                               {
                                   Label = $"{g.Key.Year}/{g.Key.Month}/{g.Key.Day}",
                                   Data = g.Count()
                               }).ToListAsync();
            }

            if (query != null)
            {
                return ResponseModel<List<DashBoardListGraphData>>.Success(query);
            }
            else
            {
                return ResponseModel<List<DashBoardListGraphData>>.Success(query, "No result Found");
            }

        }

        private DateTime GetStartDateForTimePeriod(string timePeriod)
        {
            switch (timePeriod)
            {
                case "Last 6 Months":
                    return DateTime.Now.AddMonths(-6);
                case "Last 3 Months":
                    return DateTime.Now.AddMonths(-3);
                case "Last Month":
                    return DateTime.Now.AddMonths(-1);
                case "This Year":
                default:
                    return new DateTime(DateTime.Now.Year, 1, 1);
            }
        }

        private int GetTotalEmployeesInDepartment(Guid departmentId)
        {
            return _dbContext.JobApplications.Include(x => x.Job)
                .Count(e => e.Job.DepartmentId == departmentId && e.IsDeleted == false);
        }

        public static string GetCurrentDepartmentName(List<DepartmentDataFromGRPC> departments, Guid departmentId)
        {
            var result = departments.Where(x => x.Id == departmentId).FirstOrDefault();
            return result != null ? result.Name : "";
        }


        //private async Task<EmployeeDetails?> GetEmployeeData(string userId)
        //{
        //    #region employee profile picture via grpc

        //    try
        //    {
        //        var employee = new EmployeeDetails();

        //        var channel = GrpcChannel.ForAddress("https://hrempgrpc.azurewebsites.net/");
        //        var client = new EmployeeGrpcService.EmployeeGrpcServiceClient(channel);



        //        var reply = await client.GetEmployeeDetailsByUserAsync(
        //            new EmployeeDetailsRequestByUserId()
        //            {
        //                UserId = userId
        //            });

        //        if (reply == null)
        //        {
        //            return employee;
        //        }
        //        else
        //        {
        //            employee.FullName = reply.FullName;
        //            employee.Phonenumber = reply.Phonenumber;
        //            employee.Email = reply.Email;
        //            employee.ProfileImage = reply.ProfileImage;
        //            employee.EmployeeId = reply.EmployeeId;

        //            return employee;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        _logger.LogCritical($"Exception occured while getting employee Record: {ex.Message}", nameof(GetEmployeeData));
        //        return null;
        //    }


        //    #endregion
        //}

    }
}