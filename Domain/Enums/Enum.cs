using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public class Enum
    {
        public enum InterViewStatus
        {
            Open = 1,
            Closed
        }
        public enum JobStatus
        {
            Open = 1,
            Closed
        }

        public enum ApplicationStatus
        {
            InterviewStage = 1,
            Offered,
            Pending,
            Shortlisted
        }

        public enum EmploymentType
        {
            FullTime = 1,
            Contract,
            PartTime
        }

        public enum JobAvailablity
        {
            Physical = 1,
            Remote,
            Flexible
        }
        public enum JobExperienceLevel
        {
            EntryLevel = 1,
            MidLevel,
            SeniorLevel
        }

        public enum JobPostStatus
        {
            Post = 1,
            Draft,
            Schedule
        }

        public enum RecruitmentActionStatus
        {
            Shortlist = 1,
            Offer,
            Reject
        }

        public enum ReviwerStatus
        {
            Pending = 1,
            Reviewed
        }

    }

}
