using Core.Common.Model;
using Core.Interfaces;
using Domain.Entities;
using HRShared.Common;
using Infrastructure.Helpers;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Implementation
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuizService> _logger;
        private readonly ICurrentUser _currentUser;

        public QuizService(ApplicationDbContext context, ILogger<QuizService> logger, ICurrentUser currentUser)
        {
            _context = context;
            _logger = logger;
            _currentUser = currentUser;
        }


        public async Task<ResponseModel<bool>> AddUpdateQuiz(ManageQuizDTO payload)
        {

            try
            {

                var companyId = Guid.Parse(_currentUser.GetCompany());

                #region Duplicate

                var check = payload.ID != Guid.Empty
                    ? await _context.Quizzes.FirstOrDefaultAsync(x =>
                        (x.Name == payload.Name.Trim() && x.PsychometricTypeId == payload.TypeId) && x.CompanyId == companyId && x.Id != payload.ID)
                    : await _context.Quizzes.FirstOrDefaultAsync(x =>
                        (x.Name == payload.Name.Trim() && x.PsychometricTypeId == payload.TypeId) && x.CompanyId == companyId);

                if (check != null && check.Id != Guid.Empty)
                    return ResponseModel<bool>.Failure("quiz with the name exist");

                #endregion

                var questions = GetQuestions(payload.Questions, payload.Name, companyId, _currentUser.GetUserId());
                #region Submission:

                var item = await _context.Quizzes.FirstOrDefaultAsync(x =>
                    x.Id == payload.ID && x.CompanyId == companyId);
                var type = await _context.PsychometricTypes.FirstOrDefaultAsync(x => x.Id == payload.TypeId);
                // var sQuestions = payload.Questions.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<List<ManageQuizDTO>>(payload.Questions);
                if (item != null && item.Id != Guid.Empty)
                {
                    item.Name = string.IsNullOrEmpty(payload.Name) ? "" : payload.Name.Trim();
                    item.PsychometricTypeId = payload.TypeId;
                    item.Type = type?.Name;
                    item.Questions = questions;
                    item.Duration = payload.Duration;


                    var allQuizQuest = _context.Questions.Where(q => q.QuizId == item.Id).AsQueryable().ToList();
                    if (allQuizQuest.Count > 0)
                    {
                        _context.Questions.RemoveRange(allQuizQuest);

                    }

                    _context.Quizzes.Update(item);

                }
                else
                {
                    item = new Quiz
                    {
                        Name = string.IsNullOrEmpty(payload.Name) ? "" : payload.Name.Trim(),
                        PsychometricTypeId = payload.TypeId,
                        Type = type?.Name,
                        Questions = questions,
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        Duration = payload.Duration

                    };
                    await _context.Quizzes.AddAsync(item);
                }

                #endregion

                await _context.SaveChangesAsync();

                return ResponseModel<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<bool>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> SubmitQuiz(QuizAnswerDTO payload)
        {
            var result = new QuizDTO();
            try
            {
                // var companyId = Guid.Parse(_currentUser.GetCompany());
                int totalScore = 0;
                int totalCorrectAnswer = 0;
                int totalQuizQuestion = 0;
                var userId = _currentUser.GetUserId();

                var applicantconfirmation = await _context.ApplicantProfiles.FirstOrDefaultAsync(x => x.UserId == _currentUser.GetUserId());

                if (applicantconfirmation == null)
                {
                    return ResponseModel<bool>.Failure("Applicant not found.");
                }

                var applicantRecord = await _context.ApplicantQuizRecords.Where(x => x.Id == payload.QuizId).SingleOrDefaultAsync();
                if (applicantRecord == null)
                {
                    return ResponseModel<bool>.Failure("Applicant quiz record not found");
                }


                if ((applicantRecord.EndDate != null && applicantRecord.EndDate < DateTime.Now) || applicantRecord.Iscompleted == true)
                {
                    return ResponseModel<bool>.Failure("Quiz has already been submited");
                }

                var quiz = await _context.Quizzes.Where(x => x.Id == applicantRecord.QuizId)
                    .Include(y => y.Questions)
                        .ThenInclude(q => q.QuestionOptions).FirstOrDefaultAsync();



                if (quiz != null)
                {

                    totalQuizQuestion = quiz.Questions.Count();
                    foreach (var answer in payload.Answers)
                    {
                        var question = quiz.Questions.Where(x => x.Id == answer.QuestionId).SingleOrDefault();
                        if (question != null)
                        {
                            var ans = question.QuestionOptions.Where(x => x.Id == answer.AnswerId).SingleOrDefault();
                            if (ans != null)
                            {
                                if (ans.IsAnswer)
                                { // totalScore += ans.Score.Value;
                                    totalCorrectAnswer++;
                                }
                            }
                        }
                    }
                }

                double quizMark = Convert.ToDouble(100) / Convert.ToDouble(totalQuizQuestion);
                applicantRecord.TotalCorrectAnswer = totalCorrectAnswer;
                var total = quizMark * totalCorrectAnswer;
                totalScore = Convert.ToInt32(total);
                applicantRecord.Totalscore = totalScore;
                applicantRecord.Iscompleted = true;
                _context.ApplicantQuizRecords.Update(applicantRecord);
                _context.SaveChanges();

                return ResponseModel<bool>.Success(true, "Applicant quiz submitted successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<bool>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<QuizDTO>> StartQuiz(Guid quizId)
        {
            //var result = new QuizDTO();
            try
            {
                var userId = _currentUser.GetUserId();

                var applicantconfirmation = await _context.ApplicantProfiles.FirstOrDefaultAsync(x => x.UserId == userId);

                if (applicantconfirmation == null)
                {
                    return ResponseModel<QuizDTO>.Failure("Applicant not found.");
                }

                var applicantQuizRecord = await _context.ApplicantQuizRecords.Where(x => x.Id == quizId).FirstOrDefaultAsync();
                if (applicantQuizRecord == null)
                {
                    return ResponseModel<QuizDTO>.Failure("Quiz not found.");
                }


                if (applicantQuizRecord.Iscompleted == true)
                {
                    return ResponseModel<QuizDTO>.Failure($"Quiz has already been submited");
                }

                var result = await (from p in _context.Quizzes
                                    where p.Id == applicantQuizRecord.QuizId
                                    select new QuizDTO
                                    {
                                        Id = p.Id,
                                        Name = p.Name,
                                        TypeId = p.PsychometricTypeId,
                                        Type = p.Type,
                                        CompanyID = p.CompanyId,
                                        IsDeleted = p.IsDeleted,
                                        DateCreated = p.CreatedDate,
                                        CreatedById = p.CreatedBy,
                                        Duration = p.Duration,
                                        DateModified = p.ModifiedDate,
                                        Questions = p.Questions.Select(q => new QuestionDTO
                                        {
                                            Id = q.Id,
                                            QuestionText = q.QuestionText,
                                            QuizId = q.QuizId,
                                            QuizName = q.QuizName,
                                            TypeId = q.TypeId,
                                            Type = q.Type,
                                            QuestionOptions = q.QuestionOptions.Select(o => new QuestionOptionDTO
                                            {
                                                Id = o.Id,
                                                Value = o.Value,
                                                QuestionId = o.QuestionId,
                                                Question = o.Question
                                            }).ToList()
                                        }).ToList()
                                    }).FirstOrDefaultAsync();

                applicantQuizRecord.StartDate = DateTime.Now.AddSeconds(5);
                applicantQuizRecord.EndDate = DateTime.Now.AddMinutes(applicantQuizRecord.Duration);
                applicantQuizRecord.ModifiedDate = DateTime.Now;
                _context.ApplicantQuizRecords.Update(applicantQuizRecord);
                await _context.SaveChangesAsync();

                if (result == null)
                {
                    return ResponseModel<QuizDTO>.Failure("Quiz failed to start");
                }

                return ResponseModel<QuizDTO>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<QuizDTO>.Exception(ex.Message); ;
            }
        }

        public async Task<ResponseModel<CustomPagination<List<QuizDTO>>>> GetAllQuizzes(Quizfilter filter)
        {
            var result = new List<QuizDTO>();
            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());
                result = await (from p in _context.Quizzes
                                where p.IsDeleted == false && p.CompanyId == companyId && p.IsDeleted == false
                                select new QuizDTO
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    TypeId = p.PsychometricTypeId,
                                    Type = p.Type,
                                    CompanyID = p.CompanyId,
                                    IsDeleted = p.IsDeleted,
                                    DateCreated = p.CreatedDate,
                                    CreatedById = p.CreatedBy,
                                    DateModified = p.ModifiedDate,
                                    Duration = p.Duration,
                                    Questions = p.Questions.Select(q => new QuestionDTO
                                    {
                                        Id = q.Id,
                                        QuestionText = q.QuestionText,
                                        QuizId = q.QuizId,
                                        QuizName = q.QuizName,
                                        TypeId = q.TypeId,
                                        Type = q.Type,
                                        QuestionOptions = q.QuestionOptions.Select(o => new QuestionOptionDTO
                                        {
                                            Id = o.Id,
                                            Value = o.Value,
                                            QuestionId = o.QuestionId,
                                            Question = o.Question
                                        }).ToList()
                                    }).ToList()
                                }).ToListAsync();


                if (filter.StartRange.HasValue && filter.EndRange.HasValue)
                {
                    result = result.Where(x => x.TotalQuestions >= filter.StartRange && x.TotalQuestions <= filter.EndRange).ToList();
                }


                CustomPagination<List<QuizDTO>> response = new CustomPagination<List<QuizDTO>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count
                };

                return ResponseModel<CustomPagination<List<QuizDTO>>>.Success(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<QuizDTO>>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<QuizDTO>> GetQuizById(Guid id)
        {
            var result = new QuizDTO();
            try
            {
                //var companyId = Guid.Parse(_currentUser.GetCompany());
                result = await (from p in _context.Quizzes
                                where p.Id == id
                                select new QuizDTO
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    TypeId = p.PsychometricTypeId,
                                    Type = p.Type,
                                    CompanyID = p.CompanyId,
                                    IsDeleted = p.IsDeleted,
                                    DateCreated = p.CreatedDate,
                                    CreatedById = p.CreatedBy,
                                    Duration = p.Duration,
                                    DateModified = p.ModifiedDate,
                                    Questions = p.Questions.Select(q => new QuestionDTO
                                    {
                                        Id = q.Id,
                                        QuestionText = q.QuestionText,
                                        QuizId = q.QuizId,
                                        QuizName = q.QuizName,
                                        TypeId = q.TypeId,
                                        Type = q.Type,
                                        QuestionOptions = q.QuestionOptions.Select(o => new QuestionOptionDTO
                                        {
                                            Id = o.Id,
                                            Value = o.Value,
                                            QuestionId = o.QuestionId,
                                            IsAnswer = o.IsAnswer,
                                            Question = o.Question
                                        }).ToList()
                                    }).ToList()
                                }).FirstOrDefaultAsync();

                return ResponseModel<QuizDTO>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<QuizDTO>.Exception(ex.Message); ;
            }
        }

        public async Task<ResponseModel<bool>> DeleteQuiz(Guid id)
        {
            try
            {
                // #region Initialization:
                //
                // var activityBody = new ActivityLogDTO
                // {
                //     CompanyID = usr.company_id,
                //     CreatedById = usr.user_id,
                //     ModuleAction = "Delete Quiz",
                //     ModuleName = CommonModelNames.QUIZ,
                //     UserId = usr.user_id
                // };
                //
                // #endregion

                var data = await _context.Quizzes.FindAsync(id);
                if (data == null || data.Id == Guid.Empty)
                {
                    return ResponseModel<bool>.Failure("Quiz Not Found");
                }


                data.IsDeleted = true;
                data.ModifiedBy = _currentUser.GetFullname();
                data.ModifiedDate = DateTime.Now;

                var Questions = _context.Questions.Where(a => a.QuizId == id);


                if (Questions.Count() > 0)
                {
                    foreach (var deleteQuestion in Questions)
                    {
                        deleteQuestion.IsDeleted = true;
                        deleteQuestion.ModifiedBy = _currentUser.GetFullname();
                        deleteQuestion.ModifiedDate = DateTime.Now;
                    }


                }

                var QuestionsOptions = _context.QuestionOptions.Where(a => a.QuestionId == Questions.FirstOrDefault().Id);


                if (QuestionsOptions.Count() > 0)
                {
                    foreach (var QuestionsOptionsValue in QuestionsOptions)
                    {
                        QuestionsOptionsValue.IsDeleted = true;
                        QuestionsOptionsValue.ModifiedBy = _currentUser.GetFullname();
                        QuestionsOptionsValue.ModifiedDate = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();



                return ResponseModel<bool>.Success(true);

            }

            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<bool>.Exception(ex.Message); ;
            }

        }


        public List<Question> GetQuestions(string question, string quiz, Guid companyId, Guid UserId)
        {
            var retItems = new List<Question>();
            if (string.IsNullOrEmpty(question)) return retItems;

            var questions = JsonConvert.DeserializeObject<List<QuestionDTO>>(question);
            if (!questions.Any())
                return retItems;

            questions.ForEach(x =>
            {
                string type = Enum.GetName(typeof(QuestionTypes), x.TypeId)?.Replace('_', ' ');
                retItems.Add(new Question
                {
                    QuestionText = x.QuestionText,
                    QuizId = x.QuizId,
                    QuizName = quiz,
                    TypeId = x.TypeId,
                    Type = type,
                    CompanyId = companyId,
                    CreatedBy = UserId,
                    QuestionOptions = x.QuestionOptions.Select(o => new QuestionOption
                    {
                        Value = o.Value,
                        QuestionId = o.QuestionId,
                        Question = x.QuestionText,
                        IsAnswer = o.IsAnswer,
                        CreatedBy = UserId
                    }).ToList()
                });
            });
            return retItems;
        }

        public async Task<List<IDTextViewModel>> GetQuizTypes()
        {
            List<IDTextViewModel> response = new List<IDTextViewModel>();
            try
            {
                return await _context.PsychometricTypes
                    .Where(x => x.IsDeleted == false).Select(t => new IDTextViewModel
                    {
                        Id = t.Id,
                        Text = t.Name
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return response;
            }
        }
        public List<IDTextViewModel> GetQuestionTypes()
        {
            var items = System.Enum.GetValues(typeof(QuestionTypes))
                .Cast<int>()
                .Select(x =>
                    new IDTextViewModel
                    {
                        Text = System.Enum.GetName(typeof(QuestionTypes), x)?.ToString().Replace('_', ' '),
                        Id = x
                    }
                )
                .ToList();

            return items;
        }

        public async Task<ResponseModel<CustomPagination<List<QuizDTO>>>> GetApplicantAllQuizzes(ApplicantQuizfilter filter)
        {
            var result = new List<QuizDTO>();
            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());

                result = await _context.ApplicantQuizRecords.Include(x => x.Quiz)
                      .Where(x => x.CompanyId == companyId && x.ApplicantProfileId == filter.ApplicantId && x.Iscompleted == false)
                      .Select(x => new QuizDTO
                      {
                          Id = x.Id,
                          Name = x.Quiz.Name,
                          TypeId = x.Quiz.PsychometricTypeId,
                          Type = x.Quiz.Type,
                          CompanyID = x.Quiz.CompanyId,
                          IsDeleted = x.Quiz.IsDeleted,
                          DateCreated = x.Quiz.CreatedDate,
                          CreatedById = x.Quiz.CreatedBy,
                          DateModified = x.Quiz.ModifiedDate,
                          Duration = x.Quiz.Duration,
                          Questions = x.Quiz.Questions.Select(question => new QuestionDTO
                          {
                              Id = question.Id,
                              QuestionText = question.QuestionText,
                              QuizId = question.QuizId,
                              QuizName = question.QuizName,
                              TypeId = question.TypeId,
                              Type = question.Type
                          }).ToList()
                      }).ToListAsync();


                //if (filter.StartRange.HasValue && filter.EndRange.HasValue)
                //{
                //    result = result.Where(x => x.TotalQuestions >= filter.StartRange && x.TotalQuestions <= filter.EndRange).ToList();
                //}


                CustomPagination<List<QuizDTO>> response = new CustomPagination<List<QuizDTO>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count
                };


                if (response.modelresult.Count > 0)
                {
                    return ResponseModel<CustomPagination<List<QuizDTO>>>.Success(response);
                }
                else
                {
                    return ResponseModel<CustomPagination<List<QuizDTO>>>.Success(response, "Not found.");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<QuizDTO>>>.Exception(ex.Message);
            }
        }

    }
}