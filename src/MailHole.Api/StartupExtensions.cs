using System;
using System.IO;
using System.Security.Claims;
using Hangfire;
using Hangfire.Redis;
using MailHole.Api.Auth;
using MailHole.Common.Auth;
using MailHole.Common.Extensions;
using MailHole.Common.Model.Options;
using MailHole.Db;
using MailHole.Db.Entities.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace MailHole.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddJobScheduler(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            return serviceCollection.AddHangfire(config =>
            {
                var hangfireOptions = configuration.BindHangfireOptions();
                config.UseRedisStorage(hangfireOptions.RedisConnectionString, new RedisStorageOptions
                {
                    Db = hangfireOptions.RedisDatabaseIndex,
                    Prefix = hangfireOptions.RedisPrefix
                });
            });
        }

        public static IServiceCollection AddApiDoc(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new Info {Title = "MailHole API"});
                var xmlDocPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{typeof(Startup).Assembly.GetName().Name}.xml");
                swagger.IncludeXmlComments(xmlDocPath);
            });
        }

        public static IServiceCollection AddTokenAuth(this IServiceCollection services, AuthOptions authOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = authOptions.SecurityKey,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidIssuer = authOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = authOptions.Audience,
                        ClockSkew = TimeSpan.Zero
                        
                    };
                    options.Audience = authOptions.Audience;
                    options.SaveToken = true;
                    options.ClaimsIssuer = authOptions.Issuer;
                    options.Validate();
                });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddScoped<IUserValidator<MailHoleUser>, UserValidator<MailHoleUser>>();
            services.TryAddScoped<IPasswordValidator<MailHoleUser>, PasswordValidator<MailHoleUser>>();
            services.TryAddScoped<IPasswordHasher<MailHoleUser>, PasswordHasher<MailHoleUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.TryAddScoped<IRoleValidator<MailHoleRole>, RoleValidator<MailHoleRole>>();
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<MailHoleUser>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<MailHoleUser>, UserClaimsPrincipalFactory<MailHoleUser, MailHoleRole>>();
            services.TryAddScoped<UserManager<MailHoleUser>, AspNetUserManager<MailHoleUser>>();
            services.TryAddScoped<SignInManager<MailHoleUser>, SignInManager<MailHoleUser>>();
            services.TryAddScoped<RoleManager<MailHoleRole>, AspNetRoleManager<MailHoleRole>>();
            services.TryAddScoped(provider => new AuthSeeder(provider.GetService<UserManager<MailHoleUser>>(), provider.GetService<RoleManager<MailHoleRole>>()));
            services.TryAddScoped(_ => new SigningCredentials(authOptions.SecurityKey, SecurityAlgorithms.EcdsaSha512));
            services.TryAddSingleton(authOptions);

            // Enable the use of an [Authorize("Bearer")] attribute on methods and classes to protect.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());
                options.AddPolicy(PolicyNames.UserPolicy, policy => policy.RequireClaim(ClaimTypes.Role, MailHoleRoles.User));
                options.AddPolicy(PolicyNames.AdminPolicy, policy => policy.RequireClaim(ClaimTypes.Role, MailHoleRoles.Admin));
            });


            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                
                options.User.RequireUniqueEmail = true;
                
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
            });

            new IdentityBuilder(typeof(MailHoleUser), typeof(MailHoleRole), services)
                .AddEntityFrameworkStores<MailHoleDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}