using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Authorization
{
    public class InterViewerPermissions
    {
        [DisplayName("Interviewer")]
        [Description("Interviewer Permissions")]
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
