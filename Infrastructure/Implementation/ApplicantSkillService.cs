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
    public class ApplicantSkillService : IApplicantSkill
    {

        private readonly IAsyncRepository<ApplicantSkill, Guid> _applicantSkillRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicantSkillService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly Guid companyId;
        public ApplicantSkillService(IAsyncRepository<ApplicantSkill, Guid> applicantSkillRepository, ICurrentUser currentUser, IMapper mapper,
            ILogger<ApplicantSkillService> logger, ApplicationDbContext context)
        {
            _applicantSkillRepository = applicantSkillRepository;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
            _context = context;
            companyId = Guid.Parse(_currentUser.GetCompany());
        }

        public async Task<ResponseModel<string>> CreateAsync(ApplicantSkillRequest request)
        {
            try
            {

                if (request.SkillNames.Count() > 6)
                {
                    return ResponseModel<string>.Failure("You can only add six (6) skills");
                }

                var skillNames = await _context.ApplicantSkills.Where(x => x.IsDeleted == false && x.ApplicantsId == request.ApplicantId
                && x.CompanyId == companyId).ToListAsync();

                if (skillNames.Count() > 6)
                {
                    return ResponseModel<string>.Failure($"You can only add six (6) skills.");
                }
                else
                {
                    foreach (var skill in request.SkillNames)
                    {
                        var employeeSkill = new ApplicantSkill()
                        {
                            Id = SequentialGuid.Create(),
                            ApplicantsId = request.ApplicantId,
                            CompanyId = companyId,
                            SkillName = skill,
                            CreatedBy = _currentUser.GetUserId(),
                            CreatedByName = _currentUser.GetFullname(),
                            CreatedDate = DateTime.Now
                        };

                        _applicantSkillRepository.Add(employeeSkill);
                        await _applicantSkillRepository.SaveChangesAsync();
                    }

                    return ResponseModel<string>.Success("Created Succefully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving applicant skills: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<string>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<string>> UpdateAsync(UpdateApplicantSkillRequest request)
        {
            try
            {

                var appRefSkill = await _context.ApplicantSkills.Where(x => x.ApplicantsId == request.ApplicantId).ToListAsync();
                if (appRefSkill == null)
                {
                    return ResponseModel<string>.Failure($"{request.ApplicantId} skill record not found");
                }


                if (appRefSkill.Count() > 0)
                {
                    _applicantSkillRepository.DeleteList(appRefSkill);
                    await _applicantSkillRepository.SaveChangesAsync();
                }


                foreach (var skill in request.SkillNames)
                {
                    var employeeSkill = new ApplicantSkill()
                    {
                        Id = SequentialGuid.Create(),
                        ApplicantsId = request.ApplicantId,
                        CompanyId = companyId,
                        SkillName = skill,
                        CreatedBy = _currentUser.GetUserId(),
                        CreatedByName = _currentUser.GetFullname(),
                        CreatedDate = DateTime.Now
                    };

                    _applicantSkillRepository.Add(employeeSkill);
                    await _applicantSkillRepository.SaveChangesAsync();
                }

                return ResponseModel<string>.Success("Updated Successfully");

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving skill: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<string>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<List<ApplicantSkillResponse>>> GetListAsync(ApplicantSkillFilter filter)
        {
            try
            {
                using (_context)
                {
                    var appSkills = await (from es in _context.ApplicantSkills
                                           where es.ApplicantsId == filter.ApplicantId && es.IsDeleted == false
                                           select new ApplicantSkillResponse()
                                           {
                                               Id = es.Id,
                                               ApplicantId = es.ApplicantsId,
                                               SkillName = es.SkillName,
                                               CreatedBy = es.CreatedBy,
                                               CreatedDate = es.CreatedDate
                                           }).ToListAsync();
                    ;
                    return ResponseModel<List<ApplicantSkillResponse>>.Success(appSkills);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting applicant skills: {ex.Message}", nameof(GetListAsync));
                return ResponseModel<List<ApplicantSkillResponse>>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<bool>> DeleteAsync(Guid skillId)
        {
            try
            {
                var record = await _context.ApplicantSkills.Where(x => x.Id == skillId).FirstOrDefaultAsync();

                if (record == null)
                {
                    return ResponseModel<bool>.Failure("Skill not found.");
                }

                record.IsDeleted = true;
                _context.ApplicantSkills.Update(record);
                await _context.SaveChangesAsync();
                return ResponseModel<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating applicant skill: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<bool>.Exception("Exception error " + ex.Message);
            }
        }

        public async Task<ResponseModel<List<ApplicantSkillNameSuggestion>>> GetSkillNameSuggestions()
        {

            try
            {
                var skillNames = await _context.ApplicantSkills.Where(x => x.IsDeleted == false && x.CompanyId == companyId).Select(x => new ApplicantSkillNameSuggestion
                {
                    SkillNames = x.SkillName
                }).Distinct().ToListAsync();

                if (skillNames.Count() > 0)
                {
                    return ResponseModel<List<ApplicantSkillNameSuggestion>>.Success(skillNames);
                }
                else
                {
                    return ResponseModel<List<ApplicantSkillNameSuggestion>>.Success(skillNames, "No result found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting list applicant skill: {ex.Message}", nameof(GetSkillNameSuggestions));
                return ResponseModel<List<ApplicantSkillNameSuggestion>>.Exception("Exception error " + ex.Message);
            }
        }
    }
}