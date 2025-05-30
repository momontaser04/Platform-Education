
using EDU_Platform.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Contracts.Authentication;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Filters;
using PlatformEduPro.DTO.Users;
using PlatformEduPro.Extensions;
using PlatformEduPro.Middleware;
using PlatformEduPro.Models;
using PlatformEduPro.Services;
using PlatformEduPro.Services.Interfaces;
using System.Threading.RateLimiting;

namespace PlatformEduPro
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();



            //for Email Sender to get origin
            builder.Services.AddHttpContextAccessor();

            #region RateLimiting

            builder.Services.AddRateLimiter(ratelimiteroptions =>
            {

                ratelimiteroptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                ratelimiteroptions.AddPolicy(RateLimiters.IpLimiter, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromSeconds(20)
                        }
                )
                );
                ratelimiteroptions.AddPolicy(RateLimiters.UserLimiter, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.GetUserId(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromSeconds(20)
                        }
                    )
                );
                ratelimiteroptions.AddConcurrencyLimiter(RateLimiters.Concurrency, options =>
                {
                    options.PermitLimit = 1000;
                    options.QueueLimit = 100;
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;

                });
            });
            #endregion
            // Add Cors

            builder.Services.AddCors(option =>
            option.AddPolicy("MyPolicy", builder =>

            builder.AllowAnyOrigin()
            .AllowAnyHeader().AllowAnyMethod()

            )

            );

            //Add Distribuated Caching
            builder.Services.AddDistributedMemoryCache();

            #region Option Pattern
            // Configure JWT options
            builder.Services.AddOptions<JwtOptions>()
                .BindConfiguration("JWT")
                .ValidateDataAnnotations()
                .ValidateOnStart();
            //configure Email Setting
            builder.Services.AddOptions<EmailSetting>()
                           .BindConfiguration("EmailSetting")
                           .ValidateDataAnnotations()
                           .ValidateOnStart();
            #endregion

            // Add API documentation & Swagger authentication
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenJwtAuth();
            builder.Services.AddCustomJwtAuth(builder.Configuration);







            #region Dependency Injection (Services & Repositories)
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IEmailSender, EmailService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ISctionService, SectionService>();
            builder.Services.AddScoped<IVideoService, VideoService>();
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<IRoleService, RoleService>();




            builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();


            #endregion

            #region Health Checks
            builder.Services.AddHealthChecks().AddDbContextCheck<EduPlatformDbContext>()

                   .AddHangfire(options => { options.MinimumAvailableServers = 1; });


            #endregion

            builder.Services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<EduPlatformDbContext>().AddDefaultTokenProviders();


            #region Identity_Options


            builder.Services.Configure<IdentityOptions>(options =>
            {

                options.Password.RequiredLength = 7;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

            });
            #endregion



            builder.Services.AddDbContextPool<EduPlatformDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            #region Hangfire

            builder.Services.AddHangfire(configuration => configuration
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"))
            );
            builder.Services.AddHangfireServer();
            #endregion

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ProfileLoggMiddleware>();

            #region Hangfire Options
            app.UseHttpsRedirection();
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = [

                    new HangfireCustomBasicAuthenticationFilter{


                        User = app.Configuration.GetValue<string>("HangfireSetting:UserName"),
                        Pass = app.Configuration.GetValue<string>("HangfireSetting:Password")
                    }



                    ],
                DashboardTitle = "Mohamed Montaer Dashboard",


            }
                );


            #endregion

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            #region RecurringJob
            RecurringJob.AddOrUpdate<INotificationService>(
                   nameof(INotificationService.SendNewCourseNotification),
                   x => x.SendNewCourseNotification(),
                   Cron.Daily);
            #endregion



            app.UseCors("MyPolicy");


            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.Run();
        }
    }
}
