using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using EbayProject.Api.Models;

namespace EbayProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController()
        {
        }

        [HttpGet("GetProfile")]
        public async Task GetProfile()
        {
             HttpContext.Response.StatusCode =  403;
            await HttpContext.Response.WriteAsJsonAsync("Forbidden");

        }

     
    }
}