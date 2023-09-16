using Core.Common.Model;
using Core.Common.Model.RecruitmentDto;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJobService
    {
        Task<ResponseModel<JobModel>> CreateAsync(CreateJobDto request);
        Task<ResponseModel<JobModel>> UpdateAsync(UpdateJobDto request);
        Task<ResponseModel<JobDto>> GetSingleAsync(Guid id);
        Task<ResponseModel<bool>> DeleteAsync(Guid Id);
        Task<ResponseModel<CustomPagination<List<JobDto>>>> GetAllAsync(JobFilterDto filter);
        Task<ResponseModel<DashboardChartModel>> DashBoardData(DashBoardFilter filter);
        Task<ResponseModel<List<DashBoardListGraphData>>> RecruitmentOverTime(string? timePeriod = null,
                                    DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<List<DashBoardListGraphData>>> OpenJobRolesByDepartment(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<List<DashBoardListGraphData>>> JobsPerRole(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<List<DashBoardListGraphData>>> ApplicantDistributionByAge(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<List<RoleSkill>>> DistributionBySkillSets(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<EmployeeTurnOverDashBoardListGraphData>> EmployeeTurnOver(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<List<DashBoardListGraphData>>> SourceOfApplication(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<CustomPagination<List<JobPostReviewDto>>>> JobPostReview(JobPostReviewfilterDto filter);
        Task<ResponseModel<string>> JobPostReviewComment(CreateJobReviwerComent request);
    }
}
