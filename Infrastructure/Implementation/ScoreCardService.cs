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
    public class ScoreCardService : IScoreCardService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ScoreCardService> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IAsyncRepository<ScoreCard, Guid> _repository;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ScoreCardQuestion, Guid> _questionrepository;
        private readonly Guid companyId;
        public ScoreCardService(ApplicationDbContext dbContext, ILogger<ScoreCardService> logger, ICurrentUser currentUser,
                                    IAsyncRepository<ScoreCard, Guid> repository, IMapper mapper,
                                    IAsyncRepository<ScoreCardQuestion, Guid> questionrepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currentUser = currentUser;
            _repository = repository;
            _mapper = mapper;
            _questionrepository = questionrepository;
            companyId = Guid.Parse(_currentUser.GetCompany());
        }

        public async Task<ResponseModel<ScoreCardModel>> CreateAsync(CreateScoreCardRequestModel request)
        {
            try
            {

                var checkNameExist = await _repository.GetByAsync(x => x.ScoreCardName.ToLower() == request.ScoreCardName.ToLower() && x.CompanyId == companyId && x.IsDeleted == false);
                if (checkNameExist != null)
                {
                    return ResponseModel<ScoreCardModel>.Failure($"{request.ScoreCardName} already exists");
                }


                var record = _mapper.Map<ScoreCard>(request);

                record.Id = SequentialGuid.Create();
                record.CompanyId = companyId;
                record.ScoreCardName = request.ScoreCardName;
                record.Description = request.Description;
                record.CreatedBy = _currentUser.GetUserId();
                record.CreatedByIp = _currentUser.GetFullname();
                record.CreatedDate = DateTime.Now;
                await _repository.AddAsync(record);
                await _repository.SaveChangesAsync();

                return ResponseModel<ScoreCardModel>.Success(_mapper.Map<ScoreCardModel>(record));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while creating Score Card record: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<ScoreCardModel>.Failure("Exception error");
            }
        }
        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var scoreCardExist = await _repository.GetByAsync(x => x.Id == id);

                if (scoreCardExist == null)
                {
                    return ResponseModel<bool>.Failure("Score Card does exist");
                }

                scoreCardExist.IsDeleted = true;
                _repository.Update(scoreCardExist);


                await _repository.SaveChangesAsync();
                return ResponseModel<bool>.Success(_mapper.Map<bool>(true));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting Score Card record by id: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<CustomPagination<List<ScoreCardDto>>>> GetAllAsync(PaginationRequest filter)
        {
            var result = new List<ScoreCardDto>();
            try
            {
                result = await _dbContext.ScoreCards.Include(x => x.RecruitmentFocusAreas).ThenInclude(x => x.ScoreCardQuestions)
                    .Where(s => s.IsDeleted == false && s.CompanyId == companyId && s.IsDeleted == false)
                    .Select(s => new ScoreCardDto
                    {
                        Id = s.Id,
                        Description = s.Description,
                        ScoreCardName = s.ScoreCardName,
                        IsDeleted = s.IsDeleted,
                        DateCreated = s.CreatedDate,
                        CreatedById = s.CreatedBy,
                        DateModified = s.ModifiedDate,
                        RecruitmentFocusAreas = s.RecruitmentFocusAreas.Where(s => s.IsDeleted == false).Select(x => new RecruitmentFocusAreaDto
                        {
                            Id = x.Id,
                            CompanyId = companyId,
                            FocusArea = x.FocusArea,
                            ScoreCardId = x.ScoreCardId,
                            TotalWeight = x.TotalWeight,
                            DateCreated = x.CreatedDate,
                            CreatedById = x.CreatedBy,
                            IsDeleted = x.IsDeleted,
                            ScoreCardQuestions = x.ScoreCardQuestions.Where(s => s.IsDeleted == false).Select(x => new ScoreCardQuestionDto
                            {
                                Id = x.Id,
                                Question = x.Question,
                                Weight = x.Weight,
                                RecruitmentFocusAreaId = x.RecruitmentFocusAreaId,
                                CompanyId = companyId,
                                IsDeleted = x.IsDeleted,
                                DateCreated = x.CreatedDate,
                                CreatedById = x.CreatedBy,
                                DateModified = x.ModifiedDate
                            }).ToList(),
                        }).ToList()
                    }).ToListAsync();

                CustomPagination<List<ScoreCardDto>> response = new CustomPagination<List<ScoreCardDto>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count
                };

                return ResponseModel<CustomPagination<List<ScoreCardDto>>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<ScoreCardDto>>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<ScoreCardDto>> GetSingleAsync(Guid id)
        {
            try
            {

                var record = await _dbContext.ScoreCards.Include(x => x.RecruitmentFocusAreas).ThenInclude(x => x.ScoreCardQuestions)
                     .Where(s => s.IsDeleted == false && s.CompanyId == companyId && s.Id == id)
                     .Select(s => new ScoreCardDto
                     {
                         Id = s.Id,
                         Description = s.Description,
                         ScoreCardName = s.ScoreCardName,
                         IsDeleted = s.IsDeleted,
                         DateCreated = s.CreatedDate,
                         CreatedById = s.CreatedBy,
                         DateModified = s.ModifiedDate,
                         RecruitmentFocusAreas = s.RecruitmentFocusAreas.Where(s => s.IsDeleted == false).Select(x => new RecruitmentFocusAreaDto
                         {
                             Id = x.Id,
                             CompanyId = companyId,
                             FocusArea = x.FocusArea,
                             ScoreCardId = x.ScoreCardId,
                             TotalWeight = x.TotalWeight,
                             DateCreated = x.CreatedDate,
                             CreatedById = x.CreatedBy,
                             IsDeleted = x.IsDeleted,
                             ScoreCardQuestions = x.ScoreCardQuestions.Where(x => s.IsDeleted == false).Select(x => new ScoreCardQuestionDto
                             {
                                 Id = x.Id,
                                 Question = x.Question,
                                 Weight = x.Weight,
                                 RecruitmentFocusAreaId = x.RecruitmentFocusAreaId,
                                 CompanyId = companyId,
                                 IsDeleted = x.IsDeleted,
                                 DateCreated = x.CreatedDate,
                                 CreatedById = x.CreatedBy,
                                 DateModified = x.ModifiedDate
                             }).ToList(),
                         }).ToList()
                     }).FirstOrDefaultAsync();

                return ResponseModel<ScoreCardDto>.Success(record);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting Employee Bank record list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<ScoreCardDto>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<List<SelectListItemDataModel>>> LoadScoreCardSelectListItem(string? filter)
        {
            try
            {

                var companyId = Guid.Parse(_currentUser.GetCompany());
                var loadScoreCardForSelectAsync = _dbContext.ScoreCards.Where(c => filter == null || c.ScoreCardName.Contains(filter) && c.CompanyId == companyId && c.IsDeleted == true)
                          .Select(c => new SelectListItemDataModel(c.Id,
                                                              c.ScoreCardName,
                                                              c.ScoreCardName))
                          .ToList();
                return ResponseModel<List<SelectListItemDataModel>>.Success(loadScoreCardForSelectAsync);
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while loading the score Card select list: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<List<SelectListItemDataModel>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<ScoreCardModel>> UpdateAsync(UpdateScoreCardRequestModel request)
        {
            try
            {

                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<ScoreCardModel>.Failure("Invalid score Card identifier");
                }

                var scoreCard = await _repository.GetByAsync(x => x.Id == request.Id && x.CompanyId == companyId);

                if (scoreCard == null)
                {
                    return ResponseModel<ScoreCardModel>.Failure("No record of score card with Identifier found");
                }


                var existingQuestion = await _questionrepository.ListAsync(x => x.RecruitmentFocusAreaId == request.Id && x.IsDeleted == false);
                _questionrepository.DeleteList(existingQuestion.ToList());


                scoreCard.ScoreCardName = request.ScoreCardName;
                scoreCard.CompanyId = companyId;
                scoreCard.Description = request.Description;
                scoreCard.ModifiedBy = _currentUser.GetFullname();
                scoreCard.ModifiedDate = DateTime.Now;
                _repository.Update(scoreCard);
                await _repository.SaveChangesAsync();


                return ResponseModel<ScoreCardModel>.Success(_mapper.Map<ScoreCardModel>(scoreCard));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating score card: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ScoreCardModel>.Failure("Exception error");
            }
        }
    }
}
