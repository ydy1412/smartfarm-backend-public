using REST_API.Models;
using REST_API.DTOs;
using REST_API.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace REST_API.Services
{
    public class MetaDataService
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _configuration;

        public MetaDataService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<MetaData>> LoadMetaDataFromJsonAsync(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var json = await reader.ReadToEndAsync();
                var metaDataList = JsonConvert.DeserializeObject<List<MetaData>>(json);
                return metaDataList;
            }
        }

        public async Task<List<MetaDataHierarchy>> LoadMetaDataHierarchyFromJsonAsync(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var json = await reader.ReadToEndAsync();
                var metaDataHierarchyList = JsonConvert.DeserializeObject<List<MetaDataHierarchy>>(json);
                return metaDataHierarchyList;
            }
        }

        public async Task<List<MetaData>> GetMetaDataFromDbAsync()
        {
            return await _context.MetaData.ToListAsync();
        }

        public async Task<List<MetaDataHierarchy>> GetMetaDataHierarchyFromDbAsync()
        {
            return await _context.MetaDataHierarchy.ToListAsync();
        }

        // 메타 데이터를 DB에 저장하는 로직
        public async Task AddMetaDataToDbAsync(MetaData metaData)
        {
            _context.MetaData.Add(metaData);
            await _context.SaveChangesAsync();
        }

        // 메타 데이터 계층 관계를 DB에 추가하는 로직
        public async Task AddMetaDataHierarchyToDbAsync(MetaDataHierarchy hierarchy)
        {
            _context.MetaDataHierarchy.Add(hierarchy);
            await _context.SaveChangesAsync();
        }

        // 메타 데이터를 JSON 파일에서 읽어와 DB와 비교한 후, 추가하는 로직
        public async Task SyncMetaDataWithDbAsync(string metaDataFilePath, string hierarchyFilePath)
        {
            // 1. JSON 파일에서 메타 데이터 및 계층 관계 로드
            var metaDataFromFile = await LoadMetaDataFromJsonAsync(metaDataFilePath);
            var hierarchyFromFile = await LoadMetaDataHierarchyFromJsonAsync(hierarchyFilePath);

            // 2. DB에 저장된 메타 데이터 및 계층 관계 가져오기
            var metaDataFromDb = await GetMetaDataFromDbAsync();
            var hierarchyFromDb = await GetMetaDataHierarchyFromDbAsync();

            // 3. 메타 데이터 동기화
            foreach (var fileMetaData in metaDataFromFile)
            {
                var existsInDb = metaDataFromDb.Any(dbMetaData =>
                    dbMetaData.TypeCode == fileMetaData.TypeCode &&
                    dbMetaData.Value == fileMetaData.Value &&
                    dbMetaData.Level == fileMetaData.Level);

                if (!existsInDb)
                {
                    await AddMetaDataToDbAsync(fileMetaData);
                }
            }

            // 4. 메타 데이터 계층 동기화
            foreach (var fileHierarchy in hierarchyFromFile)
            {
                var existsInDb = hierarchyFromDb.Any(dbHierarchy =>
                    dbHierarchy.ParentId == fileHierarchy.ParentId &&
                    dbHierarchy.ChildId == fileHierarchy.ChildId);

                if (!existsInDb)
                {
                    await AddMetaDataHierarchyToDbAsync(fileHierarchy);
                }
            }
        }

        // 특정 계층(Level)의 메타 데이터를 가져오는 메서드
        public async Task<List<MetaData>> GetMetaDataByLevelAsync(int level)
        {
            return await _context.MetaData
                                 .Where(md => md.Level == level)
                                 .ToListAsync();
        }

        // 특정 상위 메타 데이터의 하위 메타 데이터를 가져오는 메서드
        public async Task<List<MetaData>> GetChildMetaDataAsync(int parentMetaDataId)
        {
            return await _context.MetaData
                                 .Where(md => md.ParentMetaDataId == parentMetaDataId)
                                 .ToListAsync();
        }

        // 특정 TypeCode에 맞는 메타 데이터를 가져오는 메서드
        public async Task<List<MetaData>> GetMetaDataByTypeCodeAsync(string typeCode)
        {
            return await _context.MetaData
                                 .Where(md => md.TypeCode == typeCode)
                                 .ToListAsync();
        }

        // 특정 메타 데이터 ID로 메타 데이터를 가져오는 메서드
        public async Task<MetaData> GetMetaDataByIdAsync(int metaDataId)
        {
            return await _context.MetaData
                                 .FirstOrDefaultAsync(md => md.Id == metaDataId);
        }

        
    }
}