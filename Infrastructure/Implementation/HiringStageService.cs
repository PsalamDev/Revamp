using AutoMapper;
using Core.Common.Model;
using Core.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using HRShared.Common;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Infrastructure.Implementation
{
    public class HiringStageService : IHiringStageService
    {

        private readonly IAsyncRepository<Stage, Guid> _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<HiringStageService> _logger;
        private readonly Guid companyId;

        public HiringStageService(IAsyncRepository<Stage, Guid> repository, ICurrentUser currentUser, IMapper mapper,
            ILogger<HiringStageService> logger,
             ApplicationDbContext dbContext)
        {
            _repository = repository;
            _currentUser = currentUser;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            companyId = Guid.Parse(_currentUser.GetCompany());
        }

        public async Task<ResponseModel<StageModel>> CreateAsync(CreateStageRequestModel request)
        {
            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());

                var checkNameExist = await _repository.GetByAsync(x => x.StageName.ToLower() == request.StageName.ToLower() && x.CompanyId == companyId && x.IsDeleted == false);
                if (checkNameExist != null)
                {
                    return ResponseModel<StageModel>.Failure($"{request.StageName} already exists");
                }

                var record = _mapper.Map<Stage>(request);

                record.Id = SequentialGuid.Create();
                record.StageName = request.StageName;
                record.ScoreCardId = request.ScoreCardId;
                record.CompanyId = companyId;
                record.EmailAutoResponde = request.EmailAutoResponde;
                record.EmailTemplateId = request.EmailTemplateId;
                record.CreatedBy = _currentUser.GetUserId();
                record.CreatedByIp = _currentUser.GetFullname();
                record.CreatedDate = DateTime.Now;
                await _repository.AddAsync(record);
                await _repository.SaveChangesAsync();

                return ResponseModel<StageModel>.Success(_mapper.Map<StageModel>(record));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while creating certification record: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<StageModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var hiringStageExist = await _repository.GetByAsync(x => x.Id == id);

                if (hiringStageExist == null)
                {
                    return ResponseModel<bool>.Failure("Stage does exist");
                }

                hiringStageExist.IsDeleted = true;
                _repository.Update(hiringStageExist);
                await _repository.SaveChangesAsync();

                var subtage = _dbContext.SubStages.Where(a => a.StageId == id).ToList();
                if (subtage.Count() > 0)
                {
                    foreach (var subStageValue in subtage)
                    {
                        subStageValue.IsDeleted = true;
                        subStageValue.ModifiedBy = _currentUser.GetFullname();
                        subStageValue.ModifiedDate = DateTime.Now;

                        _dbContext.SubStages.Update(subStageValue);
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

        public async Task<ResponseModel<List<SelectListItemDataModel>>> LoadStageSelectListItem(string? filter)
        {
            try
            {
                var loadStageForSelectAsync = _dbContext.Stages.Where(c => filter == null || c.StageName.Contains(filter) && c.CompanyId == c.CompanyId && c.IsDeleted == false)
                          .Select(c => new SelectListItemDataModel(c.Id,
                                                              c.StageName,
                                                              c.StageName))
                          .ToList();
                return ResponseModel<List<SelectListItemDataModel>>.Success(loadStageForSelectAsync);
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while loading the recruitment stage select list: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<List<SelectListItemDataModel>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<StageModel>> GetSingleAsync(Guid id)
        {
            var result = new StageModel();
            try
            {
                var records = _dbContext.Stages
                    .Include(x => x.ScoreCard)
                    .Include(x => x.SubStages)
                    .Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.Id == id).AsQueryable();

                result = await records.Select(s => new StageModel
                {
                    CompanyId = s.CompanyId,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    CreatedByIp = s.CreatedByIp,
                    EmailAutoResponde = s.EmailAutoResponde,
                    EmailTemplateId = s.EmailTemplateId,
                    StageName = s.StageName,
                    Id = s.Id,
                    ModifiedBy = s.ModifiedBy,
                    SubStageModels = s.SubStages.Where(x => x.IsDeleted == false).Select(s => new SubStageModel
                    {
                        Id = s.Id,
                        SubStageName = s.SubStageName,
                        StageName = s.Stage.StageName,
                        ScoreCardId = s.ScoreCardId,
                        Description = s.ScoreCard.Description,
                        ScoreCardName = s.ScoreCard.ScoreCardName,
                        ModifiedBy = s.ModifiedBy,
                        CompanyId = s.CompanyId,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        CreatedByIp = s.CreatedByIp,
                        EmailAutoResponde = s.EmailAutoResponde,
                        EmailTemplateId = s.EmailTemplateId,
                        StageId = s.StageId,
                        IsDeleted = s.IsDeleted,
                        ModifiedByIp = s.ModifiedByIp,
                        ModifiedDate = s.ModifiedDate
                    }).ToList()
                }).FirstOrDefaultAsync();


                _logger.LogInformation("fectching hiring stage data...");
                return ResponseModel<StageModel>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<StageModel>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<StageModel>> UpdateAsync(UpdateStageRequestModel request)
        {
            try
            {
                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<StageModel>.Failure("Invalid stage identifier");
                }

                var stage = await _repository.GetByAsync(x => x.Id == request.Id);

                if (stage == null)
                {
                    return ResponseModel<StageModel>.Failure("No record of stage with Identifier found");
                }


                stage.ScoreCardId = request.ScoreCardId;
                stage.StageName = request.StageName;
                stage.EmailAutoResponde = request.EmailAutoResponde;
                stage.EmailTemplateId = request.EmailTemplateId;
                stage.ModifiedBy = _currentUser.GetFullname();
                stage.ModifiedDate = DateTime.Now;
                stage.CompanyId = companyId;

                _repository.Update(stage);
                await _repository.SaveChangesAsync();

                return ResponseModel<StageModel>.Success(_mapper.Map<StageModel>(stage));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating email template: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<StageModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<CustomPagination<List<StagesModel>>>> GetAllAsync(PaginationRequest filter)
        {
            var result = new List<StagesModel>();
            try
            {
                var records = _dbContext.Stages.Include(x => x.ScoreCard)
                    .Where(x => x.CompanyId == companyId && x.IsDeleted == false).AsQueryable();

                result = await records.Select(s => new StagesModel
                {
                    CompanyId = s.CompanyId,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    CreatedByIp = s.CreatedByIp,
                    EmailAutoResponde = s.EmailAutoResponde,
                    EmailTemplateId = s.EmailTemplateId,
                    ScoreCardId = s.ScoreCardId,
                    ScoreCardName = s.ScoreCard.ScoreCardName,
                    Description = s.ScoreCard.Description,
                    IsDeleted = s.IsDeleted,
                    ModifiedByIp = s.ModifiedByIp,
                    ModifiedDate = s.ModifiedDate,
                    StageName = s.StageName,
                    Id = s.Id,
                    ModifiedBy = s.ModifiedBy,
                    SubStageModels = s.SubStages.Where(x => x.IsDeleted == false).Select(s => new SubStagesModel
                    {

                        Id = s.Id,
                        CompanyId = s.CompanyId,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        CreatedByIp = s.CreatedByIp,
                        EmailAutoResponde = s.EmailAutoResponde,
                        EmailTemplateId = s.EmailTemplateId,
                        SubStageName = s.SubStageName,
                        StageName = s.Stage.StageName,
                        ScoreCardId = s.ScoreCardId,
                        ScoreCardName = s.ScoreCard.ScoreCardName,
                        ModifiedBy = s.ModifiedBy,
                        StageId = s.StageId,
                        IsDeleted = s.IsDeleted,
                        ModifiedByIp = s.ModifiedByIp,
                        ModifiedDate = s.ModifiedDate
                    }).OrderBy(x => x.CreatedDate).ToList()
                }).ToListAsync();


                LogContext.PushProperty("CompanyId", _currentUser.GetCompany());
                _logger.LogInformation("fectching hiring stage data...");

                CustomPagination<List<StagesModel>> response = new CustomPagination<List<StagesModel>>()
                {
                    modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    TotalCount = result.Count
                };

                return ResponseModel<CustomPagination<List<StagesModel>>>.Success(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return ResponseModel<CustomPagination<List<StagesModel>>>.Exception(ex.Message);
            }
        }

        public async Task<ResponseModel<string>> CreateStagesSeederAsync(Guid cId)
        {

            try
            {
                var stages = new List<Stage>()
                {

                    new Stage()
                    {
                         Id = SequentialGuid.Create(),
                         StageName = "Applied Stage",
                         CompanyId = cId,
                          CreatedBy = _currentUser.GetUserId(),
                         CreatedByName = _currentUser.GetFullname(),
                         CreatedDate = DateTime.Now,
                         EmailAutoResponde = false
                    },
                    new Stage()
                    {
                         Id = SequentialGuid.Create(),
                         StageName = "Hired Stage",
                         CompanyId = cId,
                          CreatedBy = _currentUser.GetUserId(),
                         CreatedByName = _currentUser.GetFullname(),
                         CreatedDate = DateTime.Now,
                         EmailAutoResponde = false
                    },
                    new Stage()
                    {
                         Id = SequentialGuid.Create(),
                         StageName = "Shortlisted Stage",
                         CompanyId = cId,
                         CreatedBy = _currentUser.GetUserId(),
                         CreatedByName = _currentUser.GetFullname(),
                         CreatedDate = DateTime.Now,
                         EmailAutoResponde = false
                    },
                    new Stage()
                    {
                         Id = SequentialGuid.Create(),
                         StageName = "Pending Offer Stage",
                         CompanyId = cId,
                          CreatedBy = _currentUser.GetUserId(),
                         CreatedByName = _currentUser.GetFullname(),
                         CreatedDate = DateTime.Now,
                         EmailAutoResponde = false
                    },
                    new Stage()
                    {
                         Id = SequentialGuid.Create(),
                         StageName = "Rejected Stage",
                         CompanyId = cId,
                          CreatedBy = _currentUser.GetUserId(),
                         CreatedByName = _currentUser.GetFullname(),
                         CreatedDate = DateTime.Now,
                         EmailAutoResponde = false
                    },
                    new Stage()
                    {
                         Id = SequentialGuid.Create(),
                         StageName = "Interview Stage",
                         CompanyId = cId,
                          CreatedBy = _currentUser.GetUserId(),
                         CreatedByName = _currentUser.GetFullname(),
                         CreatedDate = DateTime.Now,
                         EmailAutoResponde = false
                    }
        };

                await _repository.AddListAsync(stages);
                await _repository.SaveChangesAsync();
                return ResponseModel<string>.Success("Seed Successful");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.StackTrace);
                return ResponseModel<string>.Exception(ex.Message);
            }

        }
    }
}