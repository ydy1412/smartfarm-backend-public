using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using REST_API.Db;
using REST_API.Models;
using REST_API.Services;

namespace REST_API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );
            });

            services.AddControllers();

            // MySQL 데이터베이스 설정
            // services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
            //     Configuration.GetConnectionString("DefaultConnection"),
            //     new MySqlServerVersion(new Version(8, 0, 23)) // MySQL 버전에 맞게 설정
            // ));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection") // PostgreSQL 연결 문자열
                )
            );

            // JWT 설정
            var secretKey = Configuration["JwtSettings:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            // JWT 인증 서비스 설정
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        // ValidIssuer = Configuration["JwtSettings:Issuer"],
                        // ValidAudience = Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                    };
                });
            var servicesToRegister = new[]
            {
                typeof(AuthService),
                typeof(UserService),
                typeof(FarmManagerService),
                typeof(FarmSaleOfferService),
                typeof(FarmSaleOrderService),
                typeof(FarmService),
                typeof(FarmUnitService),
                typeof(MetaDataService),
            };

            foreach (var serviceType in servicesToRegister)
            {
                services.AddScoped(serviceType);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllOrigins");

            app.UseRouting();

            // 인증 미들웨어 추가
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var metaDataService = scope.ServiceProvider.GetRequiredService<MetaDataService>();

                // 파일 경로 설정
                var metaDataFilePath = Path.Combine(
                    env.ContentRootPath,
                    "Config",
                    "meta_data.json"
                );
                var hierarchyFilePath = Path.Combine(
                    env.ContentRootPath,
                    "Config",
                    "meta_data_hierarchy.json"
                );

                // 비동기 작업 실행
                Task.Run(async () =>
                    {
                        await metaDataService.SyncMetaDataWithDbAsync(
                            metaDataFilePath,
                            hierarchyFilePath
                        );
                    })
                    .Wait();
            }
        }
    }
}
