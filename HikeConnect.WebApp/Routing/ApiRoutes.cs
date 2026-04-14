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

        public static class BehavioralProfile
        {
            public const string Base = "behavioral-profile";
            public const string ById = "behavioral-profile/";
            public const string ByUserId = "behavioral-profile/user/";
        }

        public static class CompatibilityReport
        {
            public const string Base = "compatibility-report";
            public const string ById = "compatibility-report/";
            public const string ByTargetId = "compatibility-report/target/";
            public const string ByAuthorId = "compatibility-report/author/";
        }

        public static class ParticipationRequest
        {
            public const string ById = "participation-request/";
            public const string ByTripId = "participation-request/trip/";
            public const string ByUserId = "participation-request/user/";
            public static string Approve(Guid id) => $"participation-request/{id}/approve";
            public static string Reject(Guid id) => $"participation-request/{id}/reject";
            public static string Cancel(Guid id) => $"participation-request/{id}/cancel";
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
