using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Burn_Out.AcceptanceTests; // namespace where TestWebApplicationFactory lives

[assembly: Reqnroll.Plugins.Plugin]

namespace Burn_Out.AcceptanceTestsReq.ReqnrollHooks
{
    public static class TestDependencies
    {
        [Reqnroll.Binding]
        public static void ConfigureServices(IServiceCollection services)
        {
            // Single shared factory for all steps
            services.AddSingleton<TestWebApplicationFactory>();
        }
    }
}