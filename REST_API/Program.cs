using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.EntityFrameworkCore;

using REST_API.Db;

namespace REST_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // 자동으로 데이터베이스 마이그레이션 적용
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Database.Migrate();  // 자동으로 마이그레이션을 적용합니다.

                        // 모든 작업이 성공적으로 완료되면 커밋
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // 문제 발생 시 롤백
                        Console.WriteLine($"Migration failed: {ex.Message}");
                        transaction.Rollback();  // 트랜잭션 롤백
                    }
                } // 자동으로 마이그레이션을 적용합니다.
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // 환경별 설정 파일 로드 추가
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((context, options) =>
                {
                    // appsettings.json에서 Kestrel 설정을 가져와 적용
                    options.Configure(context.Configuration.GetSection("Kestrel"));
                })
                .UseStartup<Startup>();
                });
    }
}