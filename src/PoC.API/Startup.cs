using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoC.API.RestClients.CommunicationMgmtCapability;
using PoC.API.RestClients.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

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
            services.AddControllers();
            services.AddScoped<ICustomerInformationMgmtCapability, CustomerInformationMgmtCapability>();
            services.AddScoped<ICommunicationMgmtCapability, CommunicationMgmtCapability>();
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
