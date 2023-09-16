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
    public class HiringSubStageService : IHiringSubStageService
    {

        private readonly IAsyncRepository<SubStage, Guid> _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly Guid companyId;
        ILogger<HiringSubStageService> _logger;

        public HiringSubStageService(IAsyncRepository<SubStage, Guid> repository, ICurrentUser currentUser, IMapper mapper,
            ILogger<HiringSubStageService> logger,
             ApplicationDbContext dbContext)
        {
            _repository = repository;
            _currentUser = currentUser;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            companyId = Guid.Parse(_currentUser.GetCompany());
        }

        public async Task<ResponseModel<SubStageModel>> CreateAsync(CreateSubStageRequestModel request)
        {
            var companyId = Guid.Parse(_currentUser.GetCompany());
            try
            {

                var checkNameExist = await _repository.GetByAsync(x => x.SubStageName.ToLower() == request.SubStageName.ToLower() && x.CompanyId == companyId && x.IsDeleted == false);
                if (checkNameExist != null)
                {
                    return ResponseModel<SubStageModel>.Failure($"{request.SubStageName} already exists");
                }

                // var record = _mapper.Map<SubStage>(request);

                var record = new SubStage();

                record.Id = SequentialGuid.Create();
                record.SubStageName = request.SubStageName;
                record.CompanyId = companyId;
                record.EmailAutoResponde = request.EmailAutoResponde;
                record.EmailTemplateId = request.EmailTemplateId;
                record.StageId = request.HirinStageId;
                record.ScoreCardId = string.IsNullOrWhiteSpace(request.ScoreCardId) ? null : Guid.Parse(request.ScoreCardId);
                record.CreatedBy = _currentUser.GetUserId();
                record.CreatedByIp = _currentUser.GetFullname();
                record.CreatedDate = DateTime.Now;
                await _repository.AddAsync(record);
                await _repository.SaveChangesAsync();

                return ResponseModel<SubStageModel>.Success(_mapper.Map<SubStageModel>(record));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while creating certification record: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<SubStageModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var hiringSubStageExist = await _repository.GetByAsync(x => x.Id == id);

                if (hiringSubStageExist == null)
                {
                    return ResponseModel<bool>.Failure("Email template does exist");
                }

                hiringSubStageExist.IsDeleted = true;
                _repository.Update(hiringSubStageExist);
                await _repository.SaveChangesAsync();

                return ResponseModel<bool>.Success(_mapper.Map<bool>(true));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting email template record by id: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<SubStageModel>> GetSingleAsync(Guid id)
        {
            var result = new SubStageModel();
            try
            {
                using (_dbContext)
                {
                    var records = _dbContext.SubStages.Include(x => x.ScoreCard)
                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.Id == id).AsQueryable();

                    result = await records.Select(s => new SubStageModel
                    {

                        Id = s.Id,
                        StageId = s.StageId,
                        SubStageName = s.SubStageName,
                        ScoreCardId = s.ScoreCardId,
                        ScoreCardName = s.ScoreCard.ScoreCardName,
                        Description = s.ScoreCard.Description,
                        EmailAutoResponde = s.EmailAutoResponde,
                        EmailTemplateId = s.EmailTemplateId,
                        StageName = s.Stage.StageName,
                        CompanyId = s.CompanyId,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        CreatedByIp = s.CreatedByIp,
                        ModifiedBy = s.ModifiedBy,
                        IsDeleted = s.IsDeleted,
                        ModifiedByIp = s.ModifiedByIp,
                        ModifiedDate = s.ModifiedDate

                    }).FirstOrDefaultAsync();

                    return ResponseModel<SubStageModel>.Success(result);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting Employee Bank record list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<SubStageModel>.Failure("Exception error");
            }

        }

        public async Task<ResponseModel<SubStageModel>> UpdateAsync(UpdateSubStageRequestModel request)
        {
            try
            {
                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<SubStageModel>.Failure("Invalid sub stage identifier");
                }

                var stage = await _repository.GetByAsync(x => x.Id == request.Id);


                if (stage == null)
                {
                    return ResponseModel<SubStageModel>.Failure("No record of sub stage with Identifier found");
                }


                stage.SubStageName = request.SubStageName;
                stage.EmailAutoResponde = request.EmailAutoResponde;
                stage.EmailTemplateId = request.EmailTemplateId;
                stage.StageId = request.StageId;
                stage.ModifiedBy = _currentUser.GetFullname();
                stage.ModifiedDate = DateTime.Now;
                stage.CompanyId = companyId;
                stage.ScoreCardId = string.IsNullOrWhiteSpace(request.ScoreCardId) ? null : Guid.Parse(request.ScoreCardId);

                _repository.Update(stage);
                await _repository.SaveChangesAsync();

                return ResponseModel<SubStageModel>.Success(_mapper.Map<SubStageModel>(stage));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating email template: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<SubStageModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<CustomPagination<List<SubStagesModel>>>> GetAllAsync(PaginationRequest filter)
        {

            var result = new List<SubStagesModel>();
            try
            {
                using (_dbContext)
                {
                    var records = _dbContext.SubStages.Include(x => x.ScoreCard)
                        .Where(x => x.CompanyId == companyId && x.IsDeleted == false).AsQueryable();

                    result = await records.Select(s => new SubStagesModel
                    {

                        Id = s.Id,
                        CompanyId = s.CompanyId,
                        StageId = s.StageId,
                        SubStageName = s.SubStageName,
                        ScoreCardId = s.ScoreCardId,
                        ScoreCardName = s.ScoreCard.ScoreCardName,
                        Description = s.ScoreCard.Description,
                        EmailAutoResponde = s.EmailAutoResponde,
                        EmailTemplateId = s.EmailTemplateId,
                        StageName = s.Stage.StageName,
                        ModifiedBy = s.ModifiedBy,
                        IsDeleted = s.IsDeleted,
                        ModifiedByIp = s.ModifiedByIp,
                        ModifiedDate = s.ModifiedDate,
                        CreatedBy = s.CreatedBy,
                        CreatedDate = s.CreatedDate,
                        CreatedByIp = s.CreatedByIp
                    }).OrderBy(x => x.CreatedDate).ToListAsync();

                    CustomPagination<List<SubStagesModel>> response = new CustomPagination<List<SubStagesModel>>()
                    {
                        modelresult = result.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                        pageNumber = filter.PageNumber,
                        pageSize = filter.PageSize,
                        TotalCount = result.Count
                    };

                    return ResponseModel<CustomPagination<List<SubStagesModel>>>.Success(response);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting Employee Bank record list: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<CustomPagination<List<SubStagesModel>>>.Failure("Exception error");
            }


        }
    }
}
