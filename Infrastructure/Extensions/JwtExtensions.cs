using Core.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Extensions
{
    internal static class JwtExtensions
    {

        internal static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(config.GetSection($"SecuritySettings:{nameof(JwtSettings)}"));
            var jwtSettings = config.GetSection($"SecuritySettings:{nameof(JwtSettings)}").Get<JwtSettings>();
            if (string.IsNullOrEmpty(jwtSettings.Key))
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            byte[] key = Encoding.ASCII.GetBytes(jwtSettings.Key);

            // return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddJwtBearer(options =>
            //     {
            //         options.TokenValidationParameters = new TokenValidationParameters()
            //         {
            //             ValidateIssuerSigningKey = true,
            //             ValidateIssuer = false,
            //             ValidateAudience = false,
            //             ValidAudience = jwtSettings.ValidAudience,
            //             ValidIssuer = jwtSettings.ValidIssuer,
            //             ValidateLifetime = true,
            //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            //         };
            //
            //         options.Events = new JwtBearerEvents()
            //         {
            //             OnMessageReceived = context =>
            //             {
            //                 return Task.CompletedTask;
            //             },
            //             OnAuthenticationFailed = context =>
            //             {
            //                 context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //                 context.Response.ContentType = context.Request.Headers["Accept"].ToString();
            //                 return Task.CompletedTask;
            //                 //string _message = "Authetication token is invalid.";
            //             }
            //         };
            //     }).Services;

            return services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.ValidAudience,
                        ValidIssuer = jwtSettings.ValidIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/chathub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                .Services;
        }


    }
}