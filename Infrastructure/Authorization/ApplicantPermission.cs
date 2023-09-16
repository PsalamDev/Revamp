using System.ComponentModel;

namespace Infrastructure.Authorization
{
    public class ApplicantPermission
    {

        [DisplayName("Applicant")]
        [Description("Applicant Permissions")]
        public static class Applicant
        {
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }

        [DisplayName("Job")]
        [Description("Job Permissions")]
        public static class Job
        {
            public const string Apply = "Apply";
        }

    }
}
