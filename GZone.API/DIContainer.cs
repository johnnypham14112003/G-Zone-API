using Asp.Versioning;
using GZone.Repository;
using GZone.Repository.Base;
using GZone.Repository.Interfaces;
using GZone.Repository.Models;
using GZone.Repository.Repositories;
using GZone.Service.BusinessModels.Response.Customization;
using GZone.Service.BusinessModels.StrongTypedModels;
using GZone.Service.Extensions;
using GZone.Service.Interfaces;
using DLL.Interfaces;
using DLL.Services;
using GZone.Service.Services;
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.StaticFiles;
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
            services.ConfigFileProvider();
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
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing. Set DB_CONNECTION_STRING or ConnectionStrings:DefaultConnection.");
            }

            services.AddDbContext<GZoneDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }


        private static IServiceCollection InjectBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserAddressService, UserAddressService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICustomizationService, CustomizationService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWarrantyClaimService, WarrantyClaimService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IUserVoucherService, UserVoucherService>();
            services.AddScoped<IOrderVoucherService, OrderVoucherService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();

            //Add other BusinessServices here...

            return services;
        }

        private static IServiceCollection InjectRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //---------------------------------------------------------------------------
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserAddressRepository, UserAddressRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWarrantyClaimRepository, WarrantyClaimRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IUserVoucherRepository, UserVoucherRepository>();
            services.AddScoped<IOrderVoucherRepository, OrderVoucherRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<ICustomizationRepository, CustomizationRepository>();

            //Add other repository here...

            return services;
        }

        private static IServiceCollection ConfigFileProvider(this IServiceCollection services)
        {
            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

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
                        "https://localhost:5173",
                        "http://localhost:3000",
                        "https://localhost:3000",
                        "http://localhost:3001",
                        "https://localhost:3001",
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

        private static IServiceCollection ConfigMapster(this IServiceCollection services)
        {
            TypeAdapterConfig<Customization, CustomizationResponse>
            .NewConfig()
            .Map(dest => dest.CustomerName, src => src.Customer.FullName)
            .Map(dest => dest.StaffName, src => src.Staff != null ? src.Staff.FullName : null)
            .Map(dest => dest.ProductName, src => src.Product.ProductName);

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
                // 1. Tr? v? các version du?c h? tr? trong response header (api-supported-versions)
                options.ReportApiVersions = true;

                // 2. N?u client không g?i version, m?c d?nh s? dùng version này
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // 3. Ð?c version t? dâu? (M?c d?nh là Query String ?api-version=1.0)
                // C?u hình bên du?i cho phép d?c t? c? Query String VÀ Header
                //options.ApiVersionReader = ApiVersionReader.Combine(
                //    new QueryStringApiVersionReader("api-version"),
                //    new HeaderApiVersionReader("X-Version")
                //);
            })
            .AddApiExplorer(options =>
            {
                // Ð?nh d?ng tên version cho Group (ví d?: 'v'1, 'v'2)
                options.GroupNameFormat = "'v'VVV";

                // QUAN TR?NG NH?T: Thay th? {version} trong URL b?ng giá tr? th?c t?
                // Ví d?: api/v{version}/accounts -> api/v1/accounts
                // Vi?c này giúp Swagger phân bi?t du?c 2 du?ng d?n khác nhau -> H?t l?i Conflict
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

                // T?o doc cho V2
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
                // Priority: environment variables > appsettings > defaults.
                Key = Environment.GetEnvironmentVariable("Jwt_Key")
                    ?? configuration["Jwt:Key"]
                    ?? string.Empty,
                Issuer = Environment.GetEnvironmentVariable("Jwt_Issuer")
                    ?? configuration["Jwt:Issuer"]
                    ?? string.Empty,
                Audience = Environment.GetEnvironmentVariable("Jwt_Audience")
                    ?? configuration["Jwt:Audience"]
                    ?? string.Empty,
                AccessTokenExpirationMinutes = int.TryParse(Environment.GetEnvironmentVariable("Jwt_AccessTokenExpirationMinutes"), out var m)
                    ? m
                    : int.TryParse(configuration["Jwt:AccessTokenExpirationMinutes"], out var cm) ? cm : 15,
                RefreshTokenExpirationDays = int.TryParse(Environment.GetEnvironmentVariable("Jwt_RefreshTokenExpirationDays"), out var d)
                    ? d
                    : int.TryParse(configuration["Jwt:RefreshTokenExpirationDays"], out var cd) ? cd : 7
            };

            if (string.IsNullOrWhiteSpace(jwtOps.Key) || string.IsNullOrWhiteSpace(jwtOps.Issuer) || string.IsNullOrWhiteSpace(jwtOps.Audience))
            {
                throw new InvalidOperationException("JWT settings are missing. Set Jwt_Key/Jwt_Issuer/Jwt_Audience or Jwt section in appsettings.");
            }

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


