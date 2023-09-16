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
    public class ApplicantQualificationService : IApplicantQualification
    {

        private readonly IAsyncRepository<ApplicantEducationHistory, Guid> _applicantEduHistoryRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicantQualificationService> _logger;
        private readonly ApplicationDbContext _context;
        public ApplicantQualificationService(IAsyncRepository<ApplicantEducationHistory, Guid> applicantEduHistoryRepository, ICurrentUser currentUser, IMapper mapper,
            ILogger<ApplicantQualificationService> logger, ApplicationDbContext context)
        {
            _applicantEduHistoryRepository = applicantEduHistoryRepository;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
            _context = context;
        }


        public async Task<ResponseModel<ApplicantQualificationResponse>> CreateAsync(ApplicantQualificationRequest request)
        {
            var companyId = Guid.Parse(_currentUser.GetCompany());

            var applicantQualification = new ApplicantEducationHistory()
            {
                Id = SequentialGuid.Create(),
                ProgramTypeName = request.ProgramTypeName,
                QualificationType = request.QualificationType,
                CGPA = request.CGPA,
                CourseId = request.CourseId,
                CourseName = request.CourseName,
                GradeId = !string.IsNullOrEmpty(request.GradeId) ? Guid.Parse(request.GradeId) : Guid.Empty,
                GradeName = request.GradeName,
                ApplicantId = request.ApplicantId,
                EndDate = request.EndDate,
                StartDate = request.StartDate,
                InstitutionName = request.InstitutionName,
                QualificationDegreeName = request.QualificationDegreeName,
                IsOngoing = request.IsOngoing,
                CreatedBy = _currentUser.GetUserId(),
                CreatedByName = _currentUser.GetFullname(),
                CreatedDate = DateTime.Now,
                Country = request.Country
            };

            _applicantEduHistoryRepository.Add(applicantQualification);
            await _applicantEduHistoryRepository.SaveChangesAsync();

            return ResponseModel<ApplicantQualificationResponse>.Success(_mapper.Map<ApplicantQualificationResponse>(applicantQualification));
        }

        public async Task<ResponseModel<ApplicantQualificationResponse>> UpdateAsync(UpdateApplicantQualificationRequest request)
        {
            try
            {
                var appQualification = await _applicantEduHistoryRepository.GetByAsync(x => x.Id == request.Id && x.IsDeleted == false);
                if (appQualification == null)
                {
                    return ResponseModel<ApplicantQualificationResponse>.Failure($"{request.ApplicantId} qualification record not found");
                }

                appQualification.ProgramTypeName = request.ProgramTypeName;
                appQualification.QualificationType = request.QualificationType;
                appQualification.QualificationDegreeName = request.QualificationDegreeName;
                appQualification.CGPA = request.CGPA;
                appQualification.GradeId = !string.IsNullOrEmpty(request.GradeId) ? Guid.Parse(request.GradeId) : Guid.Empty;
                appQualification.GradeName = request.GradeName;
                appQualification.CourseId = request.CourseId;
                appQualification.CourseName = request.CourseName;
                appQualification.ApplicantId = request.ApplicantId;
                appQualification.EndDate = request.EndDate;
                appQualification.StartDate = request.StartDate;
                appQualification.InstitutionName = request.InstitutionName;
                appQualification.IsOngoing = request.IsOngoing;
                appQualification.ModifiedBy = _currentUser.GetUserId().ToString();
                appQualification.CreatedDate = DateTime.Now;
                appQualification.Country = request.Country;

                _applicantEduHistoryRepository.Update(appQualification);
                await _applicantEduHistoryRepository.SaveChangesAsync();

                return ResponseModel<ApplicantQualificationResponse>.Success(_mapper.Map<ApplicantQualificationResponse>(appQualification));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving qualification: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ApplicantQualificationResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<ApplicantQualificationResponse>> GetSingleAsync(Guid id)
        {
            try
            {


                using (_context)
                {
                    var empQualif = await _context.ApplicantWorkHistories.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (empQualif == null)
                    {
                        return ResponseModel<ApplicantQualificationResponse>.Failure($"cannot find qualification records for this employee");
                    }



                    return ResponseModel<ApplicantQualificationResponse>.Success(_mapper.Map<ApplicantQualificationResponse>(empQualif));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting employee qualification: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<ApplicantQualificationResponse>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<CustomPagination<List<ApplicantQualificationListResponse>>>> GetAllListAsync(QualificationListRequest req)
        {
            try
            {
                using (_context)
                {

                    var empQualifications = await (from empQua in _context.ApplicantEducationHistories
                                                   where empQua.ApplicantId == req.ApplicantId && empQua.IsDeleted == false
                                                   select new ApplicantQualificationListResponse()
                                                   {
                                                       Id = empQua.Id,
                                                       ProgramTypeName = empQua.ProgramTypeName,
                                                       QualificationDegreeName = empQua.QualificationDegreeName,
                                                       CGPA = empQua.CGPA,
                                                       ApplicantId = empQua.ApplicantId,
                                                       EndDate = empQua.EndDate,
                                                       StartDate = empQua.StartDate,
                                                       InstitutionName = empQua.InstitutionName,
                                                       CourseName = empQua.CourseName,
                                                       GradeName = empQua.GradeName,
                                                       IsOngoing = empQua.IsOngoing,
                                                       CreatedBy = empQua.CreatedBy,
                                                       CreatedDate = empQua.CreatedDate,
                                                       CourseId = empQua.CourseId,
                                                       GradeId = empQua.GradeId,
                                                       IsDeleted = empQua.IsDeleted,
                                                       CreatedByIp = empQua.CreatedByIp,
                                                       ModifiedBy = empQua.ModifiedBy,
                                                       ModifiedByIp = empQua.ModifiedByIp,
                                                       ModifiedDate = empQua.ModifiedDate,
                                                       Country = empQua.Country
                                                   }).ToListAsync();

                    CustomPagination<List<ApplicantQualificationListResponse>> model = new CustomPagination<List<ApplicantQualificationListResponse>>
                    {

                        modelresult = empQualifications,
                        TotalCount = empQualifications.Count,
                        pageNumber = 0,
                        pageSize = 10000

                    };


                    return ResponseModel<CustomPagination<List<ApplicantQualificationListResponse>>>.Success(model);

                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant qualifications: {ex.Message}", nameof(GetAllListAsync));
                return ResponseModel<CustomPagination<List<ApplicantQualificationListResponse>>>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {

                using (_context)
                {
                    var record = await _context.ApplicantEducationHistories.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();

                    if (record == null)
                    {
                        return ResponseModel<bool>.Failure($"qualification record not found");
                    }

                    record.IsDeleted = true;
                    _applicantEduHistoryRepository.Update(record);
                    await _context.SaveChangesAsync();
                    return ResponseModel<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating applicats qualifications: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<bool>.Exception("Exception error " + ex.Message);
            }
        }
    }
}