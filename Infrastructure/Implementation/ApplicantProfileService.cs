using AutoMapper;
using Core.Common.Model;
using Core.Common.Model.IdentityModels.Identity;
using Core.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Grpc.Net.Client;
using HRShared.Common;
using HRShared.CoreProviders.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Infrastructure.Implementation
{
    public class ApplicantProfileService : IApplicantProfile
    {

        private readonly IAsyncRepository<ApplicantProfile, Guid> _applicantProfileRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicantProfileService> _logger;
        private readonly ApplicationDbContext _context;

        private readonly IAzureStorageServices _azureStorageService;
        public ApplicantProfileService(IAsyncRepository<ApplicantProfile, Guid> applicantProfileRepository, ICurrentUser currentUser, IMapper mapper,
            ILogger<ApplicantProfileService> logger, ApplicationDbContext context, IAzureStorageServices azureStorageService)
        {
            _applicantProfileRepository = applicantProfileRepository;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
            _context = context;
            _azureStorageService = azureStorageService;

            //companyId = Guid.Parse(_currentUser.GetCompany());
        }

        public async Task<ResponseModel<ApplicantProfileResponse>> CreateAsync(ApplicantProfileRequest request)
        {
            try
            {
                using (_context)
                {
                    var companyId = Guid.Parse(_currentUser.GetCompany());

                    var profileExist = await _context.ApplicantProfiles
                        .Where(x => x.CompanyId == request.CompanyId && x.Email == request.PersonalEmail)
                        .FirstOrDefaultAsync();

                    if (profileExist != null)
                    {
                        return ResponseModel<ApplicantProfileResponse>.Failure($"{request.PersonalEmail} profile already exists on this recruitment portal for the client");
                    }

                    var newUser = new RegisterUserModel
                    {
                        Email = request.PersonalEmail,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        UserName = request.PersonalEmail,
                        PhoneNumber = request.PhoneNumber,
                       
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    var applicantProfile = new ApplicantProfile()
                    {
                        Id = SequentialGuid.Create(),
                        CompanyId = request.CompanyId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Social = string.Empty,
                        Website = request.Website,
                        Twitter = request.Twitter,
                        Linkedin = request.Linkedin,
                        Instagram = request.Instagram,
                        Address = request.Address,
                        AgeRange = request.AgeRange,
                        Email = request.PersonalEmail,
                        DateOfBirth = request.DateOfBirth,
                        CountryCode = request.CountryCode,
                        PhoneNumber = request.PhoneNumber,
                        CreatedBy = Guid.Empty,
                        CreatedByName = "AUTO-SYSTEM",
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                        UserId = newUser.Id 
                    };

                    await _context.ApplicantProfiles.AddAsync(applicantProfile);
                    await _context.SaveChangesAsync();

                    return ResponseModel<ApplicantProfileResponse>.Success(_mapper.Map<ApplicantProfileResponse>(applicantProfile));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occurred while saving employee profile: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<ApplicantProfileResponse>.Exception("Exception error " + ex.Message);
            }
        }


        public async Task<ResponseModel<ApplicantProfileResponse>> UploadApplicantProfileImage(ApplicantProfileImage request)
        {
            var companyId = Guid.Parse(_currentUser.GetCompany());
            var applicantProfile = await _applicantProfileRepository.GetByAsync(x => x.Id == request.ApplicantId && x.CompanyId == companyId);
            if (applicantProfile == null)
            {
                return ResponseModel<ApplicantProfileResponse>.Failure("Applicant Id resource not found");
            }

            var imageUrl = await _azureStorageService.UploadToAzureAsync(request.image);
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return ResponseModel<ApplicantProfileResponse>.Failure("Fail to upload image please try again");
            }

            applicantProfile.ModifiedBy = _currentUser.GetFullname();
            applicantProfile.ModifiedDate = DateTime.UtcNow;
            applicantProfile.ProfileImage = imageUrl;

            _applicantProfileRepository.Update(applicantProfile);
            await _applicantProfileRepository.SaveChangesAsync();
            return ResponseModel<ApplicantProfileResponse>.Success(_mapper.Map<ApplicantProfileResponse>(applicantProfile));
        }

        public async Task<ResponseModel<ApplicantProfileResponse>> UpdateAsync(UpdateApplicantProfileRequest request)
        {
            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());
                var applicantProfile = await _applicantProfileRepository.GetByAsync(x => x.Id == request.Id && x.CompanyId == companyId);

                if (applicantProfile == null)
                {
                    return ResponseModel<ApplicantProfileResponse>.Failure($"{request.ApplicantId} profile record not found");
                }


               // check if user exist in organization before adding
                var profilePhoneNumberExist = await _context.ApplicantProfiles.Where(x => x.CompanyId == companyId && x.PhoneNumber
                == request.PhoneNumber).FirstOrDefaultAsync();

                if (profilePhoneNumberExist != null)
                {
                    return ResponseModel<ApplicantProfileResponse>.Failure($"Phone number already exist");
                }


                applicantProfile.FirstName = request.FirstName;
                applicantProfile.LastName = request.LastName;
                applicantProfile.CountryCode = request.CountryCode;
                applicantProfile.DateOfBirth = request.DateOfBirth;
                applicantProfile.PhoneNumber = request.PhoneNumber;
                applicantProfile.ModifiedBy = _currentUser.GetFullname();
                applicantProfile.ModifiedDate = DateTime.Now;
                applicantProfile.Twitter = request.Twitter;
                applicantProfile.Linkedin = request.Linkedin;
                applicantProfile.Instagram = request.Instagram;
                applicantProfile.Address = request.Address;
                applicantProfile.AgeRange = request.AgeRange;
                applicantProfile.Website = request.Website;


                _applicantProfileRepository.Update(applicantProfile);
                await _applicantProfileRepository.SaveChangesAsync();



                return ResponseModel<ApplicantProfileResponse>.Success(_mapper.Map<ApplicantProfileResponse>(applicantProfile));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving profile: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ApplicantProfileResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<ApplicantProfileResponse>> GetSingleAsync(Guid id)
        {
            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());

                using (_context)
                {
                    var appProfile = await _context.ApplicantProfiles.Where(x => x.Id == id && x.CompanyId == companyId).FirstOrDefaultAsync();
                    if (appProfile == null)
                    {
                        return ResponseModel<ApplicantProfileResponse>.Failure($"cannot find applicant records for this employee");
                    }
                    return ResponseModel<ApplicantProfileResponse>.Success(_mapper.Map<ApplicantProfileResponse>(appProfile));
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant profile: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<ApplicantProfileResponse>.Exception("Exception error " + ex.Message);
            }
        }
        public async Task<ResponseModel<ApplicantProfileResponse>> GetSingleByEmailAsync(string email)
        {

            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());
                using (_context)
                {
                    var appProfile = await _context.ApplicantProfiles.Where(x => x.Email == email && x.CompanyId == companyId).FirstOrDefaultAsync();
                    if (appProfile == null)
                    {
                        return ResponseModel<ApplicantProfileResponse>.Failure($"cannot find applicant records for this employee");
                    }
                    return ResponseModel<ApplicantProfileResponse>.Success(_mapper.Map<ApplicantProfileResponse>(appProfile));
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant profile: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<ApplicantProfileResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>> GetAllListAsync(ProfileListRequest request)
        {
            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());
                using (_context)
                {

                    var applicantRecords = await (from empProf in _context.ApplicantProfiles

                                                  where empProf.CompanyId == companyId && empProf.IsDeleted == false
                                                  select new ApplicantProfileListResponse()
                                                  {

                                                      Id = empProf.Id,
                                                      FullName = empProf.FirstName + " " + empProf.LastName,
                                                      PersonalEmail = empProf.Email,
                                                      DateOfBirth = empProf.DateOfBirth,
                                                      CountryCode = empProf.CountryCode,
                                                      PhoneNumber = empProf.PhoneNumber,
                                                      ProfileImage = empProf.ProfileImage,
                                                      UserId = empProf.UserId,
                                                      CreatedDate = empProf.CreatedDate,


                                                  }).ToListAsync();


                    var totalCount = applicantRecords.Count;

                    // if (request.StartDate != DateTime.MinValue)
                    // {
                    //     empRecords = empRecords
                    //         .Where(x => x.CreatedDate >= request.StartDate && x.CreatedDate <= request.EndDate).ToList();
                    // }

                    if (!string.IsNullOrEmpty(request.Email))
                    {
                        applicantRecords = applicantRecords.Where(x => x.PersonalEmail.ToLower().Contains(request.Email.ToLower())).ToList();
                    }




                    CustomPagination<List<ApplicantProfileListResponse>> model = new CustomPagination<List<ApplicantProfileListResponse>>
                    {
                        TotalCount = totalCount,
                        pageSize = request.PageSize,
                        pageNumber = request.PageNumber,
                        modelresult = applicantRecords.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList()
                    };

                    return ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>.Success(model);

                }

            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured while getting list of company applicants profiles: {ex.Message}", nameof(GetAllListAsync));
                return ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>.Exception("Exception error " + ex.Message);

            }
        }

        public async Task<ResponseModel<DocumentResponseDTO>> DownloadApplicantCV(Guid applicantId)
        {
            var response = new DocumentResponseDTO();
            try
            {
                var companyId = Guid.Parse(_currentUser.GetCompany());
                var fileDetails = await _context.ApplicantDocuments.Where(c => c.ApplicantId == applicantId && (c.DocuemntTypeName.ToLower().Contains("Resume")
                || c.DocuemntTypeName.ToLower().Contains("CV") || c.DocuemntTypeName.ToLower().Contains("Curriculum Vitae"))).FirstOrDefaultAsync();

                if (fileDetails == null)
                    return ResponseModel<DocumentResponseDTO>.Failure("null");

                var retUrl = fileDetails.FileUrl;
                var retFileName = fileDetails.FileName;

                var fileName = retFileName;
                var filePath = retUrl;//new Uri(filePath);

                response.FilePath = filePath;
                response.FileName = fileName;
                return ResponseModel<DocumentResponseDTO>.Success(response);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant CV: {ex.Message}", nameof(GetAllListAsync));
                return ResponseModel<DocumentResponseDTO>.Failure("Exception error " + ex.Message);
            }
        }

        public Task<ResponseModel<ApplicantProfileFullDetailsResponse>> GetCompleteSingleApplicantProfileAsync(Guid id)
        {
            throw new NotImplementedException();
        }



        public async Task<ResponseModel<bool>> DeleteApplicanKeywordAsync(Guid id)
        {
            try
            {

                var companyId = Guid.Parse(_currentUser.GetCompany());

                var keywordExist = await _context.ApplicantJobSearchKeywords.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false && x.CompanyId == companyId);

                if (keywordExist == null)
                {
                    return ResponseModel<bool>.Failure("Applicant keyword not found.");
                }

                keywordExist.IsDeleted = true;
                keywordExist.ModifiedBy = _currentUser.GetFullname();
                keywordExist.ModifiedDate = DateTime.Now;
                _context.Update(keywordExist);
                await _context.SaveChangesAsync();

                return ResponseModel<bool>.Success(_mapper.Map<bool>(true));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant keyword record by id: {ex.Message}", nameof(DeleteApplicanKeywordAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<List<JobWordDto>>> LoadApplicantKeyword(Guid applicantId)
        {

            var result = new List<JobWordDto>();
            try
            {

                var companyId = Guid.Parse(_currentUser.GetCompany());
                result = await _context.ApplicantJobSearchKeywords.Where(x => x.ApplicantProfileId == applicantId && x.IsDeleted == false && x.CompanyId == companyId)
               .Select(x => new JobWordDto
               {
                   ApplicantId = x.ApplicantProfileId,
                   Id = x.Id,
                   AppKeyword = x.Jobkeyword
               }).ToListAsync();

                if (result != null)
                {
                    return ResponseModel<List<JobWordDto>>.Success(result);
                }
                else
                {
                    return ResponseModel<List<JobWordDto>>.Success(result, "No result");
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting list of applicants keyword: {ex.Message}", nameof(LoadApplicantKeyword));
                return ResponseModel<List<JobWordDto>>.Failure("Applicant keyword not found.");
            }
        }

    }
}