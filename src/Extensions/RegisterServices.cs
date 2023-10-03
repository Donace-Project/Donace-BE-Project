using System.Diagnostics;

using Donace_BE_Project.EntityFramework;
using Donace_BE_Project.EntityFramework.Repository;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Middlewares;
using Donace_BE_Project.Services;
using Donace_BE_Project.Services.Event;
using EntityFramework.Repository;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using AuthenticationService = Donace_BE_Project.Services.AuthenticationService;
using IAuthenticationService = Donace_BE_Project.Interfaces.Services.IAuthenticationService;

namespace Donace_BE_Project.Extensions
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {

            //services.AddSingleton(FirebaseApp.Create());

            services.AddDbContext<AppDbContext>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();

            services.AddScoped<IEmailSender, EmailSender>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IEventService, EventService>();

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Define security requirements for Swagger (optional)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] { }
                }
                        });
            });

            return services;
        }

        public static IServiceCollection ConfigureFirebase(this IServiceCollection services)
        {
            services.AddSingleton(FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("donace-firebase.json")
            }));

            services.AddFirebaseAuthentication();

            return services;
        }

        public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (o) => { });

            services.AddScoped<FirebaseAuthenticationFunctionHandler>();

            return services;
        }
    }
}
