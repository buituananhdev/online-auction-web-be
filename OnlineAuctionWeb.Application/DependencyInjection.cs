using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnlineAuctionWeb.Application.Services;
using System.Text;

namespace OnlineAuctionWeb.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuctionService, AuctionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBidService, BidService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IWatchListService, WatchListService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IHubService, HubService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddHttpContextAccessor();

            var key = Encoding.ASCII.GetBytes(configration.GetSection("JwtSettings:Secret").Value!);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
    }
}
