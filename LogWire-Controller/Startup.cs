using LogWire.Controller.Data.Context;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Model.Application;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Data.Repository.Application;
using LogWire.Controller.Middleware;
using LogWire.Controller.Services.API;
using LogWire.Controller.Services.API.Application;
using LogWire.Controller.Services.Hosted;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LogWire.Controller
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt => opt.UseMySql("server=localhost;port=3306;database=lw_controller;uid=lwuser;password=lwpassword"));
            services.AddDbContext<ApplicationDataContext>(opt => opt.UseMySql("server=localhost;port=3306;database=lw_application;uid=lwuser;password=lwpassword"));
            

            services.AddScoped<IDataRepository<ConfigurationEntry>, ConfigurationRepository>();
            services.AddScoped<IDataRepository<UserEntry>, UserRepository>();

            services.AddScoped<IDataRepository<ApplicationEntry>, ApplicationRepository>();

            services.AddSingleton<ManagementService>();
            services.AddSingleton<IHostedService, ManagementService>(serviceProvider => serviceProvider.GetService<ManagementService>());

            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseApiTokenAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ConfigurationServiceServer>();
                endpoints.MapGrpcService<StatusServiceServer>();
                endpoints.MapGrpcService<AuthenticationServiceServer>();
                endpoints.MapGrpcService<ApplicationServiceServer>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
