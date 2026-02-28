using Asp.Versioning;
using GZone.Repository;
using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Repositories;
using GZone.Service.BusinessModels.StrongTypedModels;
using GZone.Service.Extensions;
using GZone.Service.Interfaces;
using GZone.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace GZone.API
{
    public static class DIContainer
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //System Services
            services.InjectDbContext(configuration);
            services.InjectBusinessServices();
            services.InjectRepository();

            services.AddJwtAuthentication(configuration);
            services.ConfigCORS();
            services.ConfigKebabCase();
            services.ConfigJsonLoopDeserielize();
            services.ConfigVersioning();
            services.ConfigSwagger();

            //Third Party Services
            //...

            return services;
        }

        //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        private static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services.AddDbContext<GZoneDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }


        private static IServiceCollection InjectBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWarrantyClaimService, WarrantyClaimService>();
            services.AddScoped<INotificationService, NotificationService>();

            //Add other BusinessServices here...

            return services;
        }

        private static IServiceCollection InjectRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //---------------------------------------------------------------------------
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWarrantyClaimRepository, WarrantyClaimRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            //Add other repository here...

            return services;
        }

        private static IServiceCollection ConfigCORS(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:3000",
                        "https://fe.vercel.app",
                        "https://asp-deep-badly.ngrok-free.app"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            return services;
        }

        private static IServiceCollection ConfigKebabCase(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new KebabRouteTransform()));
            }).AddNewtonsoftJson(options =>
            {//If using NewtonSoft in project then must orride default Naming rule of System.text
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new KebabCaseNamingStrategy()
                };
            });

            services.AddSwaggerGen(c => { c.SchemaFilter<KebabSwaggerSchema>(); });
            return services;
        }

        private static IServiceCollection ConfigJsonLoopDeserielize(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            return services;
        }

        private static IServiceCollection ConfigVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // 1. Trả về các version được hỗ trợ trong response header (api-supported-versions)
                options.ReportApiVersions = true;

                // 2. Nếu client không gửi version, mặc định sẽ dùng version này
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // 3. Đọc version từ đâu? (Mặc định là Query String ?api-version=1.0)
                // Cấu hình bên dưới cho phép đọc từ cả Query String VÀ Header
                //options.ApiVersionReader = ApiVersionReader.Combine(
                //    new QueryStringApiVersionReader("api-version"),
                //    new HeaderApiVersionReader("X-Version")
                //);
            })
            .AddApiExplorer(options =>
            {
                // Định dạng tên version cho Group (ví dụ: 'v'1, 'v'2)
                options.GroupNameFormat = "'v'VVV";

                // QUAN TRỌNG NHẤT: Thay thế {version} trong URL bằng giá trị thực tế
                // Ví dụ: api/v{version}/accounts -> api/v1/accounts
                // Việc này giúp Swagger phân biệt được 2 đường dẫn khác nhau -> Hết lỗi Conflict
                options.SubstituteApiVersionInUrl = true;
            });
            return services;
        }

        public static IServiceCollection ConfigSwagger(this IServiceCollection services)
        {
            // Swagger Bearer auth
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GZone API V1",
                    Version = "v1"
                });

                // Tạo doc cho V2
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "GZone API V2",
                    Version = "v2"
                });

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    return apiDesc.GroupName == docName;
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Input JWT directly into the Value box. Example: \"{token}\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOps = new JwtSettings
            {
                // Priority: appsettings.json > Environment Variables
                Key = Environment.GetEnvironmentVariable("Jwt_Key") ?? string.Empty,
                Issuer = Environment.GetEnvironmentVariable("Jwt_Issuer") ?? string.Empty,
                Audience = Environment.GetEnvironmentVariable("Jwt_Audience") ?? string.Empty,
                AccessTokenExpirationMinutes = int.TryParse(Environment.GetEnvironmentVariable("Jwt_AccessTokenExpirationMinutes"), out var m) ? m : 15,
                RefreshTokenExpirationDays = int.TryParse(Environment.GetEnvironmentVariable("Jwt_RefreshTokenExpirationDays"), out var d) ? d : 7
            };

            // Register JwtSettings as a singleton
            services.AddSingleton(jwtOps);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOps.Issuer,
                    ValidAudience = jwtOps.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOps.Key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
    }
}
