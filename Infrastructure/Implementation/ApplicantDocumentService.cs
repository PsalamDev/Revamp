using AutoMapper;
using Core.Common.Model;
using Core.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using HRShared.Common;
using HRShared.CoreProviders.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation
{
    public class ApplicantDocumentService : IApplicantDocumentService
    {

        private readonly IAsyncRepository<ApplicantDocument, Guid> _applicantDocumentRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicantDocumentService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IAzureStorageServices _azureStorageServices;

        public ApplicantDocumentService(IAsyncRepository<ApplicantDocument, Guid> applicantDocumentRepository, ICurrentUser currentUser, IMapper mapper,
            ILogger<ApplicantDocumentService> logger, ApplicationDbContext context, IAzureStorageServices azureStorageServices)
        {
            _applicantDocumentRepository = applicantDocumentRepository;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
            _context = context;
            _azureStorageServices = azureStorageServices;
        }

        public async Task<ResponseModel<ApplicantDocumentResponse>> CreateAsync(ApplicantDocumentRequest request)
        {

            try
            {
                //get companyId 
                var companyId = Guid.Parse(_currentUser.GetCompany());

                //upload document to azure here
                var fileUrl = await _azureStorageServices.UploadToAzureAsync(request.File);
                if (string.IsNullOrWhiteSpace(fileUrl))
                {
                    return ResponseModel<ApplicantDocumentResponse>.Failure("Fail to upload document please try again");
                }

                var appDocument = new ApplicantDocument()
                {
                    Id = SequentialGuid.Create(),
                    IsDeleted = false,
                    CreatedBy = _currentUser.GetUserId(),
                    CreatedByName = _currentUser.GetFullname(),
                    CreatedDate = DateTime.Now,
                    ApplicantId = request.ApplicantId,
                    FileName = request.File.Name,
                    FileType = request.File.ContentType,
                    FileUrl = fileUrl,
                    Comment = request.Comment,
                    DocuemntType = request.DocuemntType,
                    DocumentTitle = request.DocumentTitle,
                    DocuemntTypeName = request.DocuemntTypeName
                };

                await _applicantDocumentRepository.AddAsync(appDocument);
                await _applicantDocumentRepository.SaveChangesAsync();

                return ResponseModel<ApplicantDocumentResponse>.Success(_mapper.Map<ApplicantDocumentResponse>(appDocument));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while creating document: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<ApplicantDocumentResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<ApplicantDocumentResponse>> UpdateAsync(UpdateDocumentRequest request)
        {
            try
            {
                //get companyId 
                var companyId = Guid.Parse(_currentUser.GetCompany());


                //check for document Id to validate document here
                var record = await _applicantDocumentRepository.GetByAsync(X => X.Id == request.Id);

                if (record == null)
                {
                    return ResponseModel<ApplicantDocumentResponse>.Failure($"Document with id {request.Id} not found");
                }

                //upload document to azure here
                var imageUrl = await _azureStorageServices.UploadToAzureAsync(request.File);
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    return ResponseModel<ApplicantDocumentResponse>.Failure("Fail to upload document please try again");
                }


                record.IsDeleted = false;
                record.FileName = request.File.Name;
                record.FileType = request.File.ContentType;
                record.FileUrl = imageUrl;
                record.Comment = request.Comment;
                record.DocuemntType = request.DocuemntType;
                record.DocuemntTypeName = request.DocuemntTypeName;
                record.DocuemntType = request.DocuemntType;



                _applicantDocumentRepository.Update(record);
                await _applicantDocumentRepository.SaveChangesAsync();

                return ResponseModel<ApplicantDocumentResponse>.Success(_mapper.Map<ApplicantDocumentResponse>(record));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating document: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ApplicantDocumentResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<ApplicantDocumentResponse>> GetSingleAsync(Guid id)
        {
            try
            {
                var record = await _applicantDocumentRepository.GetByAsync(X => X.Id == id);
                return ResponseModel<ApplicantDocumentResponse>.Success(_mapper.Map<ApplicantDocumentResponse>(record));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting  document: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<ApplicantDocumentResponse>.Exception("Exception error");
            }
        }

        public async Task<ResponseModel<CustomPagination<List<ApplicantDocumentListResponse>>>> GetAllListAsync(DocumentListRequest model)
        {
            try
            {
                var docs = await (from document in _context.ApplicantDocuments
                                  where document.ApplicantId == model.ApplicantId && document.IsDeleted == false
                                  select new ApplicantDocumentListResponse()
                                  {
                                      Id = document.Id,
                                      Comment = document.Comment,
                                      IsDeleted = document.IsDeleted,
                                      ApplicantId = document.ApplicantId,
                                      CreatedBy = document.CreatedBy,
                                      FileName = document.FileName,
                                      FileType = document.FileType,
                                      FileUrl = document.FileUrl,
                                      DocuemntType = document.DocuemntType,
                                      DocumentTitle = document.DocumentTitle,
                                      DocuemntTypeName = document.DocuemntTypeName

                                  }).ToListAsync();



                CustomPagination<List<ApplicantDocumentListResponse>> documents =
                    new CustomPagination<List<ApplicantDocumentListResponse>>()
                    {
                        modelresult = docs.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList(),
                        TotalCount = docs.Count,
                        pageNumber = model.PageNumber,
                        pageSize = model.PageSize
                    };

                return ResponseModel<CustomPagination<List<ApplicantDocumentListResponse>>>.Success(documents);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting documents: {ex.Message}", nameof(GetAllListAsync));
                return ResponseModel<CustomPagination<List<ApplicantDocumentListResponse>>>.Exception("Exception error " + ex.Message);
            }

        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                //check for document Id to validate document here
                var record = await _applicantDocumentRepository.GetByAsync(X => X.Id == id);

                if (record == null)
                {
                    return ResponseModel<bool>.Failure($"Document with id {id} not found");
                }

                record.IsDeleted = true;

                _applicantDocumentRepository.Update(record);
                await _applicantDocumentRepository.SaveChangesAsync();

                return ResponseModel<bool>.Success(true);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while deleting document: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<bool>.Exception("Exception error " + ex.Message);
            }
        }
    }
}