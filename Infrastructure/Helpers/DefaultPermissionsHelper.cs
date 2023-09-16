using Infrastructure.Authorization;

namespace Infrastructure.Helpers
{
    public static class DefaultPermissionsHelper
    {
        public static Type[] AdminPermissionTypes => typeof(RecruitmentApplicationPermissions).GetNestedTypes();
        public static Type[] SuperPermissionTypes => typeof(InterViewerPermissions).GetNestedTypes();
    }
}
