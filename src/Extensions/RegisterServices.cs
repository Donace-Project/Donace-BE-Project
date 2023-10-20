using System.Diagnostics;
using System.Text;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.EntityFramework;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repositories;
using Donace_BE_Project.EntityFramework.Repository;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Middlewares;
using Donace_BE_Project.Services;
using Donace_BE_Project.Services.Event;
using Donace_BE_Project.Settings;
using EntityFramework.Repository;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace Donace_BE_Project.Extensions
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            services.AddDbContext<CalendarDbContext>();
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddRedis(configuration);
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(configuration.GetConnectionString("hangfire"));
            });
            services.AddHangfireServer();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();
            services.AddSingleton<IUserProvider, UserProvider>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ICalendarRepository, CalendarRepository>();
            services.AddScoped<ICalendarParticipationRepository, CalendarParticipationRepository>();

            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<AppUserManager, AppUserManager>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthenticationAppService, AuthenticationAppService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<ICalendarService, CalendarService>();
            services.AddTransient<ICalendarParticipationService, CalendarParticipationService>();
            services.AddTransient<ICacheService, CacheService>();

            return services;
        }

        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = configuration.GetSection("Redis").GetValue<string>("Host");
            var redisInstanceName = configuration.GetSection("Redis").GetValue<string>("Port");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfiguration;
                options.InstanceName = redisInstanceName;
            });

            return services;
        }
        public static IServiceCollection RegisterSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSetting>(configuration.GetSection(nameof(JwtSetting)));

            return services;
        }


        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Event_API",
                    Version = "v1",
                });

                c.ResolveConflictingActions(apiDescription => apiDescription.First());

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer token.",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(setup =>
            {
                setup.Password.RequireDigit = false;
                setup.Password.RequireLowercase = false;
                setup.Password.RequireUppercase = false;
                setup.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<CustomerDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureCustomerSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CustomerDbContext>(option =>
               option.UseSqlServer(configuration.GetConnectionString("Customer")));
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSetting = configuration.GetSection("JwtSetting");

            var secret = jwtSetting["Secret"];
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSetting["Issuer"],
                    ValidAudience = jwtSetting["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                };
            });
        }
    }
}
