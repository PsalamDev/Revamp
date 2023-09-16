using System.ComponentModel;

namespace Infrastructure.Authorization
{
    public class RecruitmentApplicationPermissions
    {
        [DisplayName("Recruitment")]
        [Description("Recruitment Permissions")]
        public static class RecruitmentApplication
        {
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
            public const string Approve = "Approve";
        }

        [DisplayName("Job")]
        [Description("Job permissions")]
        public static class Confirmation
        {
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
            public const string Approve = "Approve";
            public const string Reject = "Reject";
        }

        [DisplayName("HiringStage")]
        [Description("HiringStage permissions")]
        public static class HiringStage
        {
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
            public const string Approve = "Approve";
            public const string Reject = "Reject";
        }

        [DisplayName("Quiz")]
        [Description("Quiz permissions")]
        public static class Quiz
        {
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }

        [DisplayName("Focus")]
        [Description("FocusArea permissions")]
        public static class FocusArea
        {
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }

        [DisplayName("ScoreCard")]
        [Description("ScoreCard permissions")]
        public static class ScoreCard
        {
            public const string View = "View";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }



        [DisplayName("RecruitmentAdmin")]
        [Description("RecruitmentAdmin Permissions")]
        public static class Interviewer
        {
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
            public const string Approve = "Approve";
            public const string Reject = "Reject";
            public const string View = "View";
        }
    }


}
