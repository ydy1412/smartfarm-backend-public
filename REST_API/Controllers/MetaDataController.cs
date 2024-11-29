using Microsoft.AspNetCore.Mvc;
using REST_API.Services;
using REST_API.Models;
using REST_API.DTOs;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetaDataController : ControllerBase
    {
        private readonly MetaDataService _metaDataService;

        public MetaDataController(MetaDataService metaDataService)
        {
            _metaDataService = metaDataService;
        }

        // 특정 계층(Level)의 메타 데이터를 가져오는 API
        [HttpGet("level/{level}")]
        public async Task<IActionResult> GetMetaDataByLevel(int level)
        {
            try
            {
                var metaData = await _metaDataService.GetMetaDataByLevelAsync(level);
                if (metaData == null || !metaData.Any())
                {
                    return NotFound(new { message = $"No meta data found for level {level}." });
                }
                return Ok(metaData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 특정 상위 메타 데이터에 속한 하위 메타 데이터를 가져오는 API
        [HttpGet("parent/{parentMetaDataId}")]
        public async Task<IActionResult> GetChildMetaData(int parentMetaDataId)
        {
            try
            {
                var metaData = await _metaDataService.GetChildMetaDataAsync(parentMetaDataId);
                if (metaData == null || !metaData.Any())
                {
                    return NotFound(new { message = $"No child meta data found for parent ID {parentMetaDataId}." });
                }
                return Ok(metaData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 특정 TypeCode에 해당하는 메타 데이터를 가져오는 API
        [HttpGet("type/{typeCode}")]
        public async Task<IActionResult> GetMetaDataByTypeCode(string typeCode)
        {
            try
            {
                var metaData = await _metaDataService.GetMetaDataByTypeCodeAsync(typeCode);
                if (metaData == null || !metaData.Any())
                {
                    return NotFound(new { message = $"No meta data found for type {typeCode}." });
                }
                return Ok(metaData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 특정 MetaData ID로 메타 데이터를 가져오는 API
        [HttpGet("{metaDataId}")]
        public async Task<IActionResult> GetMetaDataById(int metaDataId)
        {
            try
            {
                var metaData = await _metaDataService.GetMetaDataByIdAsync(metaDataId);
                if (metaData == null)
                {
                    return NotFound(new { message = $"Meta data with ID {metaDataId} not found." });
                }
                return Ok(metaData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
