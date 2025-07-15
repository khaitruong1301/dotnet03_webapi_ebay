using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EbayProject.Api.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using EbayProject.Api.Models;

namespace EbayProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(EbayContext _context) : ControllerBase
    {

        //[ filter attribute]
        [HttpGet("getallproduct")] //routing: api/getallproduct
        public async Task<ActionResult> GetAll() //model binding: binding các trị from header, ....
        {

            //action handller
            //response
            return Ok(await _context.Products.Skip(0).Take(10).ToListAsync());
        }


        [HttpPost("getDemoById/{id}")]
        public async Task GetDemo([FromRoute] string id, [FromHeader] string token, [FromBody] DemoModel models)
        {
            //Dùng context để lấy thông Request ....
            HttpContext.Request.ContentType = "application/json";
            string? idRoute = HttpContext.Request.RouteValues["id"] as string;
            string? tokenHeader = HttpContext.Request.Headers["token"];
            using var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true);

            //Dùng context để setup response 
            await HttpContext.Response.WriteAsJsonAsync("Dữ liệu trả về");

            // try
            // {

            // }
            // catch (Exception err)
            // {
            // HttpContext.Response.StatusCode = 500;

            // await HttpContext.Response.WriteAsJsonAsync(err.Message);

            // }



        }

        [HttpPost("TinhThuong")]
        public async Task<ActionResult> TinhThuong(int a, int b)
        {
            if (b == 0)
            {
                return BadRequest("Dữ liệu không hợp lệ ");
            }

            return Ok(a / b);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> deleteItem()
        {

            return Ok("deleted");
        }

        

    }
}
public class DemoModel
{
    public string id { get; set; }
    public string name { get; set; }
}