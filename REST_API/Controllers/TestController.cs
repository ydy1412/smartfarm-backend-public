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
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {

        public ApiTestController()
        {
        }

        // GET: api/farmmanager/{farmManagerId}/farms
        [HttpGet("test")]
        public async Task<IActionResult> GetTestApiCall()
        {
            return Ok("Test API call successful");
        }

    }
}