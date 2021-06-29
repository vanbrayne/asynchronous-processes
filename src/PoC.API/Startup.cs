using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexus.Link.Libraries.Core.Application;
using Nexus.Link.Libraries.Core.MultiTenant.Model;
using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.API.RestClients.CommunicationMgmtCapability;
using PoC.API.RestClients.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;
using PoC.Example.Capabilities.CommunicationMgmtCapability;
using PoC.Example.Capabilities.CustomerInformationMgmt;
using PoC.Example.Capabilities.CustomerOnboardingMgmtCapability;
using FulcrumApplicationHelper = Nexus.Link.Libraries.Web.AspNet.Application.FulcrumApplicationHelper;

namespace PoC.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            FulcrumApplicationHelper.WebBasicSetup("asynchronous-processes", new Tenant("ignore", "local"), RunTimeLevelEnum.Development);
            services.AddControllers();
            var httpSender = new HttpSender("https://localhost:44308/");

            // Adapter1
            var comCapabilityUsingRest = new CommunicationMgmtRestClient(httpSender);
            var infoCapabilityUsingRest = new CustomerInformationMgmtRestClient(httpSender);
            var onboarding = new CustomerOnboardingMgmtCapability(infoCapabilityUsingRest, comCapabilityUsingRest);
            services.AddSingleton<ICustomerOnboardingMgmt>(onboarding);

            // Adapter2
            services.AddSingleton<ICustomerInformationMgmtCapability, CustomerInformationMgmtCapability>();

            // Adapter3
            services.AddSingleton<ICommunicationMgmtCapability, CommunicationMgmtCapability>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
