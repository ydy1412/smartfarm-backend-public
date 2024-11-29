using Microsoft.EntityFrameworkCore;
using REST_API.Models;


namespace REST_API.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet<T> 속성을 사용하여 각 모델 클래스를 데이터베이스 테이블과 매핑
        public DbSet<User?> Users { get; set; }
        public DbSet<Farm?> Farms { get; set; }
        public DbSet<Facility?> Facility { get; set; }
        public DbSet<FarmManager?> FarmManagers { get; set; }
        public DbSet<FarmUnit?> FarmUnits { get; set; }
        public DbSet<FarmSaleOffer?> FarmSaleOffers { get; set; }
        public DbSet<FarmSaleOrder?> FarmSaleOrders { get; set; }
        public DbSet<MetaData?> MetaData { get; set; }

        public DbSet<MetaDataHierarchy?> MetaDataHierarchy { get; set; }


        // OnModelCreating 메서드를 재정의하여 추가적인 구성 가능
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // try
            // {
            //     modelBuilder.Entity<FarmUnitType>().HasData(
            //     new FarmUnitType { Id = 1, Name = "test", Property = "test", SourceCode = "test",Description="test" }

            // );
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine("farmUnitType insert error");
            // }

            // try
            // {
            //     modelBuilder.Entity<FarmType>().HasData(
            //     new FarmType { Id = 1, Name = "test", Property1 = "test", Property1Description = "test" }

            // );
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine("farmType insert error");
            // }


            // 추가적인 모델 구성 (예: 관계, 제약 조건 등)을 여기에서 정의할 수 있습니다.
        }
    }
}
