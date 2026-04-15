namespace HikeConnect.WebApp.Routing
{
    public static class ApiRoutes
    {
        public static class Auth
        {
            public const string Login = "auth/login";
            public const string Logout = "auth/logout";
            public const string Register = "auth/register";
            public const string Refresh = "auth/refresh";
            public const string UserById = "auth/user/";
            public const string UserByUsername = "auth/user/username/";
            public const string UpdateBio = "auth/user/bio";
        }

        // Сегменты совпадают с api/[controller] в ASP.NET (имя класса без «Controller», без дефисов).
        public static class BehavioralProfile
        {
            public const string Base = "behavioralprofile";
            public const string ById = "behavioralprofile/";
            public const string ByUserId = "behavioralprofile/user/";
        }

        public static class CompatibilityReport
        {
            public const string Base = "compatibilityreport";
            public const string ById = "compatibilityreport/";
            public const string ByTargetId = "compatibilityreport/target/";
            public const string ByAuthorId = "compatibilityreport/author/";
        }

        public static class ParticipationRequest
        {
            public const string ById = "participationrequest/";
            public const string ByTripId = "participationrequest/trip/";
            public const string ByUserId = "participationrequest/user/";
            public static string Approve(Guid id) => $"participationrequest/{id}/approve";
            public static string Reject(Guid id) => $"participationrequest/{id}/reject";
            public static string Cancel(Guid id) => $"participationrequest/{id}/cancel";
        }

        public static class Trip
        {
            public const string Base = "trip";
            public const string ById = "trip/";
            public static string Publish(Guid id) => $"trip/{id}/publish";
            public static string Unpublish(Guid id) => $"trip/{id}/unpublish";
        }

        public static class Test
        {
            public const string Base = "test";
        }
    }
}
