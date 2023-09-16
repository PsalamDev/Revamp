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
    public class ApplicantReferenceService : IApplicantReference
    {

        private readonly IAsyncRepository<ApplicantReference, Guid> _applicantReferenceRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicantReferenceService> _logger;
        private readonly ApplicationDbContext _context;

        private readonly IAzureStorageServices _azureStorageService;
        public ApplicantReferenceService(IAsyncRepository<ApplicantReference, Guid> applicantReferenceRepository, ICurrentUser currentUser, IMapper mapper,
            ILogger<ApplicantReferenceService> logger, ApplicationDbContext context, IAzureStorageServices azureStorageService)
        {
            _applicantReferenceRepository = applicantReferenceRepository;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
            _context = context;
            _azureStorageService = azureStorageService;
        }

        public async Task<ResponseModel<ApplicantReferenceResponse>> CreateAsync(ApplicantReferenceRequest request)
        {
            try
            {
                using (_context)
                {
                    var applicantReference = new ApplicantReference()
                    {

                        Id = SequentialGuid.Create(),
                        CompanyId = Guid.Parse(_currentUser.GetCompany()),
                        FullName = request.FullName,
                        Profession = request.Profession,
                        PlaceOfWork = request.PlaceOfWork,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        CreatedBy = Guid.Empty,
                        CreatedByName = "AUTO-SYSTEM",
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                        JobApplicantId = request.JobApplicantId,
                        Address = request.Address
                    };

                    await _context.ApplicantReferences.AddAsync(applicantReference);

                    await _context.SaveChangesAsync();

                    return ResponseModel<ApplicantReferenceResponse>.Success(_mapper.Map<ApplicantReferenceResponse>(applicantReference));

                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving employee reference: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<ApplicantReferenceResponse>.Exception("Exception error " + ex.Message);
            }
        }


        public async Task<ResponseModel<ApplicantReferenceResponse>> UpdateAsync(UpdateApplicantReferenceRequest request)
        {
            try
            {

                var appReference = await _applicantReferenceRepository.GetByAsync(x => x.Id == request.Id);

                if (appReference == null)
                {
                    return ResponseModel<ApplicantReferenceResponse>.Failure($"{request.Id} profile record not found");
                }


                appReference.Email = request.Email;
                appReference.Profession = request.Profession;
                appReference.CompanyId = Guid.Parse(_currentUser.GetCompany());
                appReference.FullName = request.FullName;
                appReference.PhoneNumber = request.PhoneNumber;
                appReference.PlaceOfWork = request.PlaceOfWork;
                appReference.Address = request.Address;



                _applicantReferenceRepository.Update(appReference);
                await _applicantReferenceRepository.SaveChangesAsync();



                return ResponseModel<ApplicantReferenceResponse>.Success(_mapper.Map<ApplicantReferenceResponse>(appReference));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving application reference: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ApplicantReferenceResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<ApplicantReferenceResponse>> GetSingleAsync(Guid id)
        {
            try
            {

                using (_context)
                {
                    var appReference = await _context.ApplicantReferences.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (appReference == null)
                    {
                        return ResponseModel<ApplicantReferenceResponse>.Failure($"cannot find applicant reference for this employee");
                    }
                    return ResponseModel<ApplicantReferenceResponse>.Success(_mapper.Map<ApplicantReferenceResponse>(appReference));
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant reference: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<ApplicantReferenceResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>> GetAllListAsync(ReferenceListRequest request)
        {
            try
            {
                using (_context)
                {

                    var applicantReferences = await (from appRef in _context.ApplicantReferences

                                                     where appRef.JobApplicantId == request.ApplicantId && appRef.IsDeleted == false
                                                     select new ApplicantReferenceListResponse()
                                                     {
                                                         Id = appRef.Id,
                                                         FullName = appRef.FullName,
                                                         Email = appRef.Email,
                                                         PhoneNumber = appRef.PhoneNumber,
                                                         PlaceOfWork = appRef.PlaceOfWork,
                                                         Profession = appRef.Profession,
                                                         CreatedDate = appRef.CreatedDate,
                                                         JobApplicantId = request.ApplicantId,
                                                         CompanyId = Guid.Parse(_currentUser.GetCompany()),
                                                         Address = appRef.Address
                                                     }).ToListAsync();


                    var totalCount = applicantReferences.Count;

                    CustomPagination<List<ApplicantReferenceListResponse>> model = new CustomPagination<List<ApplicantReferenceListResponse>>
                    {
                        TotalCount = totalCount,
                        pageSize = request.PageSize,
                        pageNumber = request.PageNumber,
                        modelresult = applicantReferences.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList()
                    };

                    return ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>.Success(model);

                }

            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while getting list of  applicant references: {ex.Message}", nameof(GetAllListAsync));
                return ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>.Exception("Exception error " + ex.Message);

            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                using (_context)
                {
                    var record = await _context.ApplicantReferences.Where(x => x.Id == id).FirstOrDefaultAsync();
                    record.IsDeleted = true;
                    _context.ApplicantReferences.Update(record);
                    await _context.SaveChangesAsync();

                    return ResponseModel<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while deting applicant reference: {ex.Message}", nameof(DeleteAsync));
                return ResponseModel<bool>.Exception("Exception error " + ex.Message);
            }
        }

    }
}