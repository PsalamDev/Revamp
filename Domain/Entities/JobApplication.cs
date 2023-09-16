using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.Enum;

namespace Domain.Entities
{
    public class JobApplication : BaseEntity
    {
        public Guid JobId { get; set; }
        public string ApplicantCode { get; set; }
        public Guid JobApplicantId { get; set; }
        public DateTime DateApplied { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool AlertSent { get; set; }
        public string CoverLetter { get; set; }
        public int? JobLocation { get; set; }
        public bool IsHired { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime? DateRecruited { get; set; }

        /// <summary>
        /// This Rep. the status of the Application, and what Admin's Action
        /// resulted to when performing Job Screening (i.e. Shortlisted, Interviewed etc)
        /// </summary>
        public Guid? HireStageId { get; set; }

        public string TestPassCode { get; set; }

        public decimal? ScoreCardValue { get; set; }
        public string? ScoreCardDetail { get; set; }
        public Guid? SubhireStageId { get; set; }
        public bool IsInProgress { get; set; }
        public Guid CompanyId { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? Channel { get; set; }
        public Job Job { get; set; }
        public ApplicantProfile JobApplicant { get; set; }
        public Stage? Stage { get; set; }
        public ICollection<JobInterviewHistory>? InterviewHistory { get; set; } = new List<JobInterviewHistory>();
        public ICollection<JobApplicationStageHistory>? JobApplicationStageHistories { get; set; } = new List<JobApplicationStageHistory>();
    }
}