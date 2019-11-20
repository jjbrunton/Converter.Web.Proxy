using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace ConversionProxy.Filters
{
    public class AnonymousDashboardFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}