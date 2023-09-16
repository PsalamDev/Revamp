using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJobPreferenceService
    {
        Task<ResponseModel<JobPreferenceModel>> CreateAsync(CreateJobPreferenceDto request);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
        Task<ResponseModel<CustomPagination<List<JobPreferenceDto>>>> GetAllAsync(JobPreferenceFilter filter);
        Task<ResponseModel<JobPreferenceDto>> GetSingleAsync(Guid id);
        Task<ResponseModel<JobPreferenceModel>> UpdateAsync(UpdateJobPreferencDto request);

    }
}
