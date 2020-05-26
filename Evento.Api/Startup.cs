
using Evento.Core.Repositories;
using Evento.Infrastructure.Mappers;
using Evento.Infrastructure.Repositories;
using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Evento.Infrastructure.Settings;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IHostApplicationLifetime;

namespace Evento
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IContainer Conteiner { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });
            services.AddMemoryCache();
            services.AddAuthorization(x => x.AddPolicy("HasAdminRole", p=> p.RequireRole("admin")));
            // services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddSingleton<IJwtHandler, JwtHandler>();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddLogging(a =>
            {
                a.AddDebug();
                a.AddConfiguration(Configuration.GetSection("Logging"));
                a.AddNLog();
            });

            // Pobranie appsetingsow (appsetings.json)
            var appSettingsSection = Configuration.GetSection(typeof(AppSettings).Name);

            // Zmapowanie appsetingsów na klase AppSettings
            var appSettings = appSettingsSection.Get<AppSettings>();

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = appSettings.Issuer
                };
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<EventRepository>().As<IEventRepository>().InstancePerLifetimeScope();
            Conteiner = builder.Build();

            return new AutofacServiceProvider(Conteiner);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            applicationLifetime.ApplicationStopped.Register(() => Conteiner.Dispose());
        }
    }
}
