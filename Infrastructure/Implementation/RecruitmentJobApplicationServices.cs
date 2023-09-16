using Core.Common.Model;
using Core.Common.Model.RecruitmentDto;
using Core.Common.Settings;
using Core.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Grpc.Net.Client;
using HRShared.Common;
using HRShared.CoreProviders.Interfaces;
using HRShared.Helpers;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Context;
using Infrastructure.Providers.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using static Domain.Enums.Enum;

namespace Infrastructure.Implementation
{
    public class RecruitmentJobApplicationServices : IRecruitmentJobApplicationServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IAsyncRepository<JobApplication, Guid> _jobApplicationRepository;
        private readonly IAsyncRepository<JobScheduleInterview, Guid> _jobScheduleInterviewRepository;
        private readonly ILogger<RecruitmentJobApplicationServices> _logger;
        private readonly IMailService _mailService;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IAsyncRepository<JobInterviewHistory, Guid> _jobInterviewHistoryRepository;
        private readonly IAsyncRepository<JobApplicationStageHistory, Guid> _jobApplicationStageHistories;
        private readonly IAzureStorageServices _azureStorageService;
        private readonly QuizSettings _quizSettings;
        private readonly Guid companyId;
        private readonly string _connString;

        public RecruitmentJobApplicationServices(ApplicationDbContext dbContext, ICurrentUser currentUser,
                                                    IAsyncRepository<JobApplication, Guid> JobApplicationRepository,
                                                    IAsyncRepository<JobScheduleInterview, Guid> JobScheduleInterviewRepository,
                                                    ILogger<RecruitmentJobApplicationServices> logger,
                                                    IMailService mailService, IOptions<EmailSettings> emailSettings,
                                                    IAsyncRepository<JobInterviewHistory, Guid> jobInterviewHistoryRepository,
                                                    IAsyncRepository<JobApplicationStageHistory, Guid> jobApplicationStageHistories,
                                                    IOptions<QuizSettings> quizSettings,
                                                    IAzureStorageServices azureStorageService)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _jobApplicationRepository = JobApplicationRepository;
            _jobScheduleInterviewRepository = JobScheduleInterviewRepository;
            _logger = logger;
            _mailService = mailService;
            _emailSettings = emailSettings;
            _jobInterviewHistoryRepository = jobInterviewHistoryRepository;
            _jobApplicationStageHistories = jobApplicationStageHistories;
            _azureStorageService = azureStorageService;
            _quizSettings = quizSettings.Value;
            companyId = Guid.Parse(_currentUser.GetCompany());
        }
        public async Task<ResponseModel<EvaluationRecruitmentScoreCardDTO>> getApplicantScoreCard(SaveApplicantScoreCardEvaluationDto payload)
        {
            var result = new EvaluationRecruitmentScoreCardDTO();
            try
            {
                var allInterviwerScore = new AggregateScore();

                if (payload.InterviewerEmployeeId == Guid.Empty)
                {
                    var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());


                    if (employeeId == null)
                    {
                        return ResponseModel<EvaluationRecruitmentScoreCardDTO>.Failure("Hr/Admin not found");
                    }

                    payload.InterviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);
                }


                allInterviwerScore = await _dbContext.AggregateScores.Include(x => x.QuestionsWithMarks)
                    .Where(x => x.JobApplicantionId == payload.JobApplicantionId && x.JobApplicantId == payload.JobApplicantId
                    && x.HireStageId == payload.HireStageId && x.SubHireStageId == payload.SubHireStageId
                    && x.InterviewerEmployeeId == payload.InterviewerEmployeeId && x.CompanyId == companyId && x.IsDeleted == false)
                    .FirstOrDefaultAsync();


                if (allInterviwerScore == null)
                {
                    return ResponseModel<EvaluationRecruitmentScoreCardDTO>.Failure("Score Card Score Not Available");
                }

                var stage = await _dbContext.SubStages.Where(x => x.Id == payload.SubHireStageId && x.StageId == payload.HireStageId).FirstOrDefaultAsync();

                if (stage == null)
                {
                    return ResponseModel<EvaluationRecruitmentScoreCardDTO>.Failure("Sub stage ScorecardId Not Found");
                }

                result = await (from p in _dbContext.ScoreCards
                                where p.Id == stage.ScoreCardId && p.CompanyId == companyId && p.IsDeleted == false
                                select new EvaluationRecruitmentScoreCardDTO
                                {
                                    Id = p.Id,
                                    name = p.ScoreCardName,
                                    CompanyId = p.CompanyId,
                                    IsDeleted = p.IsDeleted,
                                    DateCreated = p.CreatedDate,
                                    CreatedById = p.CreatedBy,
                                    ModifiedById = p.ModifiedBy,
                                    DateModified = p.ModifiedDate,

                                }).FirstOrDefaultAsync();

                if (result != null)
                {
                    result.FocusAreas = new List<EvaluationRecruitmentFocusAreaDTO>();
                    var scoreCardFocusArea = _dbContext.RecruitmentFocusAreas.Where(f => f.ScoreCardId == result.Id && f.IsDeleted == false).AsQueryable().ToList();
                    if (scoreCardFocusArea.Count > 0)
                    {
                        scoreCardFocusArea.ForEach(f =>
                        {
                            var singleScoreCard = new EvaluationRecruitmentFocusAreaDTO();
                            singleScoreCard.ScoreCardQuestions = new List<EvaluationScoreCardQuestionDTO>();
                            singleScoreCard.Id = f.Id;
                            singleScoreCard.RecruitmentScoreCardId = f.ScoreCardId;
                            singleScoreCard.FocusArea = f.FocusArea;
                            singleScoreCard.TotalWeight = f.TotalWeight;
                            var focusAreaScoreQuestion = _dbContext.ScoreCardQuestions.Where(s => s.RecruitmentFocusAreaId == singleScoreCard.Id && s.IsDeleted == false).ToList();
                            if (focusAreaScoreQuestion.Count > 0)
                            {
                                focusAreaScoreQuestion.ForEach(x =>
                                {
                                    var singleScoreCardQuest = new EvaluationScoreCardQuestionDTO();
                                    var questionwithMarks = _dbContext.QuestionsWithMarks.FirstOrDefault(q => q.AggregateScoresId == allInterviwerScore.Id && q.QuestionId == x.Id);
                                    singleScoreCardQuest.Id = x.Id;
                                    singleScoreCardQuest.Question = x.Question;
                                    singleScoreCardQuest.Weight = x.Weight;
                                    singleScoreCardQuest.RecruitmentFocusAreaId = x.RecruitmentFocusAreaId;
                                    singleScoreCardQuest.Score = questionwithMarks.Score;
                                    singleScoreCardQuest.Comment = questionwithMarks.Comment;
                                    singleScoreCard.ScoreCardQuestions.Add(singleScoreCardQuest);
                                });
                            };
                            result.FocusAreas.Add(singleScoreCard);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<EvaluationRecruitmentScoreCardDTO>.Exception(ex.Message);
            }

            return ResponseModel<EvaluationRecruitmentScoreCardDTO>.Success(result);
        }
        public async Task<ResponseModel<string>> SaveApplicantScoreCard(SaveApplicantScoreCardDto payload)
        {
            try
            {

                var jobApplication = await _dbContext.JobApplications.Where(p => p.Id == payload.JobApplicantionId).FirstOrDefaultAsync();

                if (jobApplication != null)
                {
                    var jobApplicant = await _dbContext.ApplicantProfiles.Where(p => p.Id == payload.JobApplicantId).FirstOrDefaultAsync();
                    if (jobApplicant != null)
                    {
                        var scheduleInterviewer = await _dbContext.JobScheduleInterviews.Where(p => p.InterviewerEmployeeId == payload.InterviewerEmployeeId
                                            && p.JobApplicantionId == jobApplication.Id).FirstOrDefaultAsync();


                        var aggregateScoreExist = await _dbContext.AggregateScores.Where(x => x.JobApplicantionId == payload.JobApplicantionId && x.JobApplicantId == payload.JobApplicantId && x.HireStageId == payload.HireStageId && x.SubHireStageId == payload.SubHireStageId && x.InterviewerEmployeeId == payload.InterviewerEmployeeId).FirstOrDefaultAsync();

                        if (aggregateScoreExist != null)
                        {
                            _dbContext.AggregateScores.Remove(aggregateScoreExist);
                            await _dbContext.SaveChangesAsync();
                        }
                        //var interviewer = await _context.Employees.Where(p => p.UserId == usr.user_id).FirstOrDefaultAsync();
                        if (scheduleInterviewer != null)
                        {
                            var lstScore = new List<long>();
                            foreach (var item in payload.QuestionsWithMark)
                            {
                                lstScore.Add(item.Score);
                            }
                            var scoreWeight = lstScore.Sum();


                            var stage = await _dbContext.SubStages
                                .Where(x => x.Id == payload.SubHireStageId && x.StageId == payload.HireStageId && x.IsDeleted == false)
                                .FirstOrDefaultAsync();

                            if (stage == null)
                            {
                                return ResponseModel<string>.Failure("Sub stage ScorecardId Not Found");
                            }

                            var scorecard = await _dbContext.ScoreCards.FirstOrDefaultAsync(x => x.Id == stage.ScoreCardId && x.IsDeleted == false);

                            if (scorecard == null)
                            {
                                return ResponseModel<string>.Failure("Scorecard Not Found");
                            }


                            int numberOfFocusArea = 0;


                            var scoreCardFocusArea = await _dbContext.RecruitmentFocusAreas
                                .Where(s => s.IsDeleted == false
                            && s.CompanyId == companyId && s.ScoreCardId == stage.ScoreCardId).ToListAsync();

                            //Console.WriteLine(scoreCardFocusArea);

                            if (scoreCardFocusArea != null)
                            {
                                numberOfFocusArea = scoreCardFocusArea.Count();
                            }
                            else
                            {
                                return ResponseModel<string>.Success("Focus Area is Zero");
                            }

                            double overAllscoreWeight = Convert.ToDouble(scoreWeight) / Convert.ToDouble(numberOfFocusArea);

                            var aggregateScore = new AggregateScore()
                            {
                                JobApplicantId = payload.JobApplicantId,
                                JobApplicantionId = payload.JobApplicantionId,
                                InterviewerEmployeeId = payload.InterviewerEmployeeId,
                                HireStageId = payload.HireStageId,
                                SubHireStageId = payload.SubHireStageId,
                                ScoreCardId = scorecard.Id,
                                WeightedScore = overAllscoreWeight,
                                CompanyId = companyId
                            };

                            _dbContext.AggregateScores.Add(aggregateScore);
                            await _dbContext.SaveChangesAsync();

                            foreach (var item in payload.QuestionsWithMark)
                            {
                                var questionWithMarks = new QuestionsWithMark()
                                {
                                    AggregateScoresId = aggregateScore.Id,
                                    QuestionId = item.QuestionId,
                                    Score = item.Score,
                                    ScoreCardId = scorecard.Id,
                                    Comment = item.Comment,
                                    FocusAreaId = item.FocusAreaId,

                                };
                                _dbContext.QuestionsWithMarks.Add(questionWithMarks);
                            }

                            await _dbContext.SaveChangesAsync();



                            decimal overAllAverageScore = 0;
                            double overAllTotal = 0;
                            int totalSubhiringStages = 0;

                            var allInterviwerScore = await _dbContext.AggregateScores
                                .Where(x => x.JobApplicantionId == payload.JobApplicantionId && x.JobApplicantId == payload.JobApplicantId
                                && x.HireStageId == payload.HireStageId && x.IsDeleted == false).ToListAsync();


                            if (allInterviwerScore != null)
                            {
                                var groupBySubhiringStage = allInterviwerScore.GroupBy(d => d.SubHireStageId).Select(
                                                                   g => new
                                                                   {
                                                                       Key = g.Key,
                                                                       Value = g.Sum(s => s.WeightedScore) / g.Count(),
                                                                   }).ToList();


                                overAllTotal = groupBySubhiringStage.Sum(x => x.Value);


                                var hiringStage = await _dbContext.SubStages.Where(x => x.CompanyId == companyId && x.StageId == stage.StageId && x.IsDeleted == false && x.ScoreCardId.HasValue == true).CountAsync();

                                if (hiringStage > 0)
                                {
                                    totalSubhiringStages = hiringStage;
                                }


                                overAllAverageScore = Convert.ToDecimal(overAllTotal) / totalSubhiringStages;
                            }



                            jobApplication.ScoreCardValue = overAllAverageScore;

                            _dbContext.JobApplications.Update(jobApplication);
                            await _dbContext.SaveChangesAsync();

                            scheduleInterviewer.Status = InterViewStatus.Closed;
                            _dbContext.JobScheduleInterviews.Update(scheduleInterviewer);
                            await _dbContext.SaveChangesAsync();
                            //overAllAverageScore
                        }
                        else
                        {
                            _logger.LogInformation("Interviewer Not Found");
                            throw new Exception("Interviewer Not Found");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Job Applicant Not Found");
                        throw new Exception("Job Applicant Not Found");
                    }


                    return ResponseModel<string>.Success("Your record was updated successfully");
                }
                else
                {
                    _logger.LogInformation("Job Application Not Found");
                    throw new Exception("Job Application Not Found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<string>.Failure("Job Application Not Found");
            }
        }
        public async Task<ResponseModel<string>> AssignStatustoJobApplication(AssingStatusToJobApplicationDto request)
        {

            try
            {
                string newSubName = "";

                var jobApplication = await _dbContext.JobApplications.Include(x => x.JobApplicant)
                    .Include(x => x.Job)
                    .Include(x => x.Stage)
                    .Where(x => x.Id == request.ApplicationId && x.IsDeleted == false &&
                    x.CompanyId == companyId).FirstOrDefaultAsync();



                if (jobApplication == null)
                {
                    return ResponseModel<string>.Failure("Not found");
                }

                if (request.HireStageId == Guid.Empty)
                {

                    return ResponseModel<string>.Failure("Invalid Hire Stage Id");
                }
                var hiringStage = await _dbContext.Stages.Include(x => x.SubStages)
                                         .Where(x => x.Id == request.HireStageId && x.IsDeleted == false && x.CompanyId == companyId)
                                         .FirstOrDefaultAsync();

                if (hiringStage == null)
                {
                    return ResponseModel<string>.Failure("Stage not found.");
                }


                if (jobApplication.Stage.StageName.Equals("Pending Offer")
                    && (hiringStage.StageName.Contains("Applied") || hiringStage.StageName.Contains("Shortlisted")
                    || hiringStage.StageName.Contains("Interview") || hiringStage.StageName.Contains("Rejected")))
                {
                    return ResponseModel<string>.Failure("You can only hire");
                }

                if (jobApplication.Stage.StageName.Contains("Hired")
                   && (hiringStage.StageName.Contains("Applied") || hiringStage.StageName.Contains("Shortlisted")
                   || hiringStage.StageName.Contains("Interview") || hiringStage.StageName.Contains("Rejected")
                   || hiringStage.StageName.Equals("Pending Offer")))
                {
                    return ResponseModel<string>.Failure("Applicant has been hired");
                }


                if (string.IsNullOrEmpty(hiringStage.StageName))
                {
                    return ResponseModel<string>.Failure("unable to fetch Hireing Stage");
                }

                if (hiringStage.StageName.Contains("Applied"))
                {
                    return ResponseModel<string>.Failure("Already Applied Application");
                }

                if (hiringStage.StageName.Contains("Shortlisted"))
                {
                    var histdedupChk = CheckIfApplicatJobApplicationStatusExist(request.ApplicationId, hiringStage.Id);
                    if (histdedupChk)
                    {
                        return ResponseModel<string>.Failure("Application Has Alredy Been Shotlisted");
                    }

                    jobApplication.HireStageId = hiringStage.Id;
                    jobApplication.IsInProgress = false;
                    jobApplication.IsHired = false;
                    jobApplication.ApplicationStatus = hiringStage.StageName;
                    jobApplication.ModifiedDate = DateTime.Now;
                    jobApplication.ModifiedBy = _currentUser.GetUserEmail();
                    // jobApplication.Stage = hiringStage;

                    _jobApplicationRepository.Update(jobApplication);
                    //await _jobApplicationRepository.SaveChangesAsync();
                    //send Quiz Notification

                    var jobApplicationStageHistory = new JobApplicationStageHistory
                    {

                        Id = SequentialGuid.Create(),
                        IsDeleted = false,
                        JobApplicationId = jobApplication.Id,
                        StatusDescription = $"{hiringStage.StageName}",
                        StageId = hiringStage.Id,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        JobApplication = jobApplication,
                        Stage = hiringStage,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);
                    // await _dbContext.SaveChangesAsync();

                    if (hiringStage.EmailTemplateId != null)
                    {

                        EmailTemplateProcess emailTemplateProcess = new()
                        {
                            Id = SequentialGuid.Create(),
                            Description = "Recruitment Action",
                            Email = jobApplication.JobApplicant.Email.Trim(),
                            UserId = jobApplication.JobApplicant.UserId,
                            TemplateId = (Guid)hiringStage.EmailTemplateId,
                            Processed = false,
                            EmailSent = false,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByName = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now
                        };

                        await _dbContext.EmailTemplateProcesses.AddAsync(emailTemplateProcess);
                        //  await _dbContext.SaveChangesAsync();
                    }

                    if (jobApplication.Job != null)
                    {

                        var reviewers = jobApplication.Job.JobReviewers != null ? jobApplication.Job.JobReviewers : null;

                        if (reviewers != null)
                        {
                            foreach (var item in reviewers)
                            {
                                await CreateUpcomingTask($"Reviewer Shortlist for {jobApplication.Job.JobTitle}", DateTime.Now, item.ReviewerId);
                            }
                        }

                    }

                    var quizEmail = await SendQuizEmail(jobApplication);
                    await _dbContext.SaveChangesAsync();

                    return ResponseModel<string>.Success("Your record was updated successfully");

                }

                if (hiringStage.StageName.Contains("Rejected"))
                {
                    var histdedupChk = CheckIfApplicatJobApplicationStatusExist(request.ApplicationId, hiringStage.Id);
                    if (histdedupChk)
                    {
                        return ResponseModel<string>.Failure("Application Has Been Rejected");
                    }
                    jobApplication.HireStageId = hiringStage.Id;
                    jobApplication.IsInProgress = false;
                    jobApplication.IsHired = false;
                    jobApplication.ApplicationStatus = hiringStage.StageName;
                    jobApplication.ModifiedDate = DateTime.Now;
                    jobApplication.ModifiedBy = _currentUser.GetUserEmail();
                    // jobApplication.Stage = hiringStage;

                    _jobApplicationRepository.Update(jobApplication);
                    // await _jobApplicationRepository.SaveChangesAsync();
                    //send Quiz Notification


                    var jobApplicationStageHistory = new JobApplicationStageHistory()
                    {

                        Id = SequentialGuid.Create(),
                        IsDeleted = false,
                        JobApplicationId = jobApplication.Id,
                        StatusDescription = $"{hiringStage.StageName}",
                        StageId = hiringStage.Id,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        JobApplication = jobApplication,
                        Stage = hiringStage,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);
                    // await _dbContext.SaveChangesAsync();

                    if (hiringStage.EmailTemplateId != null)
                    {
                        EmailTemplateProcess emailTemplateProcess = new()
                        {
                            Id = SequentialGuid.Create(),
                            Description = "Recruitment Action",
                            Email = jobApplication.JobApplicant.Email.Trim(),
                            UserId = jobApplication.JobApplicant.UserId,
                            TemplateId = (Guid)hiringStage.EmailTemplateId,
                            Processed = false,
                            EmailSent = false,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByName = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now
                        };

                        await _dbContext.EmailTemplateProcesses.AddAsync(emailTemplateProcess);
                        //await _dbContext.SaveChangesAsync();
                    }

                    await _dbContext.SaveChangesAsync();
                    return ResponseModel<string>.Success("Your record was updated successfully");

                }

                if (hiringStage.StageName.Contains("Pending Offer"))
                {
                    var histdedupChk = CheckIfApplicatJobApplicationStatusExist(request.ApplicationId, hiringStage.Id);
                    if (histdedupChk)
                    {
                        return ResponseModel<string>.Failure("Application Has Alredy Been on Pending Offer");
                    }
                    jobApplication.HireStageId = hiringStage.Id;
                    jobApplication.IsInProgress = true;
                    jobApplication.IsHired = false;
                    jobApplication.ApplicationStatus = hiringStage.StageName;
                    //jobApplication.Stage = hiringStage;
                    jobApplication.ModifiedDate = DateTime.Now;
                    jobApplication.ModifiedBy = _currentUser.GetUserEmail();

                    _jobApplicationRepository.Update(jobApplication);
                    // await _jobApplicationRepository.SaveChangesAsync();

                    //send Quiz Notification

                    var jobApplicationStageHistory = new JobApplicationStageHistory()
                    {

                        Id = SequentialGuid.Create(),
                        IsDeleted = false,
                        JobApplicationId = jobApplication.Id,
                        StatusDescription = $"{hiringStage.StageName}",
                        StageId = hiringStage.Id,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        JobApplication = jobApplication,
                        Stage = hiringStage,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);
                    //await _dbContext.SaveChangesAsync();

                    if (hiringStage.EmailTemplateId != null)
                    {
                        EmailTemplateProcess emailTemplateProcess = new()
                        {
                            Id = SequentialGuid.Create(),
                            Description = "Recruitment Action",
                            Email = jobApplication.JobApplicant.Email.Trim(),
                            UserId = jobApplication.JobApplicant.UserId,
                            TemplateId = (Guid)hiringStage.EmailTemplateId,
                            Processed = false,
                            EmailSent = false,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByName = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now
                        };

                        await _dbContext.EmailTemplateProcesses.AddAsync(emailTemplateProcess);
                    }

                    var sendGrpc = await ApplicantOnboarding(jobApplication.Id);
                    if (sendGrpc != null)
                    {
                        if (sendGrpc.IsSuccessful == "true")
                        {
                            RecruitmentApplicantOnboarding recruitmentApplicant = new RecruitmentApplicantOnboarding
                            {
                                Id = SequentialGuid.Create(),
                                JobApplicationId = jobApplication.Id,
                                OnboardingId = sendGrpc.Id,
                                CreatedBy = _currentUser.GetUserId(),
                                CreatedByName = _currentUser.GetFullname(),
                                CreatedDate = DateTime.Now,
                                CompanyId = companyId,
                                IsOfferSent = sendGrpc.IsOfferSent
                            };

                            await _dbContext.RecruitmentApplicantOnboardings.AddAsync(recruitmentApplicant);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    return ResponseModel<string>.Success("Your record was updated successfully");

                }

                if (hiringStage.StageName.Contains("Hired"))
                {

                    var histdedupChk = CheckIfApplicatJobApplicationStatusExist(request.ApplicationId, hiringStage.Id);
                    if (histdedupChk)
                    {
                        return ResponseModel<string>.Failure("Application Has Alredy Been Hired");
                    }
                    jobApplication.HireStageId = hiringStage.Id;
                    jobApplication.IsHired = true;
                    jobApplication.IsInProgress = false;
                    jobApplication.ApplicationStatus = hiringStage.StageName;
                    jobApplication.ModifiedDate = DateTime.Now;
                    jobApplication.ModifiedBy = _currentUser.GetUserEmail();
                    //jobApplication.Stage = hiringStage;
                    jobApplication.DateRecruited = DateTime.Now;


                    _jobApplicationRepository.Update(jobApplication);
                    // await _jobApplicationRepository.SaveChangesAsync();

                    var jobApplicationStageHistory = new JobApplicationStageHistory()
                    {

                        Id = SequentialGuid.Create(),
                        IsDeleted = false,
                        JobApplicationId = jobApplication.Id,
                        StatusDescription = $"{hiringStage.StageName}",
                        StageId = hiringStage.Id,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        JobApplication = jobApplication,
                        Stage = hiringStage,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);
                    // _dbContext.SaveChanges();


                    if (hiringStage.EmailTemplateId != null)
                    {
                        EmailTemplateProcess emailTemplateProcess = new()
                        {
                            Id = SequentialGuid.Create(),
                            Description = "Recruitment Action",
                            Email = jobApplication.JobApplicant.Email.Trim(),
                            UserId = jobApplication.JobApplicant.UserId,
                            TemplateId = (Guid)hiringStage.EmailTemplateId,
                            Processed = false,
                            EmailSent = false,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByName = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now
                        };

                        await _dbContext.EmailTemplateProcesses.AddAsync(emailTemplateProcess);
                        // await _dbContext.SaveChangesAsync();

                    }
                    await _dbContext.SaveChangesAsync();
                    return ResponseModel<string>.Success("Your record was updated successfully");

                }

                if (hiringStage.StageName.Contains("Interview"))
                {

                    var subStage = hiringStage?.SubStages?.Where(x => x.Id == request.HireSubStageId).FirstOrDefault();

                    if (subStage == null)
                    {
                        return ResponseModel<string>.Failure("Sub Hiring Stage not Found");
                    }

                    newSubName = subStage.SubStageName;

                    var histdedupChk = CheckIfApplicatJobApplicationSubStatusExist(request.ApplicationId, hiringStage.Id, subStage.Id);
                    if (histdedupChk)
                    {
                        return ResponseModel<string>.Failure("Application Has Alredy Been Interviewed");
                    }

                    var jobApplicationStageHistory = new JobApplicationStageHistory()
                    {

                        Id = SequentialGuid.Create(),
                        IsDeleted = false,
                        JobApplicationId = jobApplication.Id,
                        StageId = hiringStage.Id,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        JobApplication = jobApplication,
                        Stage = hiringStage,
                        CreatedDate = DateTime.Now,
                        StatusDescription = $"{hiringStage.StageName}",
                        SubStageId = subStage.Id
                    };

                    jobApplicationStageHistory.StatusDescription = $"Scheduled for {newSubName}";
                    _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);
                    // await _dbContext.SaveChangesAsync();


                    //var jApplication = await _dbContext.JobApplications.Where(x => x.Id == request.ApplicationId
                    //&& x.IsDeleted == false && x.CompanyId == companyId).FirstOrDefaultAsync();

                    jobApplication.IsHired = false;
                    jobApplication.IsInProgress = true;
                    jobApplication.ApplicationStatus = subStage.SubStageName;
                    //jobApplication.Stage = hiringStage;
                    jobApplication.ModifiedDate = DateTime.Now;
                    jobApplication.ModifiedBy = _currentUser.GetUserEmail();
                    jobApplication.SubhireStageId = subStage.Id;
                    jobApplication.HireStageId = hiringStage.Id;


                    _jobApplicationRepository.Update(jobApplication);
                    // await _jobApplicationRepository.SaveChangesAsync();


                    if (subStage.EmailTemplateId != null)
                    {
                        var emailTemplateProcess = new EmailTemplateProcess
                        {
                            Id = SequentialGuid.Create(),
                            Description = "Assign Status to JobApplication",
                            Email = jobApplication.JobApplicant.Email.Trim(),
                            UserId = jobApplication.JobApplicant.UserId,
                            TemplateId = (Guid)subStage.EmailTemplateId,
                            Processed = false,
                            EmailSent = false,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByName = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now,

                        };

                        await _dbContext.EmailTemplateProcesses.AddAsync(emailTemplateProcess);
                        // await _dbContext.SaveChangesAsync();
                    }

                    if (jobApplication.Job != null)
                    {

                        var reviewers = jobApplication.Job.JobReviewers != null ? jobApplication.Job.JobReviewers : null;

                        if (reviewers != null)
                        {
                            foreach (var item in reviewers)
                            {
                                await CreateUpcomingTask($"Fill in score card for {jobApplication.Job.JobTitle}", DateTime.Now, item.ReviewerId);
                            }
                        }

                    }
                    await _dbContext.SaveChangesAsync();
                    return ResponseModel<string>.Success("Your record was updated successfully");
                }

                return ResponseModel<string>.Failure("Hiring Stage not Found");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResponseModel<string>.Failure("Not Found.");
            }
        }

        public async Task<ResponseModel<string>> RemoveApplicantFromShortList(RemoveShortlistedJobApplicationDto request)
        {
            try
            {
                string newSubName = "";

                var jobApplication = await _dbContext.JobApplications.Include(x => x.JobApplicant)
                    .Include(x => x.Job)
                    .Include(x => x.Stage)
                    .Where(x => x.Id == request.ApplicationId && x.IsDeleted == false &&
                    x.CompanyId == companyId).FirstOrDefaultAsync();

                if (jobApplication != null)
                {

                    if (jobApplication.ApplicationStatus.Contains("Shortlisted"))
                    {

                        var hiringStageHistrory = await _dbContext.JobApplicationStageHistories.Where(x => x.JobApplicationId == jobApplication.Id && x.CompanyId == companyId).ToListAsync();

                        if (hiringStageHistrory != null)
                        {
                            _jobApplicationStageHistories.DeleteList(hiringStageHistrory);
                            await _jobApplicationStageHistories.SaveChangesAsync();
                        }

                        var appllied = await _dbContext.Stages.FirstOrDefaultAsync(x => x.StageName.Contains("Applied") && x.CompanyId == companyId);

                        if (appllied != null)
                        {
                            jobApplication.IsInProgress = false;
                            jobApplication.IsHired = false;
                            jobApplication.ApplicationStatus = appllied.StageName;
                            jobApplication.ModifiedDate = DateTime.Now;
                            jobApplication.ModifiedBy = _currentUser.GetUserEmail();
                            jobApplication.Stage = appllied;
                            jobApplication.HireStageId = appllied.Id;
                            jobApplication.SubhireStageId = null;


                            _dbContext.JobApplications.Update(jobApplication);
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            return ResponseModel<string>.Failure("Applied Stage not found.");
                        }


                        var jobApplicationStageHistory = new JobApplicationStageHistory()
                        {

                            Id = SequentialGuid.Create(),
                            IsDeleted = false,
                            JobApplicationId = jobApplication.Id,
                            StatusDescription = $"{appllied.StageName}",
                            StageId = appllied.Id,
                            CompanyId = companyId,
                            CreatedBy = _currentUser.GetUserId(),
                            JobApplication = jobApplication,
                            Stage = appllied,
                            CreatedDate = DateTime.Now,
                            CreatedByName = _currentUser.GetFullname()
                        };

                        _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);
                        await _dbContext.SaveChangesAsync();

                        //if (appllied.EmailTemplateId != null)
                        //{
                        //    var meesage = SendMessageByTemplate(appllied.EmailTemplateId.ToString(),
                        //        jobApplication.JobApplicant.UserId.ToString(),
                        //        jobApplication.JobApplicant.Email.Trim()).Result;
                        //}

                        return ResponseModel<string>.Success("Application Successful Removed from Shortlisted");

                    }
                    else
                    {
                        return ResponseModel<string>.Failure("Job Application status is not Shortlisted");
                    }


                }
                else
                {
                    return ResponseModel<string>.Failure("Job Application Not Found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResponseModel<string>.Failure("Not Found.");
            }
        }
        public async Task<ResponseModel<string>> ApplyForJob(ApplyForJobDto payload)
        {
            try
            {
                var inProgress = false;
                var job = await _dbContext.Jobs.Where(j => j.Id == payload.JobId && j.CompanyId == companyId).FirstOrDefaultAsync();

                if (job == null)
                {
                    throw new Exception("Job not found");
                }

                var prevApplication = await _dbContext.JobApplications.Where(jp => jp.JobId == job.Id && jp.JobApplicantId == payload.ApplicantId && jp.CompanyId == companyId).FirstOrDefaultAsync();

                if (prevApplication != null)
                {
                    return ResponseModel<string>.Failure("You have already applied for this job");
                }

                var hireStageId = await _dbContext.Stages.Where(h => h.StageName.Contains("Applied") && h.CompanyId == companyId).FirstOrDefaultAsync();

                if (job == null)
                {
                    return ResponseModel<string>.Failure("Job not found");
                }

                var jobApplication = await _dbContext.JobApplications.Include(x => x.JobApplicant)
                                               .Where(p => p.JobApplicantId == payload.ApplicantId
                                        && p.JobId == payload.JobId && p.IsInProgress == true).FirstOrDefaultAsync();
                if (jobApplication == null)
                {
                    var InprogresChk = await _dbContext.JobApplications.Where(p => p.JobApplicantId == payload.ApplicantId
                    && p.IsInProgress == true && p.CompanyId == companyId).FirstOrDefaultAsync();
                    if (InprogresChk == null)
                    {
                        inProgress = true;
                    }

                    var imageUrl = await _azureStorageService.UploadToAzureAsync(payload.ApplicantCV);
                    if (string.IsNullOrWhiteSpace(imageUrl))
                    {
                        return ResponseModel<string>.Failure("Fail to upload image please try again");
                    }


                    var applicantCode = _dbContext.ApplicantProfiles.Where(c => c.Id == payload.ApplicantId && c.CompanyId == companyId).FirstOrDefault();


                    jobApplication = new JobApplication()
                    {
                        Id = SequentialGuid.Create(),
                        JobId = payload.JobId,
                        JobApplicantId = payload.ApplicantId,//_currentUser.GetUserId(),                   
                        AlertSent = false,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        CreatedDate = DateTime.Now,
                        DateApplied = DateTime.Now,
                        IsDeleted = false,
                        ApplicationStatus = "Applied",
                        HireStageId = hireStageId.Id,
                        IsHired = false,
                        IsInProgress = inProgress,
                        ApplicantCode = "",
                        CoverLetter = "",
                        CreatedByName = _currentUser.GetUserId().ToString(),
                        TestPassCode = "Test",
                        Stage = hireStageId,
                        Channel = payload.JobChannel
                    };

                    await _jobApplicationRepository.AddAsync(jobApplication);


                    var jobApplicantCV = new JobApplicantCVBank()
                    {
                        Id = SequentialGuid.Create(),
                        CVFileUrl = imageUrl,
                        FileName = payload.ApplicantCV.FileName,
                        FileType = payload.ApplicantCV.ContentType,
                        ApplicantionId = jobApplication.Id,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        CreatedDate = DateTime.Now,
                        CreatedByName = _currentUser.GetFullname(),
                        JobApplication = jobApplication
                    };

                    _dbContext.JobApplicantCVBanks.Add(jobApplicantCV);


                    var jobApplicationStageHistory = new JobApplicationStageHistory()
                    {

                        Id = SequentialGuid.Create(),
                        IsDeleted = false,
                        JobApplicationId = jobApplication.Id,
                        StatusDescription = $"{hireStageId.StageName}",
                        StageId = hireStageId.Id,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        JobApplication = jobApplication,
                        Stage = hireStageId,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);

                    if (hireStageId.EmailTemplateId != null)
                    {
                        EmailTemplateProcess emailTemplateProcess = new()
                        {
                            Id = SequentialGuid.Create(),
                            Description = "Apply For Job",
                            Email = jobApplication.JobApplicant.Email.Trim(),
                            UserId = jobApplication.JobApplicant.UserId,
                            TemplateId = (Guid)hireStageId.EmailTemplateId,
                            Processed = false,
                            EmailSent = false,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByName = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now
                        };

                        await _dbContext.EmailTemplateProcesses.AddAsync(emailTemplateProcess);
                    }


                    await _dbContext.SaveChangesAsync();
                    return ResponseModel<string>.Success("Application Successful");
                }
                else
                {
                    return ResponseModel<string>.Success("Applicant has an application in-progress");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<string>.Failure($"{ex.Message}");
            }
        }

        public async Task<ResponseModel<string>> SaveApplicantCVToCVBank(CreateApplicantCVtoCVBank payload)
        {
            try
            {

                var applicantProfile = await _dbContext.ApplicantProfiles.Where(j => j.Id == payload.ApplicantId && j.CompanyId == companyId).FirstOrDefaultAsync();

                if (applicantProfile == null)
                {
                    throw new Exception("applicant not found");
                }

                var jobApplicantCV = new JobApplicantCVBank()
                {
                    Id = SequentialGuid.Create(),
                    CVFileUrl = payload.ApplicantCVUrl,
                    ApplicantionId = applicantProfile.Id,
                    CompanyId = companyId,
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedDate = DateTime.Now,
                    CreatedByName = _currentUser.GetFullname(),
                    ApplicantProfile = applicantProfile
                };

                _dbContext.JobApplicantCVBanks.Add(jobApplicantCV);
                await _dbContext.SaveChangesAsync();

                return ResponseModel<string>.Success("Record Saved Successful to CV Bank.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<string>.Failure($"{ex.Message}");
            }
        }

        public async Task<ResponseModel<ApplicantQuizRecordDto>> GetApplicantQuizRecord(Guid id)
        {
            var data = await _dbContext.ApplicantQuizRecords.Where(x => x.Id == id && x.IsDeleted == false).Select(x => new ApplicantQuizRecordDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                QuizId = x.QuizId,
                UserId = x.CompanyId
            }).FirstOrDefaultAsync();

            if (data != null)
            {
                return ResponseModel<ApplicantQuizRecordDto>.Success(data);
            }
            else
            {
                return ResponseModel<ApplicantQuizRecordDto>.Failure("No result found");
            }
        }

        public async Task<ResponseModel<string>> AddUpdateJobApplication(ManageJobApplicationDto payload)
        {
            try
            {
                if (payload.Id != Guid.Empty)
                {
                    var item = await _dbContext.JobApplications.FirstOrDefaultAsync(x => x.Id == payload.Id && x.CompanyId == companyId);


                    item.CompanyId = companyId;
                    item.JobId = payload.JobId;
                    item.LastUpdate = payload.LastUpdate;
                    item.ModifiedBy = _currentUser.GetFullname();
                    item.ModifiedDate = DateTime.Now;
                    item.JobApplicantId = payload.JobApplicantId;
                    item.HireStageId = payload.HireStageId;


                    _dbContext.JobApplications.Update(item);
                    await _jobApplicationRepository.SaveChangesAsync();
                }
                else
                {
                    var item = new JobApplication
                    {
                        CompanyId = companyId,
                        JobApplicantId = payload.JobApplicantId,
                        AlertSent = payload.AlertSent,
                        DateApplied = payload.DateApplied,
                        ApplicantCode = payload.ApplicantCode,
                        CoverLetter = payload.CoverLetter,
                        HireStageId = payload.HireStageId,
                        CreatedBy = _currentUser.GetUserId(),
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                    };
                    await _dbContext.JobApplications.AddAsync(item);
                    await _jobApplicationRepository.SaveChangesAsync();
                }

                return ResponseModel<string>.Success(" record was saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResponseModel<string>.Exception($"{ex.Message}");
            }
        }
        public async Task<ResponseModel<JobApplicationDto>> ViewJobApplicationProfileById(Guid JobApplicantId)
        {
            try
            {
                var result = await _dbContext.JobApplications.Include(s => s.JobApplicant).
                        Where(a => a.IsDeleted == false && a.CompanyId ==
                        companyId && a.IsDeleted == false && a.JobApplicantId == JobApplicantId)
                        .Select(x => new JobApplicationDto
                        {

                        }).FirstOrDefaultAsync();
                if (result != null)
                {
                    return ResponseModel<JobApplicationDto>.Success(result);
                }
                else
                {
                    return ResponseModel<JobApplicationDto>.Success(result);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(ViewJobApplicationProfileById));
                return ResponseModel<JobApplicationDto>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<List<InterviwerListforSharingDTO>>> GetInterviwerforSharing(Guid applicationId, Guid hirestageId, Guid subhirestageId)
        {
            var IntervierList = new List<InterviwerListforSharingDTO>();
            try
            {
                IntervierList = await (from i in _dbContext.JobScheduleInterviews
                                       where i.JobApplicantionId == applicationId
                                       && i.HireStageId == hirestageId
                                       && (i.SubhireStageId == subhirestageId || subhirestageId == Guid.Empty)
                                       select new InterviwerListforSharingDTO()
                                       {
                                           InterviewerEmployeeId = i.InterviewerEmployeeId,
                                           JobApplicantId = i.JobApplicantionId,
                                           InterviewDate = i.InterviewDate,
                                           InterviewTime = i.InterviewTime,
                                           Duration = i.Duration,
                                           InterviewType = i.InterviewType
                                       }).ToListAsync();
                return ResponseModel<List<InterviwerListforSharingDTO>>.Success(IntervierList);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(GetInterviwerforSharing));
                return ResponseModel<List<InterviwerListforSharingDTO>>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<string>> ShareApplicantProfileForInterview(ScheduleJobInterviewDto payload)
        {
            try
            {

                var applicantJobApplication = _dbContext.JobApplications.Where(x => x.JobApplicantId == payload.JobApplicantId && x.IsInProgress == true).FirstOrDefault();
                if (applicantJobApplication != null)
                {
                    foreach (var item in payload.InterviewerEmployee)
                    {
                        var scheduleInterviewer = await _dbContext.JobScheduleInterviews.Where(p => p.InterviewerEmployeeId == item.InterviewerEmployeeId//payload.InterviewerEmployeeId
                                          && p.JobApplicantionId == applicantJobApplication.Id).FirstOrDefaultAsync();

                        if (scheduleInterviewer == null)
                        {
                            return ResponseModel<string>.Failure("Interviewer Not Found");


                        }
                        else
                        {
                            scheduleInterviewer.IsShared = true;
                            scheduleInterviewer.Status = InterViewStatus.Open;
                            _dbContext.JobScheduleInterviews.Update(scheduleInterviewer);
                        }
                        var r0 = await _dbContext.SaveChangesAsync();
                    }

                    return ResponseModel<string>.Success("");

                }
                else
                {
                    return ResponseModel<string>.Failure("Invalid Job Application");
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(ShareApplicantProfileForInterview));
                return ResponseModel<string>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<string>> SendMailToApplicant(Guid Id)
        {

            var objModel = new JobApplicationMailDto();
            try
            {
                var applicantToSendMailTo = await (from j in _dbContext.JobApplications
                                                   from h in _dbContext.Stages.Where(h => h.Id == j.HireStageId).DefaultIfEmpty()
                                                   from s in _dbContext.SubStages.Where(s => s.StageId == j.HireStageId).DefaultIfEmpty()
                                                   where j.JobApplicantId == Id && j.IsInProgress

                                                   select new JobApplicationMailDto
                                                   {
                                                       JobApplicantionId = j.Id,
                                                       JobApplicantId = j.JobApplicantId,
                                                       IsInprogress = j.IsInProgress,
                                                       HireStageId = j.HireStageId,
                                                       EmailTemplateIdH = h.EmailTemplateId,
                                                       SubHireStageId = j.SubhireStageId,
                                                       ApplicationStatus = j.ApplicationStatus,
                                                       EmailTemplateIdS = s.EmailTemplateId,
                                                       JobId = j.JobId
                                                   }).FirstOrDefaultAsync();

                var jobId = applicantToSendMailTo.JobId;
                var jobObj = await _dbContext.Jobs.Where(m => m.Id == jobId).FirstOrDefaultAsync();
                var jobRoleName = jobObj.JobTitle;
                var AnnualSal = jobObj.SalaryRange;

                var companyName = _currentUser.GetCompany();

                if (applicantToSendMailTo == null)
                {
                    return ResponseModel<string>.Failure("No Hire stage record available for this applicant yet");
                }

                var applicationId = applicantToSendMailTo.JobApplicantionId;
                var mailContent = await _dbContext.JobScheduleInterviews.Where(x => x.JobApplicantionId == applicationId).FirstOrDefaultAsync();

                if (mailContent == null)
                {
                    if (applicantToSendMailTo.EmailTemplateIdS != Guid.Empty && applicantToSendMailTo.EmailTemplateIdH != Guid.Empty)
                    {
                        var emailTempID = applicantToSendMailTo.EmailTemplateIdS;
                        var applicantDetails = await _dbContext.ApplicantProfiles.Where(v => v.Id == Id).FirstOrDefaultAsync();
                        var applicantName = applicantDetails.FirstName + " " + applicantDetails.LastName;
                        var applicantFirstName = applicantDetails.FirstName;


                        var msgResponse = await SendMessageByTemplate(emailTempID.ToString(), applicantDetails.UserId.ToString(), applicantDetails.Email);//send Message

                        if (msgResponse != null)
                        {
                            return ResponseModel<string>.Success(msgResponse);
                        }

                        return ResponseModel<string>.Failure("Message not sent");
                    }
                    else if (applicantToSendMailTo.EmailTemplateIdS != Guid.Empty && applicantToSendMailTo.EmailTemplateIdH == Guid.Empty)
                    {
                        var emailTempID = applicantToSendMailTo.EmailTemplateIdH;
                        var applicantDetails = await _dbContext.ApplicantProfiles.Where(v => v.Id == Id).FirstOrDefaultAsync();
                        var applicantName = applicantDetails.FirstName + " " + applicantDetails.LastName;
                        var applicantFirstName = applicantDetails.FirstName;


                        var msgResponse = await SendMessageByTemplate(emailTempID.ToString(), applicantDetails.UserId.ToString(), applicantDetails.Email);


                        if (msgResponse != null)
                        {
                            return ResponseModel<string>.Success("Message sent Successfully");
                        }

                        return ResponseModel<string>.Failure("Message not sent");
                    }
                }
                else if (mailContent != null)
                {
                    var interviewDate = mailContent.InterviewDate;
                    var interviewTime = mailContent.InterviewTime;
                    var interviewType = mailContent.InterviewType;
                    var interviewDuration = mailContent.Duration;
                    var JRoleName = jobRoleName;
                    var jobTitle = jobObj.JobTitle;

                    if (applicantToSendMailTo.EmailTemplateIdS != Guid.Empty && applicantToSendMailTo.EmailTemplateIdH != Guid.Empty)
                    {
                        var emailTempID = applicantToSendMailTo.EmailTemplateIdS;
                        var applicantDetails = await _dbContext.ApplicantProfiles.Where(v => v.Id == Id).FirstOrDefaultAsync();
                        var applicantName = applicantDetails.FirstName + " " + applicantDetails.LastName;
                        var applicantFirstName = applicantDetails.FirstName;

                        var msgResponse = true; //Send Notification

                        if (msgResponse)
                        {
                            return ResponseModel<string>.Success("Message sent Successfully");
                        }

                        return ResponseModel<string>.Failure("Message not sent");
                    }
                    else if (applicantToSendMailTo.EmailTemplateIdS == Guid.Empty && applicantToSendMailTo.EmailTemplateIdH != Guid.Empty)
                    {
                        var emailTempID = applicantToSendMailTo.EmailTemplateIdH;
                        var applicantDetails = await _dbContext.ApplicantProfiles.Where(v => v.Id == Id).FirstOrDefaultAsync();
                        var applicantName = applicantDetails.FirstName + " " + applicantDetails.LastName;
                        var applicantFirstName = applicantDetails.FirstName;


                        var msgResponse = await SendMessageByTemplate(emailTempID.ToString(), applicantDetails.UserId.ToString(), applicantDetails.Email);


                        if (msgResponse != null)
                        {
                            return ResponseModel<string>.Success("Message sent Successfully");
                        }

                        return ResponseModel<string>.Success("Message not sent");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(GetInterviwerforSharing));
                return ResponseModel<string>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>> FetchJobInterviewerListByApplicationId(ApplicantInterviewScheduleFilterDTO payload)
        {
            try
            {
                /// Fetch all record 


                var data = await (from a in _dbContext.JobScheduleInterviews
                                  from ap in _dbContext.JobApplications.Where(x => x.Id == a.JobApplicantionId)
                                  from apf in _dbContext.ApplicantProfiles.Where(x => x.Id == a.JobApplication.JobApplicantId).DefaultIfEmpty()
                                  where a.IsActive == true && a.CompanyId == companyId
                                  && a.IsDeleted == false && a.InterviewerEmployeeId == _currentUser.GetUserId()
                                  && a.JobApplication.JobApplicantId == payload.JobApplicantId
                                  select new ApplicantInterviewScheduleDTO()
                                  {
                                      JobApplicantId = apf.Id,
                                      JobApplicantName = apf.FirstName + " " + apf.LastName,
                                      InterviewerEmployeeId = a.InterviewerEmployeeId,
                                      JobRoleName = a.JobTitle,
                                      InterviewDate = a.InterviewDate,
                                      InterviewTime = a.InterviewTime,
                                      Duration = a.Duration,
                                      JobApplicantionId = a.JobApplicantionId,
                                      IsShared = a.IsShared,
                                      HireStageId = a.HireStageId,
                                      SubhireStageId = a.SubhireStageId,
                                      InterviewType = a.InterviewType
                                  }).AsQueryable().OrderBy(x => x.InterviewDate).ToListAsync();



                if (!string.IsNullOrEmpty(payload.JobApplicantName))
                {
                    data = data.Where(x => x.JobApplicantName.Contains(payload.JobApplicantName)).ToList();
                }

                if (payload.InterviewDate.HasValue)
                {
                    data = data.Where(x => x.InterviewDate == payload.InterviewDate).ToList();
                }

                if (payload.Duration > 0)
                {
                    data = data.Where(x => x.Duration == payload.Duration).ToList();
                }


                if (data != null)
                {
                    CustomPagination<List<ApplicantInterviewScheduleDTO>> response = new CustomPagination<List<ApplicantInterviewScheduleDTO>>()
                    {
                        modelresult = data.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                        pageNumber = payload.PageNumber,
                        pageSize = payload.PageSize,
                        TotalCount = data.Count()
                    };

                    return ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>.Failure("Not Found.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(GetInterviwerforSharing));
                return ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>> FetchJobApplicationByRole(JobApplicationByJobTitle filter)
        {
            try
            {

                List<JobApplicationListingDTO> applicationByJobTitle = new List<JobApplicationListingDTO>();

                //Get EmployeeId by the userToken

                var emp = _currentUser.GetUserId().ToString();

                var employeeId = GetEmployeeData(emp).Result;


                if (employeeId != null)
                {
                    var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

                    /// Fetch all record 
                    /// 

                    var data = new List<JobScheduleInterview>();

                    if (employeeId.EmployeeId != null)
                    {
                        data = await _dbContext.JobScheduleInterviews
                                      .Include(x => x.HireStage)
                                      .Include(x => x.JobApplication)
                                      .ThenInclude(x => x.Job)
                                      .Include(x => x.JobApplication)
                                      .ThenInclude(x => x.JobApplicant)
                            .Where(x => x.CompanyId == companyId
                            && x.IsDeleted == false
                            && x.InterviewerEmployeeId == interviewerEmployeeId
                            && x.HireStage.StageName.Contains("Interview"))
                                                .ToListAsync();
                    }
                    //else
                    //{
                    //    data = await _dbContext.JobApplications
                    //                            .Include(x => x.Job)
                    //                            .ThenInclude(x => x.JobReviewers)
                    //                            .Where(x => x.CompanyId == companyId && x.IsDeleted == false)
                    //                            .ToListAsync();
                    //}


                    //var data = await _dbContext.JobScheduleInterviews
                    //                   .Include(x => x.JobApplication)
                    //                   .ThenInclude(x => x.Job)
                    //                   .Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.InterviewerEmployeeId == interviewerEmployeeId)
                    //                   .ToListAsync();

                    data = data.DistinctBy(x => x.JobApplication.Id).ToList();

                    if (!string.IsNullOrEmpty(filter.JobTitle))
                    {
                        data = data.Where(x => x.JobApplication.Job.JobTitle.ToLower().Contains(filter.JobTitle.ToLower())).ToList();
                    }

                    if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                    {
                        data = data.Where(n => n.JobApplication.DateApplied.Date >= filter.StartDate.Value.Date
                        && n.JobApplication.DateApplied.Date <= filter.EndDate.Value.Date).ToList();
                    }

                    applicationByJobTitle = data.GroupBy(x => x.JobApplication.Job.JobTitle).Select(x => new JobApplicationListingDTO { JobTitle = x.Key, TotalCount = x.Count() }).ToList();
                }
                else
                {
                    return ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>.Failure("Admin Data not found");
                }

                //else
                //{

                //	/// Fetch all record 

                //	var data = await _dbContext.JobApplications.Include(x => x.Job).Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToListAsync();

                //	if (!string.IsNullOrEmpty(filter.JobTitle))
                //	{
                //		data = data.Where(x => x.Job.JobTitle.Equals(filter.JobTitle)).ToList();
                //	}

                //	applicationByJobTitle = data.GroupBy(x => x.Job.JobTitle).Select(x => new JobApplicationListingDTO { JobTitle = x.Key, TotalCount = x.Count() }).ToList();
                //}

                if (applicationByJobTitle != null)
                {

                    CustomPagination<List<JobApplicationListingDTO>> response = new CustomPagination<List<JobApplicationListingDTO>>()
                    {
                        modelresult = applicationByJobTitle.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        TotalCount = applicationByJobTitle.Count
                    };

                    return ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>.Failure("Not Record Found.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<CustomPagination<List<JobApplicationListDto>>>> FetchJobApplications(JobApplicationSearch payload)
        {

            var result = new List<JobApplicationListDto>();
            try
            {


                var jobApplication = new List<JobApplication>();


                if (payload.StartDate.HasValue && payload.EndDate.HasValue)
                {
                    jobApplication = await _dbContext.JobApplications
                                          .Include(x => x.Job)
                                          .Include(x => x.JobApplicant)
                                          .Where(x => x.CompanyId == companyId && x.IsDeleted == false &&
                                          x.DateApplied.Date >= payload.StartDate.Value.Date
                                          && x.DateApplied.Date <= payload.EndDate.Value.Date).ToListAsync();
                }
                else
                {
                    jobApplication = await _dbContext.JobApplications
                                         .Include(x => x.Job)
                                         .Include(x => x.JobApplicant)
                                         .Where(x => x.CompanyId == companyId && x.IsDeleted == false)
                                         .ToListAsync();
                }


                result = jobApplication.Select(p => new JobApplicationListDto
                {

                    Id = p.Id,
                    JobApplicantId = p.JobApplicantId,
                    JobId = p.JobId,
                    ApplicantName = (p.JobApplicant.FirstName + " " + p.JobApplicant.LastName),
                    JobTitle = p.Job.JobTitle,
                    ApplicantCode = p.ApplicantCode,
                    CountryId = p.Job.CountryId,
                    StateId = p.Job.StateId,
                    DateApplied = p.DateApplied,
                    ScoreCardDetail = p.ScoreCardDetail,
                    ScoreCardValue = p.ScoreCardValue,
                    JobLocation = p.JobLocation,
                    CompanyId = p.CompanyId,
                    IsDeleted = p.IsDeleted,
                    DateCreated = p.CreatedDate,
                    CreatedById = p.CreatedBy,
                    ModifiedById = p.ModifiedBy,
                    DateModified = p.ModifiedDate,
                    IsInProgress = p.IsInProgress,
                    HireStageId = p.HireStageId,
                    SubHireStageId = p.SubhireStageId,
                    ApplicationStatus = p.ApplicationStatus,
                    ApplicantEmail = p.JobApplicant.Email,
                    DateRecruited = p.DateRecruited,
                }).OrderByDescending(x => x.DateCreated).ToList();



                try
                {
                    foreach (var item in result)
                    {
                        if (item.HireStageId != null)
                        {
                            item.HireStageName = StageNameById(item.HireStageId).Result;
                        }
                        if (item.SubHireStageId != null)
                        {
                            item.SubHireStageName = SubStageNameById(item.SubHireStageId).Result;
                        }
                    }

                }
                catch (Exception)
                {

                    _logger.LogError("Failed to update stage names");
                }


                if (payload.JobApplicantId.HasValue)
                {
                    result = result.Where(x => x.JobApplicantId == payload.JobApplicantId).ToList();
                }

                if (!string.IsNullOrEmpty(payload.JobTitle))
                {
                    result = result.Where(x => x.JobTitle.ToLower().Contains(payload.JobTitle.ToLower())).ToList();
                }

                if (payload.ApplicationDate.HasValue)
                {
                    result = result.Where(x => x.DateApplied == payload.ApplicationDate).ToList();
                }


                if (!string.IsNullOrEmpty(payload.Status))
                {
                    result = result.Where(jr => jr.HireStageName == payload.Status || jr.SubHireStageName == payload.Status).ToList();
                }

                result = result.Where(x => !(x.HireStageName.Contains("Rejected"))).ToList();

                CustomPagination<List<JobApplicationListDto>> response = new CustomPagination<List<JobApplicationListDto>>()
                {
                    modelresult = result.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                    pageNumber = payload.PageNumber,
                    pageSize = payload.PageSize,
                    TotalCount = result.Count
                };

                foreach (var item in response.modelresult)
                {
                    item.QuizScore = await GetQuizScore(item.Id);
                }

                return ResponseModel<CustomPagination<List<JobApplicationListDto>>>.Success(response);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(GetInterviwerforSharing));
                return ResponseModel<CustomPagination<List<JobApplicationListDto>>>.Exception("Exception error " + ex.Message);
            }
        }


        //All Job Application Updated Select List
        public async Task<ResponseModel<CustomPagination<List<JobApplicationDto>>>> FetchAllJobApplications(JobApplicationList payload)
        {
            try
            {
                ///// Fetch all record 
                var result = new List<JobApplicationDto>();
                result = await (from p in _dbContext.JobApplications
                                join c in _dbContext.ApplicantProfiles
                                on p.JobApplicantId equals c.Id
                                join b in _dbContext.Jobs
                                on p.JobId equals b.Id
                                where p.IsDeleted == false && p.CompanyId == companyId
                                && (p.JobId == Guid.Empty || payload.JobId == p.JobId)
                                    && (p.JobApplicantId == Guid.Empty || payload.JobApplicantId == p.JobApplicantId)
                                    && (p.DateApplied == null || payload.ApplicationDate == p.DateApplied)

                                select new JobApplicationDto
                                {
                                    Id = p.Id,
                                    JobApplicantId = p.JobApplicantId,
                                    JobId = p.JobId,
                                    ApplicantCode = p.ApplicantCode,
                                    DateApplied = p.DateApplied,
                                    ScoreCardDetail = p.ScoreCardDetail,
                                    ScoreCardValue = p.ScoreCardValue,
                                    JobLocation = p.JobLocation,
                                    CompanyId = p.CompanyId,
                                    IsDeleted = p.IsDeleted,
                                    DateRecruited = p.DateRecruited,
                                    ApplicationStatus = p.ApplicationStatus
                                }).ToListAsync();


                if (result != null)
                {

                    CustomPagination<List<JobApplicationDto>> response = new CustomPagination<List<JobApplicationDto>>()
                    {
                        modelresult = result.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                        pageNumber = payload.PageNumber,
                        pageSize = payload.PageSize,
                        TotalCount = result.Count
                    };

                    return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Failure("Not Found.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Exception(ex.Message);
            }
        }
        //Applicant Drop off stage
        public async Task<ResponseModel<List<FetchApplicantDropOffStageDto>>> FetchApplicantDropOffStage(int year)
        {
            var applicationdroppedofflist = new List<FetchApplicantDropOffStageDto>();
            try
            {
                //var result = new List<FetchApplicantDropOffStageDto>();
                var hirestages = await _dbContext.Stages.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToListAsync();
                if (hirestages.Count > 0)
                {

                    hirestages.ForEach(h =>
                    {
                        var jbApplications = _dbContext.JobApplications.Where(x => x.HireStageId == h.Id && x.ApplicationStatus == "Dropped Off" && x.DateApplied.Year == year).ToList();
                        var appDroppedOff = new FetchApplicantDropOffStageDto();
                        appDroppedOff.CountOfAppllicantDropOfStageId = jbApplications.Count;
                        appDroppedOff.DropOffStageName = h.StageName;
                        applicationdroppedofflist.Add(appDroppedOff);
                    });

                }
                return ResponseModel<List<FetchApplicantDropOffStageDto>>.Success(applicationdroppedofflist);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(FetchApplicantDropOffStage));
                return ResponseModel<List<FetchApplicantDropOffStageDto>>.Exception("Exception error " + ex.Message);
            }
        }
        //All Hired Applicants
        public async Task<ResponseModel<CustomPagination<List<JobApplicationDto>>>> FetchAllHiredApplicants(JobApplicationList payload)
        {
            try
            {
                ///// Fetch all record 
                var result = new List<JobApplicationDto>();
                result = await (from p in _dbContext.JobApplications
                                where p.IsDeleted == false && p.IsHired && p.CompanyId == companyId
                                select new JobApplicationDto
                                {
                                    Id = p.Id,
                                    JobApplicantId = p.JobApplicantId,
                                    JobId = p.JobId,
                                    ApplicantCode = p.ApplicantCode,
                                    DateApplied = p.DateApplied,
                                    ScoreCardDetail = p.ScoreCardDetail,
                                    ScoreCardValue = p.ScoreCardValue,
                                    JobLocation = p.JobLocation,
                                    CompanyId = p.CompanyId,
                                    IsDeleted = p.IsDeleted,
                                    DateRecruited = p.DateRecruited,
                                    CreatedById = p.CreatedBy,
                                    ModifiedById = p.ModifiedBy,
                                    DateModified = p.ModifiedDate
                                }).ToListAsync();

                if (result != null)
                {

                    CustomPagination<List<JobApplicationDto>> response = new CustomPagination<List<JobApplicationDto>>()
                    {
                        modelresult = result.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                        pageNumber = payload.PageNumber,
                        pageSize = payload.PageSize,
                        TotalCount = result.Count
                    };

                    return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Failure("Not Found.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Exception(ex.Message);
            }
        }
        //All Rejected Offers
        public async Task<ResponseModel<CustomPagination<List<JobApplicationDto>>>> FetchAllRejectedOffers(JobApplicationList payload)
        {
            try
            {
                ///// Fetch all record 
                var result = new List<JobApplicationDto>();
                result = await (from p in _dbContext.JobApplications
                                where p.IsDeleted == false && p.IsHired == false && p.ApplicationStatus == "Rejected" && p.CompanyId == companyId
                                select new JobApplicationDto
                                {
                                    Id = p.Id,
                                    JobApplicantId = p.JobApplicantId,
                                    JobId = p.JobId,
                                    ApplicantCode = p.ApplicantCode,
                                    DateApplied = p.DateApplied,
                                    ScoreCardDetail = p.ScoreCardDetail,
                                    ScoreCardValue = p.ScoreCardValue,
                                    JobLocation = p.JobLocation,
                                    CompanyId = p.CompanyId,
                                    IsDeleted = p.IsDeleted,
                                    CreatedById = p.CreatedBy,
                                    DateCreated = p.CreatedDate,
                                    DateModified = p.ModifiedDate,
                                    ModifiedById = p.ModifiedBy
                                }).ToListAsync();

                if (result != null)
                {

                    CustomPagination<List<JobApplicationDto>> response = new CustomPagination<List<JobApplicationDto>>()
                    {
                        modelresult = result.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                        pageNumber = payload.PageNumber,
                        pageSize = payload.PageSize,
                        TotalCount = result.Count
                    };

                    return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Failure("Not Found.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobApplicationDto>>>.Exception(ex.Message);
            }
        }
        //List All Jobs Source
        public async Task<ResponseModel<List<JobApplicantionSourceDto>>> FetchJobApplicationBySource(int year)
        {

            var result = new List<JobApplicantionSourceDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(""))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_RECRUITMENT_DASHBOARD_BAR_CHART_DATA", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Year", year));
                        //var result = new List<JobApplicantionSourceDto>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                JobApplicantionSourceDto jobSrc = new JobApplicantionSourceDto
                                {
                                    JobSource = reader["JobSource"].ToString(),
                                    NoOfApplicants = Convert.ToInt32(reader["NoOfApplicants"]),
                                    YearApplied = Convert.ToInt32(reader["YearApplied"])
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<JobApplicantionSourceDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<JobApplicantionSourceDto>>.Exception(ex.Message);
            }

        }
        //List All Jobs By Role
        public async Task<ResponseModel<List<JobsByRoleDto>>> FetchAllJobsByRole(int year)
        {

            var result = new List<JobsByRoleDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(""))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_RECRUITMENT_DASHBOARD_JOBS_PER_ROLE_PIE_CHART_DATA", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Year", year));
                        //var result = new List<JobApplicantionSourceDto>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                JobsByRoleDto jobSrc = new JobsByRoleDto
                                {
                                    JobRoleName = reader["JobRoleName"].ToString(),
                                    NoOfJobsPerRole = Convert.ToInt32(reader["NoOfJobsPerRole"]),
                                    YearCreated = Convert.ToInt32(reader["YearCreated"])
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<JobsByRoleDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<JobsByRoleDto>>.Exception(ex.Message);
            }

        }
        //List All Jobs By Department
        public async Task<ResponseModel<List<JobsByDepartmentDto>>> FetchAllJobsByDepartment(int year)
        {

            var result = new List<JobsByDepartmentDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(""))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_RECRUITMENT_DASHBOARD_JOBS_PER_DEPARTMENT_PIE_CHART_DATA", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Year", year));
                        //var result = new List<JobApplicantionSourceDto>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                JobsByDepartmentDto jobSrc = new JobsByDepartmentDto
                                {
                                    DepartmentName = reader["DepartmentName"].ToString(),
                                    NoOfJobsPerDepartment = Convert.ToInt32(reader["NoOfJobsPerDepartment"]),
                                    YearCreated = Convert.ToInt32(reader["YearCreated"])
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<JobsByDepartmentDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<JobsByDepartmentDto>>.Exception(ex.Message);
            }

        }
        //List All Application By Age Band
        public async Task<ResponseModel<List<JobApplicationAgeBandDto>>> FetchJobApplicationByAgeBand(int year)
        {

            var result = new List<JobApplicationAgeBandDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_RECRUITMENT_DASHBOARD_JOB_APLICATION_PER_AGE_BAND_BARCHART_DATA", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Year", year));
                        //var result = new List<JobApplicantionSourceDto>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                JobApplicationAgeBandDto jobSrc = new JobApplicationAgeBandDto
                                {
                                    AgeBand = reader["AgeBand"].ToString(),
                                    NoOfApplicants = Convert.ToInt32(reader["NoOfApplicants"]),
                                    YearApplied = Convert.ToInt32(reader["YearApplied"])
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<JobApplicationAgeBandDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<JobApplicationAgeBandDto>>.Exception(ex.Message);
            }

        }
        //List Recruitment Over Time
        public async Task<ResponseModel<List<JobRecruitmentOvertimeDto>>> GetRecruitmentOverTime(int year)
        {
            var result = new List<JobRecruitmentOvertimeDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_DASHBOARD_JOB_RECRUITMENT_OVER_TIME_LINECHART_DATA", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Year", year));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                JobRecruitmentOvertimeDto jobSrc = new JobRecruitmentOvertimeDto
                                {
                                    MonthRecruited = reader["MonthRecruited"].ToString(),
                                    NoOfApplications = Convert.ToInt32(reader["NoOfApplications"])//,
                                                                                                  //YearApplied = Convert.ToInt32(reader["YearApplied"])
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<JobRecruitmentOvertimeDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<JobRecruitmentOvertimeDto>>.Exception(ex.Message);
            }

        }
        //List Employee Skill 
        public async Task<ResponseModel<List<EmployeeSkillDistributionDto>>> ListEmployeeSkillDistribution(int year)
        {

            var result = new List<EmployeeSkillDistributionDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_DASHBOARD_DISTRIBUTION_BY_SKILLSET_PIE_CHART_DATA", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Year", year));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                EmployeeSkillDistributionDto jobSrc = new EmployeeSkillDistributionDto
                                {
                                    SkillName = reader["SkillName"].ToString(),
                                    NoOfEmployees = Convert.ToInt32(reader["NoOfEmployees"])//,
                                                                                            //YearApplied = Convert.ToInt32(reader["YearApplied"])
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<EmployeeSkillDistributionDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<EmployeeSkillDistributionDto>>.Exception(ex.Message);
            }

        }
        public async Task<ResponseModel<List<InvitedApplicantsListDto>>> ListOfInvitedApplicants(JobApplicationList payload)
        {

            var result = new List<InvitedApplicantsListDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_INTERVIEWER_DASHBOARD_INVITED_APPLICANTS_CARD", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Add(new SqlParameter("@Year", year));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                InvitedApplicantsListDto jobSrc = new InvitedApplicantsListDto
                                {
                                    NoOfInvitedApplicants = Convert.ToInt32(reader["NoOfInvitedApplicants"]),
                                    StatusName = reader["StatusName"].ToString()//,
                                                                                //YearApplied = Convert.ToInt32(reader["YearApplied"])
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<InvitedApplicantsListDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<InvitedApplicantsListDto>>.Exception(ex.Message);
            }

        }
        public async Task<ResponseModel<List<InterviewedApplicantsDto>>> ListOfInterviewedApplicants(JobApplicationList payload)
        {
            var result = new List<InterviewedApplicantsDto>();
            try
            {
                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_INTERVIEWER_DASHBOARD_INTERVIEWED_APPLICANTS_CARD", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                InterviewedApplicantsDto jobSrc = new InterviewedApplicantsDto
                                {
                                    NoOfInterviewedApplicants = Convert.ToInt32(reader["NoOfInterviewedApplicants"]),
                                    StatusName = reader["StatusName"].ToString()
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<InterviewedApplicantsDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<InterviewedApplicantsDto>>.Exception(ex.Message);
            }

        }
        public async Task<ResponseModel<List<ApplicantsPendingOfferListDto>>> ListOfApplicantsPendingOffer(JobApplicationList payload)
        {

            var result = new List<ApplicantsPendingOfferListDto>();
            try
            {

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand("STP_INTERVIEWER_DASHBOARD_PENDING_OFFER_APPLICANTS_CARD", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ApplicantsPendingOfferListDto jobSrc = new ApplicantsPendingOfferListDto
                                {
                                    NoOfPendingOffers = Convert.ToInt32(reader["NoOfPendingOffers"]),
                                    StatusName = reader["StatusName"].ToString()
                                };
                                result.Add(jobSrc);
                            }
                        }
                        return ResponseModel<List<ApplicantsPendingOfferListDto>>.Success(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<ApplicantsPendingOfferListDto>>.Exception(ex.Message);
            }

        } //
        public async Task<ResponseModel<List<InterviewHistoryDto>>> ListApplicantInterviewHistory(Guid applicationId, Guid applicantId)
        {
            var result = new List<InterviewHistoryDto>();
            try
            {
                result = await (from h in _dbContext.JobInterviewHistories
                                where h.IsActive && h.IsDeleted == false
                                && (h.JobApplicantId == applicantId)
                                && (h.JobApplicationId == applicationId)
                                select new InterviewHistoryDto
                                {
                                    JobApplicationId = h.JobApplicationId,
                                    JobApplicantId = h.JobApplicantId,
                                    StatusDescription = h.StatusDescription,
                                    DateTriggered = h.CreatedDate
                                }).ToListAsync();

                return ResponseModel<List<InterviewHistoryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<InterviewHistoryDto>>.Exception(ex.Message);
            }

        }
        public async Task<ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>> GetAllUpComingTasks(JobApplicationList payload)
        {
            try
            {
                ///// Fetch all record 
                var result = new List<JobApplicationUpcomingInterviews>();
                result = await (from i in _dbContext.JobScheduleInterviews
                                join c in _dbContext.JobApplications
                                on i.JobApplicantionId equals c.Id
                                join a in _dbContext.ApplicantProfiles
                                on c.JobApplicantId equals a.Id
                                where i.IsActive && i.IsDeleted == false && i.CompanyId == companyId && c.IsHired == false
                                && i.InterviewerEmployeeId == _currentUser.GetUserId()
                                select new JobApplicationUpcomingInterviews
                                {
                                    //Id = p.Id,
                                    ApplicantName = $"{a.FirstName} {a.LastName}",
                                    JobRoleName = i.JobTitle,
                                    InterviewDate = i.InterviewDate,
                                    Duration = i.Duration,
                                }).ToListAsync();

                if (result != null)
                {

                    CustomPagination<List<JobApplicationUpcomingInterviews>> response = new CustomPagination<List<JobApplicationUpcomingInterviews>>()
                    {
                        modelresult = result.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                        pageNumber = payload.PageNumber,
                        pageSize = payload.PageSize,
                        TotalCount = result.Count
                    };

                    return ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>.Failure("You have no interviews scheduled today");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>.Exception(ex.Message);
            }
        }
        public async Task<ResponseModel<string>> JobApplicationApproval(JobApplicationUpdateStatus payload)
        {
            try
            {

                #region Duplicate

                var item = await _dbContext.JobApplications.FirstOrDefaultAsync(x =>
                       (x.JobApplicantId == payload.JobApplicantId && x.CompanyId == companyId));
                if (item == null)
                {
                    return ResponseModel<string>.Failure("applicant information");
                }
                #endregion

                item.HireStageId = payload.HireStage;
                item.ModifiedDate = DateTime.Now;
                item.ModifiedBy = _currentUser.GetFullname();
                item.AlertSent = false;
                item.ApplicantCode = "";


                _jobApplicationRepository.Update(item);
                await _jobApplicationRepository.SaveChangesAsync();

                return ResponseModel<string>.Success("Recruitment Job Application Status");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<string>.Exception(ex.Message);

            }
        }
        public async Task<ResponseModel<string>> AddUpdateScheduleJobInterviews(JobApplicantScheduleInterview payload)
        {
            var models = new List<JobScheduleInterview>();

            try
            {
                var applinbcantApplication = _dbContext.JobApplications
                                            .Include(x => x.Job)
                                            .Include(x => x.JobApplicant)
                                            .Include(x => x.Stage)
                                            .Where(x => x.Id == payload.JobApplicantionId
                                            && x.IsInProgress == true && x.CompanyId == companyId)
                                            .FirstOrDefault();


                var currentStage = await _dbContext.Stages.FirstOrDefaultAsync(x => x.Id == applinbcantApplication.HireStageId);

                if (currentStage == null)
                {
                    return ResponseModel<string>.Failure("Current stage not found.");
                }


                if (applinbcantApplication != null)
                {

                    var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());

                    var result2 = new List<JobScheduleInterview>();

                    if (employeeId == null)
                    {
                        return ResponseModel<string>.Failure("Hr/Admin not found");
                    }
                    var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

                    var interviwer = payload.InterviewerIds.FirstOrDefault(x => x.Equals(interviewerEmployeeId));

                    var checkIfInterviewerExit = await _dbContext.JobScheduleInterviews.Where(x => x.JobApplicantionId == applinbcantApplication.Id && x.CompanyId == companyId && x.IsDeleted == false && x.InterviewerEmployeeId == interviwer && x.SubhireStageId == payload.SubHireStageId && x.HireStageId == payload.HireStageId).FirstOrDefaultAsync();

                    if (checkIfInterviewerExit != null)
                    {
                        checkIfInterviewerExit.InterviewDate = payload.InterviewDate;
                        checkIfInterviewerExit.InterviewTime = payload.InterviewTime;
                        checkIfInterviewerExit.InterviewType = payload.InterviewType;
                        checkIfInterviewerExit.Duration = payload.Duration;

                        _dbContext.JobScheduleInterviews.Update(checkIfInterviewerExit);
                        await _dbContext.SaveChangesAsync();

                        await CreateUpcomingTask($"Fill in score card for {checkIfInterviewerExit.JobTitle}", checkIfInterviewerExit.InterviewDate, checkIfInterviewerExit.InterviewerEmployeeId);

                        if (currentStage.EmailTemplateId != null)
                        {
                            var meesage = await SendMessageByTemplate(currentStage.EmailTemplateId.ToString(), applinbcantApplication.JobApplicant.UserId.ToString(), applinbcantApplication.JobApplicant.Email.Trim());
                            return ResponseModel<string>.Success("Record Updated successfully");
                        }
                        else
                        {
                            _logger.LogInformation($"Email template Id is null for SubStage {applinbcantApplication.HireStageId}");
                            return ResponseModel<string>.Success("Record Updated successfully");
                        }

                    }
                    else
                    {


                        if (currentStage.StageName.Contains("Interview"))
                        {
                            foreach (var Schedule in payload.InterviewerIds)

                            {
                                var addMember = new JobScheduleInterview
                                {
                                    Id = SequentialGuid.Create(),
                                    InterviewDate = payload.InterviewDate,
                                    InterviewTime = payload.InterviewTime,
                                    Duration = payload.Duration,
                                    InterviewType = payload.InterviewType,
                                    InterviewerEmployeeId = Schedule,
                                    ModifiedDate = DateTime.Now,
                                    ModifiedBy = _currentUser.GetFullname(),
                                    CompanyId = companyId,
                                    JobApplicantionId = applinbcantApplication.Id,
                                    HireStageId = applinbcantApplication.HireStageId,
                                    SubhireStageId = applinbcantApplication.SubhireStageId,
                                    IsShared = false,
                                    JobTitle = applinbcantApplication.Job.JobTitle,
                                    Status = InterViewStatus.Open,
                                    JobApplication = applinbcantApplication,
                                    CreatedDate = DateTime.Now,
                                    IsActive = true
                                };

                                _dbContext.JobScheduleInterviews.Add(addMember);
                                await _dbContext.SaveChangesAsync();
                            }

                            if (currentStage.EmailTemplateId != null)
                            {
                                var meesage = await SendMessageByTemplate(currentStage.EmailTemplateId.ToString(), applinbcantApplication.JobApplicant.UserId.ToString(), applinbcantApplication.JobApplicant.Email.Trim());
                            }

                            return ResponseModel<string>.Success("Your record was saved successfully");
                        }
                        else
                        {
                            return ResponseModel<string>.Failure("Because of the application status, application cannot be scheduled for an interview.");
                        }
                    }

                }
                else
                {
                    return ResponseModel<string>.Failure("Job Applicant Application Not Found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(AddUpdateScheduleJobInterviews));
                return ResponseModel<string>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<List<JobApplicationDto>>> GetJobApplicationByApplicant(Guid ApplicantID)
        {
            try
            {
                var result = await _dbContext.JobApplications.Where(x => x.JobApplicantId == ApplicantID && x.IsDeleted == false)
                        .Select(x => new JobApplicationDto
                        {

                        }).ToListAsync();

                return ResponseModel<List<JobApplicationDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(GetJobApplicationByApplicant));
                return ResponseModel<List<JobApplicationDto>>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<string>> DeleteJobApplication(Guid id)
        {
            try
            {
                var item = await _dbContext.JobApplications.FindAsync(id);
                if (item == null || item.Id == Guid.Empty)
                {
                    return ResponseModel<string>.Failure("Job Applicant Application Not Found");
                }

                item.IsDeleted = true;
                item.ModifiedBy = _currentUser.GetFullname();
                item.ModifiedDate = DateTime.Now;


                _jobApplicationRepository.Update(item);
                await _jobApplicationRepository.SaveChangesAsync();

                return ResponseModel<string>.Success("Your record was deleted");

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(DeleteJobApplication));
                return ResponseModel<string>.Exception("Exception error " + ex.Message);
            }
        }
        private bool CheckIfApplicatJobApplicationStatusExist(Guid jobApplicationId, Guid? hireStageId)
        {
            var histdedupChk = _dbContext.JobApplicationStageHistories
                            .Where(h => h.JobApplicationId == jobApplicationId
                            && h.StageId == hireStageId
                            && h.CompanyId == companyId)
                            .FirstOrDefault();

            return histdedupChk != null ? true : false;
        }

        private bool CheckIfApplicatJobApplicationSubStatusExist(Guid jobApplicationId, Guid? hireStageId, Guid? subStageId)
        {
            var histdedupChk = _dbContext.JobApplicationStageHistories
                            .Where(h => h.JobApplicationId == jobApplicationId
                            && h.StageId == hireStageId
                            && h.SubStageId == subStageId
                            && h.CompanyId == companyId)
                            .FirstOrDefault();

            return histdedupChk != null ? true : false;
        }

        //All Hired Applicants Count
        public async Task<ResponseModel<int>> FetchAllHiredApplicantsCount()
        {
            try
            {
                var result = await _dbContext.JobApplications.CountAsync(p => p.IsDeleted == false && p.IsHired && p.CompanyId == companyId);

                if (result > 0)
                {
                    return ResponseModel<int>.Success(result);
                }
                else
                {
                    return ResponseModel<int>.Success(result, "0");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<int>.Exception(ex.Message);
            }
        }
        //All Rejected Offers Count
        public async Task<ResponseModel<int>> FetchAllRejectedOffersCount()
        {
            try
            {
                var result = await _dbContext.JobApplications.Include(x => x.Stage)
                     .CountAsync(p => p.IsDeleted == false && p.Stage.StageName.ToLower().Contains("Rejected") && p.CompanyId == companyId);

                if (result > 0)
                {
                    return ResponseModel<int>.Success(result);
                }
                else
                {
                    return ResponseModel<int>.Failure("0");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<int>.Exception(ex.Message);
            }
        }
        public async Task<ResponseModel<int>> NumberOfApplicationsFromJobsPosted()
        {
            try
            {
                var result = await _dbContext.JobApplications.CountAsync(p => p.IsDeleted == false && p.CompanyId == companyId);

                if (result > 0)
                {
                    return ResponseModel<int>.Success(result);
                }
                else
                {
                    return ResponseModel<int>.Failure("0");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<int>.Exception(ex.Message);
            }
        }
        public async Task<ResponseModel<int>> NumberOfJobsPosted()
        {
            try
            {
                var result = await _dbContext.Jobs
                        .Where(jr => (jr.JobPostStatus == JobPostStatus.Post && jr.CompanyId == companyId && !jr.IsDeleted)
                        || (jr.JobPostStatus == JobPostStatus.Schedule && (jr.SchedulePostDate ?? DateTime.MinValue) <= DateTime.Now && jr.CompanyId == companyId && !jr.IsDeleted) && jr.CompanyId == companyId && !jr.IsDeleted)
                                .CountAsync();
                //await _dbContext.JobApplications.Include(x => x.Job)
                //    .CountAsync(p => p.IsDeleted == false && p.CompanyId == companyId && p.Job.JobPostStatus == JobPostStatus.Post);

                if (result > 0)
                {
                    return ResponseModel<int>.Success(result);
                }
                else
                {
                    return ResponseModel<int>.Failure("0");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<int>.Exception(ex.Message);
            }
        }
        public async Task<ResponseModel<CustomPagination<List<JobListDto>>>> FetchJobApplicationByPostingStatus(JobListFilterDto filter)
        {
            try
            {

                var departmentlist = await GrpcDepartmentList();

                var query = new List<Job>();

                if (filter.JobPostStatus == JobPostStatus.Post)
                {
                    query = await _dbContext.Jobs
                        .Where(jr => (jr.JobPostStatus == filter.JobPostStatus && jr.CompanyId == companyId && !jr.IsDeleted)
                        || (jr.JobPostStatus == JobPostStatus.Schedule && (jr.SchedulePostDate ?? DateTime.MinValue) <= DateTime.Now && jr.CompanyId == companyId && !jr.IsDeleted) && jr.CompanyId == companyId && !jr.IsDeleted)
                                .ToListAsync();
                }

                if (filter.JobPostStatus == JobPostStatus.Schedule)
                {
                    query = await _dbContext.Jobs.Where(jr => (jr.JobPostStatus == filter.JobPostStatus && jr.CompanyId == companyId && !jr.IsDeleted)
                                                    && (jr.SchedulePostDate ?? DateTime.MinValue) > DateTime.Now && jr.CompanyId == companyId && !jr.IsDeleted)
                        .ToListAsync();
                }

                //if (filter.JobPostStatus == JobPostStatus.Schedule)
                //{
                //    query = await _dbContext.Jobs.Where(jr => jr.JobPostStatus == filter.JobPostStatus && jr.CompanyId == companyId && !jr.IsDeleted && (jr.SchedulePostDate ?? DateTime.MinValue).TimeOfDay > DateTime.Now.TimeOfDay)
                //        .ToListAsync();
                //}

                if (filter.JobPostStatus == JobPostStatus.Draft)
                {
                    query = await _dbContext.Jobs.Where(jr => jr.JobPostStatus == filter.JobPostStatus && jr.CompanyId == companyId && !jr.IsDeleted).ToListAsync();
                }

                // ((currentDate.Date >= (jr.PostValidityFrom ?? DateTime.MinValue).Date && currentDate.Date <= (jr.PostValidityFrom ?? DateTime.MinValue).Date)

                DateTime currentDate = DateTime.Now;
                var result = query
                    .Select(jr => new JobListDto()
                    {
                        JobId = jr.Id,
                        Status = jr.PostValidityTo.HasValue ? (jr.PostValidityTo < DateTime.Now ? JobStatus.Closed.GetDescription() : JobStatus.Open.GetDescription()) : JobStatus.Open.GetDescription(),
                        DatePosted = jr.DatePosted != null ? jr.DatePosted : jr.SchedulePostDate,
                        Department = jr.DepartmentId.ToString(),
                        JobTitle = jr.JobTitle,
                        ScheduleDate = jr.SchedulePostDate,
                        NumberOfApplicant = _dbContext.JobApplications.Count(a => a.JobId == jr.Id),
                        SalaryRange = jr.SalaryRange,
                        EmploymentType = jr.EmploymentType,
                        EmploymentTypeText = jr.EmploymentType.GetDescription(),
                        CompanyId = jr.CompanyId,
                        JobPostStatus = jr.JobPostStatus,
                        JobStatus = jr.PostValidityTo.HasValue ? (jr.PostValidityTo < DateTime.Now ? JobStatus.Closed : JobStatus.Open) : JobStatus.Open,
                        JobStatusText = jr.JobPostStatus.GetDescription(),
                        DateCreated = jr.CreatedDate,
                        CountryId = jr.CountryId,
                        StateId = jr.StateId,
                        JobDescription = jr.Description,
                        Experience = jr.Experience,
                        SalaryRangeFrom = jr.SalaryRangeFrom,
                        SalaryRangeTo = jr.SalaryRangeTo,
                        DepartmentName = GetCurrentDepartmentName(departmentlist, jr.DepartmentId)
                    })
                    .ToList();

                if (filter.SalaryRangeFrom.HasValue && filter.SalaryRangeTo.HasValue)
                {
                    result = result.Where(x => x.SalaryRangeFrom >= filter.SalaryRangeFrom && x.SalaryRangeTo <= filter.SalaryRangeTo).ToList();
                }

                if (filter.EmploymentType != null)
                {
                    result = result.Where(x => x.EmploymentType == filter.EmploymentType).ToList();
                }

                if (!string.IsNullOrEmpty(filter.JobTitle))
                {
                    result = result.Where(x => x.JobTitle.ToLower().Trim().Contains(filter.JobTitle.ToLower().Trim())).ToList();
                }

                if (filter.DatePosted != null)
                {
                    result = result.Where(x => x.DateCreated == filter.DatePosted).ToList();
                }

                if (filter.DepartmentId != null && filter.DepartmentId != Guid.Empty)
                {
                    result = result.Where(x => x.Department == filter.DepartmentId.ToString()).ToList();
                }

                if (!string.IsNullOrEmpty(filter.search))
                {
                    result = result.Where(x => x.JobTitle.ToLower().Contains(filter.search.ToLower()) || x.SalaryRange.Contains(filter.search)
                    || x.DepartmentName.Contains(filter.search) || x.JobDescription.ToLower().Contains(filter.search.ToLower())).ToList();
                }

                if (filter.Status.HasValue && filter.Status == JobStatus.Closed)
                {
                    result = result.Where(jr => jr.Status == "Closed").ToList();
                }

                if (filter.Status.HasValue && filter.Status == JobStatus.Open)
                {
                    result = result.Where(jr => jr.Status == "Open").ToList();
                }

                if (!string.IsNullOrEmpty(filter.JobExperienceLevel))
                {
                    result = result.Where(jr => jr.Experience.Equals(filter.JobExperienceLevel)).ToList();
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {

                    result = result.Where(job => job.DatePosted >= filter.StartDate && job.DatePosted <= filter.EndDate).ToList();
                }

                var response = new CustomPagination<List<JobListDto>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count
                };

                return ResponseModel<CustomPagination<List<JobListDto>>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while fetching job list: {ex.Message}", nameof(FetchApplicantDropOffStage));
                return ResponseModel<CustomPagination<List<JobListDto>>>.Exception("Exception error " + ex.Message);
            }

        }
        public async Task<List<DepartmentDataFromGRPC>> GrpcDepartmentList()
        {

            List<DepartmentDataFromGRPC> list = new List<DepartmentDataFromGRPC>();

            var channel = GrpcChannel.ForAddress("https://hracctgrpc.azurewebsites.net/");
            var client = new Companyy.CompanyyClient(channel);
            var _cpId = companyId.ToString();
            try
            {
                var reply = client.GetAllDepartment(new GetDepartmentsRequest { CompanyId = _cpId });

                if (reply != null)
                {
                    var result = reply.DeptList.Select(x => new DepartmentDataFromGRPC
                    {
                        Id = Guid.Parse(x.Id),
                        Name = x.Name,
                        Code = x.Code,
                        HeadOfDepartment = x.HeadOfDepartment,
                        HODName = x.HODName,
                    }).ToList();

                    return result;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return list;
        }
        public static string GetCurrentDepartmentName(List<DepartmentDataFromGRPC> departments, Guid departmentId)
        {
            var result = departments.Where(x => x.Id == departmentId).FirstOrDefault();
            return result != null ? result.Name : "";
        }
        public async Task<ResponseModel<CustomPagination<List<SupportOfficerApplication>>>> GetSupportOfficerApplication(SupportOfficerApplicationFilter filter)
        {
            try
            {

                var result = await _dbContext.JobApplications
                                        .Include(x => x.Job)
                                        .Include(x => x.JobApplicant)
                                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false)
                                        .Select(x => new SupportOfficerApplication
                                        {
                                            Id = x.Id,
                                            Email = x.JobApplicant.Email,
                                            FullName = $"{x.JobApplicant.LastName} {x.JobApplicant.FirstName}",
                                            JobTitle = x.Job.JobTitle,
                                            Status = x.ApplicationStatus,
                                            EvaluationScore = x.ScoreCardValue,
                                            DateApplied = x.DateApplied,
                                        }).ToListAsync();


                if (!string.IsNullOrEmpty(filter.JobTitle))
                {
                    result = result.Where(n => n.JobTitle.Equals(filter.JobTitle, StringComparison.CurrentCultureIgnoreCase)).ToList();
                }


                if (result != null)
                {

                    CustomPagination<List<SupportOfficerApplication>> response = new CustomPagination<List<SupportOfficerApplication>>()
                    {
                        modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        TotalCount = result.Count
                    };

                    foreach (var item in response.modelresult)
                    {
                        item.QuizScore = GetQuizScore(item.Id).Result;
                    }

                    return ResponseModel<CustomPagination<List<SupportOfficerApplication>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<SupportOfficerApplication>>>.Failure("Not Found.");
                }
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(SupportOfficerApplication));
                return ResponseModel<CustomPagination<List<SupportOfficerApplication>>>.Exception("Exception error " + ex.Message);
            }

        }
        private async Task<int?> GetQuizScore(Guid applicantionId)
        {
            int result = -1;

            var quiScoreawait = await _dbContext.ApplicantQuizRecords
                .Where(x => x.JobApplicationId == applicantionId).FirstOrDefaultAsync();

            if (quiScoreawait != null)
            {
                result = quiScoreawait.Totalscore != null ? quiScoreawait.Totalscore.Value : -1;
            }
            return result;
        }
        public async Task<ResponseModel<CustomPagination<List<ApplicantSelection>>>> GetApplicantSelection(ApplicantSelectionFilter filter)
        {
            try
            {

                var result = await _dbContext.JobApplications
                                        .Include(x => x.Job)
                                        .Include(x => x.JobApplicant)
                                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false)
                                        .Select(x => new ApplicantSelection
                                        {
                                            ApplicationId = x.Id,
                                            ApplicantId = x.JobApplicantId,
                                            FullName = $"{x.JobApplicant.LastName} {x.JobApplicant.FirstName}",
                                            JobTitle = x.Job.JobTitle,
                                            JobPostText = x.Job.JobPostStatus.GetDescription(),
                                            DateApplied = x.DateApplied,
                                            JobPost = x.Job.JobPostStatus,

                                        }).OrderByDescending(x => x.DateApplied).ToListAsync();

                if (!string.IsNullOrEmpty(filter.JobTitle))
                {
                    result = result.Where(n => n.JobTitle.Contains(filter.JobTitle.ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                }

                if (!string.IsNullOrEmpty(filter.Search))
                {
                    result = result.Where(n => n.JobTitle.Contains(filter.Search, StringComparison.CurrentCultureIgnoreCase)
                        || n.FullName.Contains(filter.Search, StringComparison.CurrentCultureIgnoreCase)
                        || n.JobPostText.Contains(filter.Search, StringComparison.CurrentCultureIgnoreCase)
                    ).ToList();
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    result = result.Where(n => n.DateApplied >= filter.StartDate
                    && n.DateApplied <= filter.EndDate).ToList();
                }


                if (result != null)
                {

                    CustomPagination<List<ApplicantSelection>> response = new CustomPagination<List<ApplicantSelection>>()
                    {
                        modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        TotalCount = result.Count
                    };


                    return ResponseModel<CustomPagination<List<ApplicantSelection>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<ApplicantSelection>>>.Failure("Not Found.");
                }
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(SupportOfficerApplication));
                return ResponseModel<CustomPagination<List<ApplicantSelection>>>.Exception("Exception error " + ex.Message);
            }

        }

        public async Task<ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>> GetCandidateApplicantSelection(CandidateApplicantSelectionFilter filter)
        {
            try
            {
                List<CandidateApplicantSelection> result = new List<CandidateApplicantSelection>();

                List<JobApplicationListingDTO> applicationByJobTitle = new List<JobApplicationListingDTO>();

                //Get EmployeeId by the userToken

                var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());

                var result2 = new List<JobScheduleInterview>();

                if (employeeId == null)
                {
                    return ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>.Failure("Hr/Admin not found");
                }
                var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

                if (employeeId.EmployeeId != null)
                {
                    result2 =

                        //(from jInterview in _dbContext.JobScheduleInterviews  
                        //       join job in _dbContext.Jobs on jInterview.jo)

                        //from e in employees
                        //join d in departments on e.Department_Id equals d.DepartmentId into table1


                        _dbContext.JobScheduleInterviews
                                      .Include(x => x.HireStage)
                                      .Include(x => x.SubhireStage)
                                      .Include(x => x.JobApplication)
                                      .ThenInclude(x => x.Job)
                                      .Include(x => x.JobApplication)
                                      .ThenInclude(x => x.JobApplicant)
                                      .Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.InterviewerEmployeeId == interviewerEmployeeId)
                                      .AsQueryable().OrderByDescending(x => x.JobApplication.DateApplied).ToList();
                }




                //else
                //{
                //    result2 = _dbContext.JobApplications
                //                            .Include(x => x.JobApplicant)
                //                            .Include(x => x.Stage)
                //                            .Include(x => x.Job)
                //                            .ThenInclude(x => x.JobReviewers)
                //                            .Where(x => x.CompanyId == companyId && x.IsDeleted == false)
                //                            .AsQueryable().OrderByDescending(x => x.DateApplied).ToList();
                //}

                //var result2 = _dbContext.JobScheduleInterviews
                //                      .Include(x => x.JobApplication)
                //                      .ThenInclude(x => x.Job)
                //                      .Include(x => x.JobApplication)
                //                      .ThenInclude(x => x.Stage)
                //                      .Include(x => x.JobApplication)
                //                      .ThenInclude(x => x.JobApplicant)
                //                      .Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.InterviewerEmployeeId == interviewerEmployeeId).AsQueryable().OrderByDescending(x => x.JobApplication.DateApplied).ToList();

                //OrderByDescending(x => x.DateApplied)

                result = result2.Select(f => new CandidateApplicantSelection()
                {

                    ApplicationId = f.JobApplication.Id,
                    ApplicantId = f.JobApplication.JobApplicantId,
                    JobId = f.JobApplication.JobId,
                    Email = f.JobApplication.JobApplicant.Email,
                    FullName = $"{f.JobApplication.JobApplicant.LastName} {f.JobApplication.JobApplicant.FirstName}",
                    JobTitle = f.JobTitle,
                    Status = f.JobApplication.ApplicationStatus,
                    EvaluationScore = f.JobApplication.ScoreCardValue,
                    DateApplied = f.JobApplication.DateApplied,
                    JobPost = f.JobApplication.Job.JobPostStatus,
                    JobPostText = f.JobApplication.Job.JobPostStatus.GetDescription(),
                    StageName = f.HireStage.StageName
                }).DistinctBy(x => x.ApplicationId).ToList();


                foreach (var item in result)
                {
                    item.QuizScore = GetQuizScore(item.ApplicationId).Result;
                }

                if (!string.IsNullOrEmpty(filter.JobTitle))
                {
                    result = result.Where(n => n.JobTitle.ToLower().Contains(filter.JobTitle.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(filter.Search))
                {
                    result = result.Where(n => n.JobTitle.ToLower().Contains(filter.Search.ToLower())
                        || n.FullName.ToLower().Contains(filter.Search.ToLower())
                        || n.Status.ToLower().Contains(filter.Search.ToLower())
                    ).ToList();
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    result = result.Where(n => n.DateApplied.Date >= filter.StartDate.Value.Date
                    && n.DateApplied.Date <= filter.EndDate.Value.Date).ToList();
                }


                if (filter.RangeQuizStart.HasValue && filter.RangeQuizEndEnd.HasValue)
                {
                    result = result.Where(n => n.QuizScore >= filter.RangeQuizStart
                    && n.QuizScore <= filter.RangeQuizEndEnd).ToList();
                }

                if (filter.RangeScoreCardStart.HasValue && filter.RangeScoreCardEnd.HasValue)
                {
                    result = result.Where(n => n.EvaluationScore >= filter.RangeScoreCardStart
                    && n.EvaluationScore <= filter.RangeScoreCardEnd).ToList();
                }

                result = result.Where(x => !(x.StageName.Contains("Rejected"))).ToList();

                if (result != null)
                {

                    CustomPagination<List<CandidateApplicantSelection>> response = new CustomPagination<List<CandidateApplicantSelection>>()
                    {
                        modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        TotalCount = result.Count
                    };



                    return ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>.Failure("Not Found.");
                }
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(SupportOfficerApplication));
                return ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>.Exception("Exception error " + ex.Message);
            }

        }

        private async Task<string> StageNameById(Guid? stageId)
        {
            var stage = await _dbContext.Stages.FirstOrDefaultAsync(x => x.Id == stageId && x.CompanyId == companyId);
            return stage != null ? stage.StageName : "";
        }

        private async Task<string> SubStageNameById(Guid? SubstageId)
        {
            var stage = await _dbContext.SubStages.FirstOrDefaultAsync(x => x.Id == SubstageId && x.CompanyId == x.CompanyId);
            return stage != null ? stage.SubStageName : "";
        }

        public async Task<ResponseModel<List<JobApplicationStageHistoryModel>>> JobApplicationStageHistory(Guid jobApplicationId)
        {
            try
            {
                var result = await _dbContext.JobApplicationStageHistories
                    .Where(x => x.JobApplicationId == jobApplicationId && x.CompanyId == companyId)
                    .Select(x => new JobApplicationStageHistoryModel
                    {
                        CompanyId = x.CompanyId,
                        JobApplicationId = x.JobApplicationId,
                        StageId = x.StageId,
                        StatusDescription = x.StatusDescription,
                        SubStageId = x.SubStageId,
                        RecruitmentActionDate = x.CreatedDate
                    }).OrderByDescending(x => x.RecruitmentActionDate).ToListAsync();

                if (result != null)
                {
                    return ResponseModel<List<JobApplicationStageHistoryModel>>.Success(result);
                }
                else
                {
                    return ResponseModel<List<JobApplicationStageHistoryModel>>.Failure("Result not found");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<List<JobApplicationStageHistoryModel>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<int>> InvitedApplicants()
        {
            var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());


            if (employeeId == null)
            {
                return ResponseModel<int>.Failure("Hr/Admin not found");
            }

            var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

            var numberOfScheduleInterview = await _dbContext.JobScheduleInterviews
                                      .Include(x => x.HireStage)
                                      .Include(x => x.JobApplication)
                                      .Where(x => x.InterviewerEmployeeId == interviewerEmployeeId
                                      && x.HireStage.StageName.Contains("Interview")).ToListAsync();

            numberOfScheduleInterview = numberOfScheduleInterview.DistinctBy(x => x.JobApplicantionId).ToList();

            var total = numberOfScheduleInterview.Count();

            if (total > 0)
            {
                return ResponseModel<int>.Success(total);
            }
            else
            {
                return ResponseModel<int>.Success(0, "No result Found");
            }
        }

        public async Task<ResponseModel<int>> NumberOfApplicantInterviewed()
        {
            var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());


            if (employeeId == null)
            {
                return ResponseModel<int>.Failure("Hr/Admin not found");
            }

            var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

            var jobApplication = await _dbContext.JobScheduleInterviews
                .Where(x => x.Status == InterViewStatus.Closed && x.InterviewerEmployeeId == interviewerEmployeeId).ToListAsync();

            if (jobApplication != null)
            {
                return ResponseModel<int>.Success(jobApplication.Count());
            }
            else
            {
                return ResponseModel<int>.Success(0, "No result Found");
            }

        }

        public async Task<ResponseModel<int>> Pendinginterview()
        {
            var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());


            if (employeeId == null)
            {
                return ResponseModel<int>.Failure("Hr/Admin not found");
            }

            var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

            var jobApplication = await _dbContext.JobScheduleInterviews
                 .Where(x => x.Status == InterViewStatus.Open && x.CreatedDate < DateTime.Now && x.InterviewerEmployeeId == interviewerEmployeeId).ToListAsync();

            if (jobApplication != null)
            {
                return ResponseModel<int>.Success(jobApplication.Count());
            }
            else
            {
                return ResponseModel<int>.Success(0, "No result Found");
            }
        }

        public async Task<ResponseModel<List<DashBoardListGraphData>>> SuccessfulApplicants(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
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

            var jobApplicants = await _dbContext.JobApplications
                .Include(x => x.Job)
                 .Where(x => x.CompanyId == companyId && x.IsDeleted == false
                          && x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.IsHired == true)
                 .ToListAsync();

            var successfulApplication = jobApplicants.GroupBy(x => x.Job.JobTitle)
                                .Select(x => new DashBoardListGraphData
                                {
                                    Label = x.Key,
                                    Data = x.Count()
                                }).ToList();

            if (successfulApplication.Count > 0)
            {
                return ResponseModel<List<DashBoardListGraphData>>.Success(successfulApplication);
            }
            else
            {
                return ResponseModel<List<DashBoardListGraphData>>.Success(successfulApplication, "No result avialable");
            }

        }


        public async Task<ResponseModel<List<DashBoardListGraphData>>> ApplicantsDropOffStages(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null)
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

            var jobApplicants = await _dbContext.JobApplications
                .Include(x => x.Job)
                .Include(x => x.Stage)
                 .Where(x => x.CompanyId == companyId && x.IsDeleted == false
                          && x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.IsHired == false)
                 .ToListAsync();

            var dropOffStagApplication = jobApplicants.GroupBy(x => x.Stage.StageName)
                                .Select(x => new DashBoardListGraphData
                                {
                                    Label = x.Key,
                                    Data = x.Count()
                                }).ToList();

            if (dropOffStagApplication.Count > 0)
            {
                return ResponseModel<List<DashBoardListGraphData>>.Success(dropOffStagApplication);
            }
            else
            {
                return ResponseModel<List<DashBoardListGraphData>>.Success(dropOffStagApplication, "No result avialable");
            }

        }


        public async Task<ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>> FetchUpComingJobInterviewerList(PaginationRequest payload)
        {
            try
            {
                /// Fetch all record 

                var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());


                if (employeeId == null)
                {
                    return ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>.Failure("Hr/Admin not found");
                }

                var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);

                var data = await (from a in _dbContext.JobScheduleInterviews
                                  from ap in _dbContext.JobApplications.Where(x => x.Id == a.JobApplicantionId)
                                  from apf in _dbContext.ApplicantProfiles.Where(x => x.Id == a.JobApplication.JobApplicantId).DefaultIfEmpty()
                                  where a.IsActive == true && a.CompanyId == companyId
                                  && a.IsDeleted == false && a.InterviewerEmployeeId == interviewerEmployeeId
                                  select new ApplicantInterviewScheduleDTO()
                                  {
                                      JobApplicantId = apf.Id,
                                      JobApplicantName = apf.FirstName + " " + apf.LastName,
                                      InterviewerEmployeeId = a.InterviewerEmployeeId,
                                      JobRoleName = a.JobTitle,
                                      InterviewDate = a.InterviewDate,
                                      InterviewTime = a.InterviewTime,
                                      Duration = a.Duration,
                                      JobApplicantionId = a.JobApplicantionId,
                                      IsShared = a.IsShared,
                                      HireStageId = a.HireStageId,
                                      SubhireStageId = a.SubhireStageId,
                                      InterviewType = a.InterviewType
                                  }).AsQueryable().ToListAsync();


                if (data != null)
                {
                    CustomPagination<List<ApplicantInterviewScheduleDTO>> response = new CustomPagination<List<ApplicantInterviewScheduleDTO>>()
                    {
                        modelresult = data.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                        pageNumber = payload.PageNumber,
                        pageSize = payload.PageSize,
                        TotalCount = data.Count()
                    };

                    return ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>.Failure("Not Found.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while onboarding new employee: {ex.Message}", nameof(GetInterviwerforSharing));
                return ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>.Exception("Exception error " + ex.Message);
            }
        }


        public async Task<ResponseModel<List<CalenderInterviewDay>>> InterviewCalender()
        {
            // Determine the first day of the current month
            DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // Determine the last day of the current month
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Create a list of interview dates for the current month
            List<DateTime> interviewDates = await _dbContext.JobScheduleInterviews
                .Select(interview => interview.InterviewDate.Date).Distinct()
                .Where(date => date >= firstDayOfMonth && date <= lastDayOfMonth)
                .ToListAsync();

            // Create a list of interview days for the current month with indicators for scheduled interviews
            List<CalenderInterviewDay> interviewDays = new List<CalenderInterviewDay>();
            for (int i = 1; i <= lastDayOfMonth.Day; i++)
            {
                CalenderInterviewDay interviewDay = new CalenderInterviewDay { DayOfMonth = i, HasInterview = interviewDates.Contains(new DateTime(DateTime.Today.Year, DateTime.Today.Month, i)) };
                interviewDays.Add(interviewDay);
            }

            if (interviewDates.Count > 0)
            {
                return ResponseModel<List<CalenderInterviewDay>>.Success(interviewDays, "No result avialable");
            }
            return ResponseModel<List<CalenderInterviewDay>>.Success(interviewDays, "No result avialable");
        }

        public async Task<ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>> GetAllUpComingTasks(PaginationRequest payload)
        {
            try
            {

                var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());


                if (employeeId == null)
                {
                    return ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>.Failure("Hr/Admin not found");
                }

                var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);
                ///// Fetch all record 
                var result = new List<ReveiwerUpComingTaskDto>();
                result = await _dbContext.ReveiwerUpComingTasks.Where(x => x.CompanyId == companyId && x.InterviewerEmployeeId == interviewerEmployeeId)
                                .Select(x => new ReveiwerUpComingTaskDto
                                {
                                    TaskDescription = x.UpComingTaskDescription,
                                    UpComingTaskDate = x.UpComingTaskDate,
                                    FormatedCreated = GetDateLabel(x.UpComingTaskDate),
                                    DateCreated = x.CreatedDate
                                }).ToListAsync();

                if (result != null && result.Count() > 0)
                {

                    CustomPagination<List<ReveiwerUpComingTaskDto>> response = new CustomPagination<List<ReveiwerUpComingTaskDto>>()
                    {
                        modelresult = result.Skip((payload.PageNumber - 1) * payload.PageSize).Take(payload.PageSize).ToList(),
                        pageNumber = payload.PageNumber,
                        pageSize = payload.PageSize,
                        TotalCount = result.Count
                    };

                    return ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>.Failure("No upcoming tasks");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>.Exception(ex.Message);
            }
        }


        public async Task<ResponseModel<DocumentResponseDTO>> DownloadJobApplicantionCV(Guid applicantionId)
        {
            var response = new DocumentResponseDTO();
            try
            {
                var fileDetails = await _dbContext.JobApplicantCVBanks.Where(c => c.ApplicantionId == applicantionId && c.CompanyId == companyId).FirstOrDefaultAsync();

                if (fileDetails == null)
                    return ResponseModel<DocumentResponseDTO>.Failure("null");

                var retUrl = fileDetails.CVFileUrl;
                var retFileName = fileDetails.FileName;

                var fileName = retFileName;
                var filePath = retUrl;//new Uri(filePath);

                response.FilePath = filePath;
                response.FileName = fileName;
                return ResponseModel<DocumentResponseDTO>.Success(response);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant application CV: {ex.Message}", nameof(DownloadJobApplicantionCV));
                return ResponseModel<DocumentResponseDTO>.Failure("Exception error " + ex.Message);
            }
        }


        public async Task<ResponseModel<string>> AddApplicantobWordSerchAsync(CreateJobWordDto request)
        {
            try
            {

                var checkNameExist = await _dbContext.ApplicantJobSearchKeywords.FirstOrDefaultAsync(x => x.Jobkeyword == request.AppKeyword && x.CompanyId == companyId && x.IsDeleted == false && x.ApplicantProfileId == request.applicantId);

                if (checkNameExist != null)
                {
                    return ResponseModel<string>.Failure($"{checkNameExist.Jobkeyword} already exists");
                }

                const int max = 10;
                var countKeyword = await _dbContext.ApplicantJobSearchKeywords.Where(x => x.ApplicantProfileId == request.applicantId).ToListAsync();
                if (countKeyword.Count() > max)
                {
                    return ResponseModel<string>.Failure($"Is only ten keyword that is allow");
                }

                var app = await _dbContext.ApplicantProfiles.Where(x => x.Id == request.applicantId).FirstOrDefaultAsync();

                if (app == null)
                {
                    return ResponseModel<string>.Failure($"applicant not found");
                }

                var record = new ApplicantJobSearchKeyword
                {
                    Id = SequentialGuid.Create(),
                    Jobkeyword = request.AppKeyword,
                    ApplicantProfileId = request.applicantId,
                    CompanyId = companyId,
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedByName = _currentUser.GetFullname(),
                    CreatedDate = DateTime.Now,
                    ApplicantProfile = app
                };
                await _dbContext.ApplicantJobSearchKeywords.AddAsync(record);
                await _dbContext.SaveChangesAsync();

                return ResponseModel<string>.Success("Successfully Created");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

                _logger.LogCritical($"Exception occured while creating certification record: {ex.Message}");
                return ResponseModel<string>.Failure("Exception error");
            }
        }


        public async Task<ResponseModel<string>> SaveJobApplicantScoreCard(SaveJobApplicantScoreCardDto payload)
        {

            try
            {
                var employeeId = await GetEmployeeData(_currentUser.GetUserId().ToString());

                if (employeeId == null)
                {
                    return ResponseModel<string>.Failure("Interviewer Not Found");
                }

                var jobApplication = await _dbContext.JobApplications.Where(p => p.Id == payload.JobApplicantionId).FirstOrDefaultAsync();

                if (jobApplication != null)
                {
                    var interviewerEmployeeId = Guid.Parse(employeeId.EmployeeId);


                    var scheduleInterviewer = await _dbContext.JobScheduleInterviews.Where(p => p.InterviewerEmployeeId == interviewerEmployeeId
                                        && p.JobApplicantionId == jobApplication.Id).FirstOrDefaultAsync();

                    if (scheduleInterviewer != null)
                    {

                        jobApplication.ScoreCardValue = payload.ScoreCardScore;

                        _dbContext.JobApplications.Update(jobApplication);
                        await _dbContext.SaveChangesAsync();

                        scheduleInterviewer.Status = InterViewStatus.Closed;
                        _dbContext.JobScheduleInterviews.Update(scheduleInterviewer);
                        await _dbContext.SaveChangesAsync();
                        //overAllAverageScore
                        return ResponseModel<string>.Success("Your record was updated successfully");
                    }
                    else
                    {
                        _logger.LogInformation("Interviewer Not Found");
                        return ResponseModel<string>.Failure("Interviewer Not Found");
                    }
                }
                else
                {
                    _logger.LogInformation("Job Application Not Found");
                    return ResponseModel<string>.Failure("Job Application Not Found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<string>.Failure("Job Application Not Found");
            }
        }


        public async Task<ResponseModel<string>> OfferLetterConfirmation(string onboardingId)
        {

            try
            {

                var onboarding = await _dbContext.RecruitmentApplicantOnboardings.Where(x => x.OnboardingId == onboardingId).FirstOrDefaultAsync();

                if (onboarding == null)
                {
                    return ResponseModel<string>.Failure("Onboarding data not found");
                }

                JobApplication? jobApplication = await _dbContext.JobApplications.Include(x => x.JobApplicant)
                    .Include(x => x.Job)
                    .Include(x => x.Stage)
                    .Where(x => x.Id == onboarding.JobApplicationId && x.IsDeleted == false &&
                    x.CompanyId == companyId).FirstOrDefaultAsync();

                if (jobApplication == null)
                {
                    return ResponseModel<string>.Failure("Job Application Not found");
                }


                var hiringStage = await _dbContext.Stages.Where(x => x.StageName.Contains("Hired")).FirstOrDefaultAsync();

                if (hiringStage == null)
                {
                    return ResponseModel<string>.Failure("Stage not found.");
                }

                jobApplication.HireStageId = hiringStage.Id;
                jobApplication.IsHired = true;
                jobApplication.IsInProgress = false;
                jobApplication.ApplicationStatus = hiringStage.StageName;
                jobApplication.ModifiedDate = DateTime.Now;
                jobApplication.ModifiedBy = _currentUser.GetUserEmail();
                //jobApplication.Stage = hiringStage;
                jobApplication.DateRecruited = DateTime.Now;


                _jobApplicationRepository.Update(jobApplication);
                // await _jobApplicationRepository.SaveChangesAsync();

                var jobApplicationStageHistory = new JobApplicationStageHistory()
                {

                    Id = SequentialGuid.Create(),
                    IsDeleted = false,
                    JobApplicationId = jobApplication.Id,
                    StatusDescription = $"Hired",
                    StageId = hiringStage.Id,
                    CompanyId = companyId,
                    CreatedBy = _currentUser.GetUserId(),
                    JobApplication = jobApplication,
                    Stage = hiringStage,
                    CreatedDate = DateTime.Now
                };

                _dbContext.JobApplicationStageHistories.Add(jobApplicationStageHistory);
                // _dbContext.SaveChanges();


                if (hiringStage.EmailTemplateId != null)
                {
                    EmailTemplateProcess emailTemplateProcess = new()
                    {
                        Id = SequentialGuid.Create(),
                        Description = "Recruitment Action",
                        Email = jobApplication.JobApplicant.Email.Trim(),
                        UserId = jobApplication.JobApplicant.UserId,
                        TemplateId = (Guid)hiringStage.EmailTemplateId,
                        Processed = false,
                        EmailSent = false,
                        CreatedBy = _currentUser.GetUserId(),
                        CreatedByName = _currentUser.GetFullname(),
                        CreatedDate = DateTime.Now
                    };

                    await _dbContext.EmailTemplateProcesses.AddAsync(emailTemplateProcess);
                    // await _dbContext.SaveChangesAsync();

                }
                await _dbContext.SaveChangesAsync();
                return ResponseModel<string>.Success("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResponseModel<string>.Failure("failed");
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
                case "This Month":
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                case "This Year":
                    return new DateTime(DateTime.Now.Year, 1, 1);
                default:
                    return new DateTime(DateTime.Now.Year, 1, 1);
            }
        }

        private async Task<string?> SendMessageByTemplate(string templateId, string userId, string email)
        {
            #region employee via grpc

            try
            {

                var commChannel = GrpcChannel.ForAddress("https://hrcommgrpc.azurewebsites.net/");
                var commClient = new Greeter.GreeterClient(commChannel);  //Companyy.CompanyyClient(commChannel);


                var employeeReply = await commClient.SendMessageByTemplateAsync(new SendMessageViaTemplateRequest()
                {
                    Emails = email,
                    TemplateId = templateId,
                    UserId = userId
                });

                if (employeeReply == null)
                {
                    _logger.LogCritical($"GRPC Email template to templateId: {templateId}  UserId: {userId} Email: {email} failed");
                    return null;
                }
                else
                {
                    _logger.LogInformation($"GRPC Email template to sent to: {templateId}  UserId: {userId} Email: {email} Response {employeeReply.Response}");
                    return employeeReply.Response;
                }
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"GRPC Error --> Exception occured when sending email to with templateId: {templateId}  UserId: {userId} Email: {email} {ex.Message}", nameof(SendMessageByTemplate));
                return null;
            }


            #endregion
        }
        private async Task<bool> SendQuizEmail(JobApplication jobApplication)
        {
            try
            {
                if (jobApplication != null)
                {
                    var quizId = jobApplication.Job.QuizId;

                    if (quizId.HasValue)
                    {
                        var quiz = await _dbContext.Quizzes.Where(x => x.Id == quizId.Value && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (quiz != null)
                        {
                            var applicantQuizRecord = new ApplicantQuizRecord()
                            {
                                Id = SequentialGuid.Create(),
                                QuizId = quizId.Value,
                                ApplicantProfileId = jobApplication.JobApplicantId,
                                Iscompleted = false,
                                Duration = 30,
                                CreatedBy = _currentUser.GetUserId(),
                                CompanyId = companyId,
                                CreatedByName = _currentUser.GetFullname(),
                                CreatedDate = DateTime.Now,
                                JobApplicationId = jobApplication.Id
                            };

                            _dbContext.ApplicantQuizRecords.Add(applicantQuizRecord);
                            await _dbContext.SaveChangesAsync();

                            var quizUrl = $"{_quizSettings.QuizUrl}?id={applicantQuizRecord.Id}";

                            string buildContent = $"Hi {jobApplication.JobApplicant.FirstName}, " +
           $"<p>Please follow this Link to complete application quiz</p>" +
           $"<div class = 'row'>" +
           $"<div class = 'col-md-12 col-sm-12'>" +
           $"<a href = '{quizUrl}'>" +
           $"<button class = 'btn btn-primary'>Start Quiz</button>" +
           $"</a>" +
           $"</div>" +
           $"</div>";
                            var mailRequest = new MailRequest(
                                new List<string> { jobApplication.JobApplicant.Email },
                                "Invitation for job application quiz", MailTemplateHelper.GenerateMailContent(buildContent, "base-layout.html", "Quiz"));
                            //send mail

                            await _mailService.SendAsync(mailRequest);
                        }
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending Quiz Email to applicant {ex.Message}");
                return false;
            }
        }

        private async Task<EmployeeDetails?> GetEmployeeData(string userId)
        {
            #region employee profile picture via grpc

            try
            {
                var employee = new EmployeeDetails();

                var channel = GrpcChannel.ForAddress("https://hrempgrpc.azurewebsites.net/");
                var client = new EmployeeGrpcService.EmployeeGrpcServiceClient(channel);



                var reply = await client.GetEmployeeDetailsByUserAsync(
                    new EmployeeDetailsRequestByUserId()
                    {
                        UserId = userId
                    });

                if (reply == null)
                {
                    return employee;
                }
                else
                {
                    employee.FullName = reply.FullName;
                    employee.Phonenumber = reply.Phonenumber;
                    employee.Email = reply.Email;
                    employee.ProfileImage = reply.ProfileImage;
                    employee.EmployeeId = reply.EmployeeId;

                    return employee;
                }
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while getting employee Record: {ex.Message}", nameof(GetEmployeeData));
                return null;
            }


            #endregion
        }


        private async Task<OnboardRecruitmentResponse?> ApplicantOnboarding(Guid applicationId)
        {
            #region employee profile picture via grpc

            try
            {
                OnboardRecruitmentRequest request = new();

                OnboardingData? applicantRecord = await GetApplicantOnboardingPersonnalInfo(applicationId);
                if (applicantRecord.JobApplicantOnboarding == null)
                {
                    return null;
                }

                request = applicantRecord.JobApplicantOnboarding;

                var onboarding = new OnboardRecruitmentResponse();

                var channel = GrpcChannel.ForAddress("https://hrempgrpc.azurewebsites.net/");
                var client = new EmployeeGrpcService.EmployeeGrpcServiceClient(channel);

                var reply = await client.OnboardEmployeeViaRecruitmentAsync(
                    new OnboardViaRecruitmentRequest()
                    {
                        CompanyId = request.CompanyId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        MiddleName = request.MiddleName,
                        PhoneNumber = request.PhoneNumber,
                        DateOfBirth = request.PhoneNumber,
                        EmailAddress = request.PhoneNumber,
                        Gender = request.Gender,
                        MaritalStatus = request.MaritalStatus,
                        PersonalEmail = request.PersonalEmail,
                        ReligionId = request.ReligionId,
                        FieldOfStudy = request.FieldOfStudy,
                        Degree = request.Degree,
                        DateOfCompletion = request.DateOfCompletion,
                        CGPA = request.CGPA,
                        InstitutionId = request.InstitutionId,
                        NextOfKinName = request.NextOfKinName,
                        NextOfKinRelationship = request.NextOfKinRelationship,
                        NextOfKinPhoneNumber = request.NextOfKinPhoneNumber,
                        NextOfKinAddress = request.NextOfKinAddress,
                        Address = request.Address,
                        State = request.State,
                        Country = request.Country,
                        JobTitleId = request.JobTitleId,
                        JobTitleName = request.JobTitleName,
                        HiredDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(request.HiredDate),
                        SalaryPerAnnum = request.SalaryPerAnnum,
                        Designation = request.Designation,
                        EmployeeType = request.EmployeeType,
                        DepartmentId = request.DepartmentId,
                        DepartmentName = request.DepartmentName,
                        ReportingManager = request.ReportingManager,
                        ReportingManagerId = request.ReportingManagerId,
                        CompanyLocationId = request.CompanyLocationId,
                        CompanyLocationName = request.CompanyLocationName,
                        OfferLetterExpiration = request.OfferLetterExpiration,
                    });

                if (reply == null)
                {
                    return onboarding;
                }
                else
                {
                    onboarding.Id = reply.Id;
                    onboarding.IsOfferSent = reply.IsOfferSent;
                    onboarding.IsSuccessful = reply.IsSuccessful;
                    return onboarding;
                }
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while getting employee Record: {ex.Message}", nameof(ApplicantOnboarding));
                return null;
            }


            #endregion
        }


        private async Task<OnboardingData> GetApplicantOnboardingPersonnalInfo(Guid applicationId)
        {

            OnboardingData onboard = new();

            var application = await _dbContext.JobApplications.Include(x => x.Job).Where(x => x.Id == applicationId && x.CompanyId == companyId && !x.IsDeleted).FirstOrDefaultAsync();

            if (application == null)
            {
                return onboard;

            }

            var applicant = await _dbContext.ApplicantProfiles
                                                    .Where(x => x.Id == application.JobApplicantId
                                                    && x.CompanyId == companyId && !x.IsDeleted)
                                                    .FirstOrDefaultAsync();

            var applicantEducation = await _dbContext.ApplicantEducationHistories
                                                   .Where(x => x.Id == application.JobApplicantId && !x.IsDeleted)
                                                   .LastOrDefaultAsync();

            if (application == null)
            {
                return onboard;
            }

            var result1 = new OnboardRecruitmentRequest()
            {
                FirstName = applicant.FirstName,
                LastName = applicant.LastName,
                MiddleName = "",
                EmailAddress = applicant.Email,
                Address = applicant.Address,
                DateOfBirth = applicant.DateOfBirth.ToString(),
                PhoneNumber = applicant.PhoneNumber,
                Gender = "",
                MaritalStatus = "",
                PersonalEmail = applicant.Email,
                ReligionId = "",
                FieldOfStudy = applicantEducation.CourseName,
                Degree = applicantEducation.QualificationDegreeName,
                CGPA = applicantEducation.CGPA,
                InstitutionId = applicantEducation.InstitutionName,
                NextOfKinAddress = "",
                NextOfKinName = "",
                NextOfKinRelationship = "",
                NextOfKinPhoneNumber = "",
                State = "",
                Country = "",
                CompanyId = companyId.ToString(),
                JobTitleId = application.JobId.ToString(),
                JobTitleName = application.Job.JobTitle,
                HiredDate = DateTime.Now,
                SalaryPerAnnum = "",
                Designation = application.Job.Description,
                EmployeeType = application.Job.EmploymentType.GetDescription(),
                DepartmentId = application.Job.DepartmentId.ToString(),
                DepartmentName = "",
                ReportingManager = "",
                ReportingManagerId = application.Id.ToString(),
                CompanyLocationId = application.JobLocation.ToString(),
                CompanyLocationName = "",
            };

            onboard.JobApplicantOnboarding = result1;

            return onboard;

        }
        private async Task CreateUpcomingTask(string description, DateTime upcomingTaskDate, Guid interviewerEmployeeId)
        {
            try
            {
                var reveiwerUpComingTask = new ReveiwerUpComingTask
                {

                    Id = SequentialGuid.Create(),
                    IsDeleted = false,
                    UpComingTaskDescription = description,
                    UpComingTaskDate = upcomingTaskDate,
                    CompanyId = companyId,
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedDate = DateTime.Now,
                    CreatedByName = _currentUser.GetFullname(),
                    InterviewerEmployeeId = interviewerEmployeeId
                };

                _dbContext.ReveiwerUpComingTasks.Add(reveiwerUpComingTask);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting employee Record: {ex.Message}", nameof(CreateUpcomingTask));
            }
        }

        public static string GetDateLabel(DateTime date)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            if (date == today)
            {
                return "Today";
            }
            else if (date == tomorrow)
            {
                return "Tomorrow";
            }
            else if (date > tomorrow)
            {
                DayOfWeek dayOfWeek = date.DayOfWeek;
                return dayOfWeek.ToString();
            }
            else
            {
                return "Invalid Date";
            }
        }

        private async Task<string?> GetApplicantCV(Guid applicationId)
        {
            #region employee via grpc

            try
            {
                var cv = await _dbContext.JobApplicantCVBanks.FirstOrDefaultAsync(x => x.ApplicantionId == applicationId);

                if (cv != null) { }

                return cv != null ? cv.CVFileUrl : "";
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while getting employee Record: {ex.Message}", nameof(GetEmployeeData));
                return null;
            }


            #endregion
        }
    }
}