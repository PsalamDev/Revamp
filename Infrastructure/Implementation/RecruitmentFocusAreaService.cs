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
    public class RecruitmentFocusAreaService : IRecruitmentFocusAreaService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RecruitmentFocusAreaService> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IAsyncRepository<RecruitmentFocusArea, Guid> _recruitmentFocusAreaRepository;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ScoreCardQuestion, Guid> _questionrepository;
        private readonly Guid companyId;
        public RecruitmentFocusAreaService(ApplicationDbContext dbContext, ILogger<RecruitmentFocusAreaService> logger, ICurrentUser currentUser,
                                    IAsyncRepository<RecruitmentFocusArea, Guid> repository, IMapper mapper,
                                    IAsyncRepository<ScoreCardQuestion, Guid> questionrepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currentUser = currentUser;
            _recruitmentFocusAreaRepository = repository;
            _mapper = mapper;
            _questionrepository = questionrepository;
            companyId = Guid.Parse(_currentUser.GetCompany());
        }
        public async Task<ResponseModel<RecruitmentFocusAreaModel>> CreateAsync(CreateRecruitmentFocusAreaRequestModel request)
        {
            try
            {

                var checkNameExist = await _dbContext.RecruitmentFocusAreas.Where(x => x.FocusArea.Equals(request.FocusArea) && x.CompanyId == companyId && x.IsDeleted == false)
                    .FirstOrDefaultAsync();

                if (checkNameExist != null)
                {
                    return ResponseModel<RecruitmentFocusAreaModel>.Failure($"{request.FocusArea} already exists");
                }


                var totalScoreCardQuestionWeight = request.ScoreCardQuestions.Sum(x => x.Weight);
                if (request.TotalWeight < totalScoreCardQuestionWeight)
                {
                    return ResponseModel<RecruitmentFocusAreaModel>.Failure($"Please adjust your questions weight to match the overall weight");
                }


                var record = new RecruitmentFocusArea
                {
                    Id = SequentialGuid.Create(),
                    FocusArea = request.FocusArea,
                    ScoreCardId = request.ScoreCardId,
                    CompanyId = companyId,
                    TotalWeight = request.TotalWeight,
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedByIp = _currentUser.GetFullname(),
                    CreatedDate = DateTime.Now
                };
                await _recruitmentFocusAreaRepository.AddAsync(record);
                await _recruitmentFocusAreaRepository.SaveChangesAsync();

                if (request.ScoreCardQuestions.Count() > 0)
                {
                    foreach (var scoreCard in request.ScoreCardQuestions)
                    {
                        var scoreCardQuestion = new ScoreCardQuestion
                        {
                            Id = SequentialGuid.Create(),
                            RecruitmentFocusAreaId = record.Id,
                            Question = scoreCard.Question,
                            Weight = scoreCard.Weight,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByIp = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now,
                            CompanyId = companyId,
                            IsDeleted = false
                        };
                        _questionrepository.Add(scoreCardQuestion);
                        await _questionrepository.SaveChangesAsync();
                    }
                }

                return ResponseModel<RecruitmentFocusAreaModel>.Success(_mapper.Map<RecruitmentFocusAreaModel>(record));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while creating recruitment focus area record: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<RecruitmentFocusAreaModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var scoreCardExist = await _recruitmentFocusAreaRepository.GetByAsync(x => x.Id == id && x.IsDeleted == false);

                if (scoreCardExist == null)
                {
                    return ResponseModel<bool>.Failure("Recruitment focus area does exist");
                }

                scoreCardExist.IsDeleted = true;
                scoreCardExist.ModifiedBy = _currentUser.GetFullname();
                scoreCardExist.ModifiedDate = DateTime.Now;
                _recruitmentFocusAreaRepository.Update(scoreCardExist);
                await _recruitmentFocusAreaRepository.SaveChangesAsync();

                var questionRecord = await _dbContext.ScoreCardQuestions.Where(x => x.RecruitmentFocusAreaId == id).ToListAsync();

                foreach (var question in questionRecord)
                {

                    question.IsDeleted = true;
                    question.ModifiedDate = DateTime.Now;
                    _questionrepository.Update(question);
                    await _dbContext.SaveChangesAsync();
                }

                return ResponseModel<bool>.Success(_mapper.Map<bool>(true));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting recruitment focus area record by id: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<CustomPagination<List<RecruitmentFocusAreaDto>>>> GetAllAsync(PaginationRequest filter)
        {

            var result = new List<RecruitmentFocusAreaDto>();
            try
            {
                using (_dbContext)
                {
                    var records = _dbContext.RecruitmentFocusAreas
                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false).AsQueryable();

                    result = await records.Select(s => new RecruitmentFocusAreaDto
                    {

                        Id = s.Id,
                        CompanyId = s.CompanyId,
                        FocusArea = s.FocusArea,
                        TotalWeight = s.TotalWeight,
                        ScoreCardId = s.ScoreCardId,
                        IsDeleted = s.IsDeleted,
                        DateCreated = s.CreatedDate,
                        CreatedById = s.CreatedBy,
                        DateModified = s.ModifiedDate,
                        ScoreCardQuestions = s.ScoreCardQuestions.Select(s => new ScoreCardQuestionDto
                        {
                            Id = s.Id,
                            CompanyId = s.CompanyId,
                            Question = s.Question,
                            Weight = s.Weight,
                            IsDeleted = s.IsDeleted,
                            RecruitmentFocusAreaId = s.RecruitmentFocusAreaId,
                            DateCreated = s.CreatedDate,
                            CreatedById = s.CreatedBy,
                            DateModified = s.ModifiedDate
                        }).ToList()
                    }).ToListAsync();


                    CustomPagination<List<RecruitmentFocusAreaDto>> response = new CustomPagination<List<RecruitmentFocusAreaDto>>()
                    {
                        modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        TotalCount = result.Count
                    };

                    return ResponseModel<CustomPagination<List<RecruitmentFocusAreaDto>>>.Success(response);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<RecruitmentFocusAreaDto>>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<RecruitmentFocusAreaModel>> GetSingleAsync(Guid id)
        {
            try
            {
                using (_dbContext)
                {
                    var records = _dbContext.RecruitmentFocusAreas
                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.Id == id).AsQueryable();

                    var data = await records.Select(s => new RecruitmentFocusAreaModel
                    {

                        CompanyId = s.CompanyId,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        CreatedByIp = s.CreatedByIp,
                        FocusArea = s.FocusArea,
                        TotalWeight = s.TotalWeight,
                        Id = s.Id,
                        ModifiedBy = s.ModifiedBy,
                        ScoreCardId = s.ScoreCardId,
                        ScoreCardQuestions = s.ScoreCardQuestions.Select(s => new ScoreCardQuestionModel
                        {
                            Id = s.Id,
                            CompanyId = s.CompanyId,
                            CreatedBy = s.CreatedBy,
                            CreatedDate = s.CreatedDate,
                            CreatedByIp = s.CreatedByIp,
                            Question = s.Question,
                            Weight = s.Weight,
                            RecruitmentFocusAreaId = s.RecruitmentFocusAreaId,
                            ModifiedBy = s.ModifiedBy,
                            IsDeleted = s.IsDeleted,
                            ModifiedByIp = s.ModifiedByIp,
                            ModifiedDate = s.ModifiedDate
                        }).ToList()


                    }).FirstOrDefaultAsync();

                    return ResponseModel<RecruitmentFocusAreaModel>.Success(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting Employee Bank record list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<RecruitmentFocusAreaModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<List<SelectListItemDataModel>>> LoadRecruitmentFocusAreaSelectListItem(string? filter)
        {
            try
            {
                var loadScoreCardForSelectAsync = _dbContext.RecruitmentFocusAreas.Where(c => filter == null || c.FocusArea.Contains(filter) && c.CompanyId == c.CompanyId && c.IsDeleted == true)
                      .Select(c => new SelectListItemDataModel(c.Id,
                                                          c.FocusArea,
                                                          c.FocusArea))
                      .ToList();
                return ResponseModel<List<SelectListItemDataModel>>.Success(loadScoreCardForSelectAsync);
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while loading the score Card select list: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<List<SelectListItemDataModel>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<RecruitmentFocusAreaModel>> UpdateAsync(UpdateRecruitmentFocusAreaRequestModel request)
        {
            try
            {
                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<RecruitmentFocusAreaModel>.Failure("Invalid score Card identifier");
                }

                var focusArea = await _recruitmentFocusAreaRepository.GetByAsync(x => x.Id == request.Id);

                if (focusArea == null)
                {
                    return ResponseModel<RecruitmentFocusAreaModel>.Failure("No record of score card with Identifier found");
                }

                var totalScoreCardQuestionWeight = request.ScoreCardQuestions.Sum(x => x.Weight);
                if (request.TotalWeight < totalScoreCardQuestionWeight)
                {
                    return ResponseModel<RecruitmentFocusAreaModel>.Failure($"Please adjust your questions weight to match the overall weight");
                }

                var existingQuestion = await _dbContext.ScoreCardQuestions.Where(x => x.RecruitmentFocusAreaId == request.Id).ToListAsync();
                if (existingQuestion.Count() > 0)
                {
                    _questionrepository.DeleteList(existingQuestion);
                }

                focusArea.FocusArea = request.FocusArea;
                focusArea.CompanyId = companyId;
                focusArea.TotalWeight = request.TotalWeight;
                focusArea.ModifiedBy = _currentUser.GetFullname();
                focusArea.ModifiedDate = DateTime.Now;
                focusArea.CompanyId = companyId;

                _recruitmentFocusAreaRepository.Update(focusArea);
                await _recruitmentFocusAreaRepository.SaveChangesAsync();

                foreach (var questionRequest in request.ScoreCardQuestions)
                {
                    var questionRecord = new ScoreCardQuestion
                    {
                        Id = SequentialGuid.Create(),
                        CompanyId = companyId,
                        CreatedBy = _currentUser.GetUserId(),
                        CreatedByIp = _currentUser.GetFullname(),
                        CreatedDate = DateTime.Now,
                        Question = questionRequest.Question,
                        Weight = questionRequest.Weight,
                        RecruitmentFocusAreaId = focusArea.Id
                    };
                    await _questionrepository.AddAsync(questionRecord);
                }
                await _questionrepository.SaveChangesAsync();
                return ResponseModel<RecruitmentFocusAreaModel>.Success(_mapper.Map<RecruitmentFocusAreaModel>(focusArea));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating score card: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<RecruitmentFocusAreaModel>.Failure("Exception error");
            }
        }
    }
}