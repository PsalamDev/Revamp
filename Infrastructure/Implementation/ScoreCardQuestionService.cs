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
    public class ScoreCardQuestionService : IScoreCardQuestionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ScoreCardQuestionService> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IAsyncRepository<ScoreCardQuestion, Guid> _repository;
        private readonly IMapper _mapper;
        private readonly Guid companyId;

        public ScoreCardQuestionService(ApplicationDbContext dbContext, ILogger<ScoreCardQuestionService> logger, ICurrentUser currentUser,
                                    IAsyncRepository<ScoreCardQuestion, Guid> repository, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currentUser = currentUser;
            _repository = repository;
            _mapper = mapper;
            companyId = Guid.Parse(_currentUser.GetCompany());

        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var scoringTypeExist = await _repository.GetByAsync(x => x.Id == id);

                if (scoringTypeExist == null)
                {
                    return ResponseModel<bool>.Failure("score card queston does exist");
                }

                scoringTypeExist.IsDeleted = true;
                _repository.Update(scoringTypeExist);
                await _repository.SaveChangesAsync();
                return ResponseModel<bool>.Success(true);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting score card queston record by id: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<CustomPagination<List<ScoreCardQuestionModel>>>> GetAllAsync(PaginationRequest filter)
        {

            var result = new List<ScoreCardQuestionModel>();
            try
            {
                using (_dbContext)
                {
                    var records = _dbContext.ScoreCardQuestions
                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false).AsQueryable();

                    result = await records.Select(s => new ScoreCardQuestionModel
                    {
                        CompanyId = s.CompanyId,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        CreatedByIp = s.CreatedByIp,
                        Question = s.Question,
                        RecruitmentFocusAreaId = s.RecruitmentFocusAreaId,
                        Weight = s.Weight,
                        Id = s.Id,
                        ModifiedBy = s.ModifiedBy,

                    }).ToListAsync();

                    CustomPagination<List<ScoreCardQuestionModel>> response = new CustomPagination<List<ScoreCardQuestionModel>>()
                    {
                        modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        TotalCount = result.Count
                    };
                    return ResponseModel<CustomPagination<List<ScoreCardQuestionModel>>>.Success(response);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting score card queston record list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<CustomPagination<List<ScoreCardQuestionModel>>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<ScoreCardQuestionModel>> GetSingleAsync(Guid id)
        {
            try
            {
                using (_dbContext)
                {
                    var records = _dbContext.ScoreCardQuestions
                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.Id == id).AsQueryable();

                    var data = await records.Select(s => new ScoreCardQuestionModel
                    {
                        CompanyId = s.CompanyId,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        CreatedByIp = s.CreatedByIp,
                        Question = s.Question,
                        Weight = s.Weight,
                        RecruitmentFocusAreaId = s.RecruitmentFocusAreaId,
                        Id = s.Id,
                        ModifiedBy = s.ModifiedBy,

                    }).FirstOrDefaultAsync();

                    return ResponseModel<ScoreCardQuestionModel>.Success(data);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting score card queston record list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<ScoreCardQuestionModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<ScoreCardQuestionModel>> UpdateAsync(UpdateScoreCardQuestionModel request)
        {
            try
            {
                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<ScoreCardQuestionModel>.Failure("Invalid score card queston identifier");
                }

                var scoreCardQuestion = await _repository.GetByAsync(x => x.Id == request.Id);

                if (scoreCardQuestion == null)
                {
                    return ResponseModel<ScoreCardQuestionModel>.Failure("No record of score card queston with Identifier found");
                }

                scoreCardQuestion.Question = request.Question;
                scoreCardQuestion.CompanyId = companyId;
                scoreCardQuestion.Weight = request.Weight;
                scoreCardQuestion.RecruitmentFocusAreaId = scoreCardQuestion.RecruitmentFocusAreaId;
                scoreCardQuestion.ModifiedBy = _currentUser.GetFullname();
                scoreCardQuestion.ModifiedDate = DateTime.Now;
                scoreCardQuestion.CompanyId = companyId;


                _repository.Update(scoreCardQuestion);
                await _repository.SaveChangesAsync();


                return ResponseModel<ScoreCardQuestionModel>.Success(_mapper.Map<ScoreCardQuestionModel>(scoreCardQuestion));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating score card queston: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ScoreCardQuestionModel>.Failure("Exception error");
            }
        }
    }
}
