using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EbayProject.Api.Helpers;
using EbayProject.Api.models;
using EbayProject.Api.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
//using EbayProject.Api.Models;

namespace EbayProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationDemoController(EbayContext _context,JwtAuthService _jwt) : ControllerBase
    {

        [Authorize]
        [HttpGet("getAllUser")]
        public async Task<ActionResult> getAllUser()
        {
            return Ok(await _context.Users.Skip(0).Take(10).ToListAsync());
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterVM model)
        {
            //Check email và user name có tồn tại hay chưa
            User? userDB = _context.Users.SingleOrDefault(u => u.Username == model.UserName || u.Email == model.Email);
            if (userDB != null)
            {
                return BadRequest("Tài khoản đã tồn tại!");
            }
            // try
            // {
            //     _context.Database.BeginTransaction();
            //     //Khối lệnh sql raw
            //     string sql = "INSERT INTO Users (Username, PasswordHash, email) VALUES (@param1, @param2,@param3)";
            //     SqlParameter[] parameters = {
            //         new SqlParameter("@param1", model.UserName),
            //         new SqlParameter("@param2",  PasswordHelper.HashPassword(model.Password)),
            //         new SqlParameter("@param3",  model.Email)
            //     };
            //     int affectedRows = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

            //     User us = _context.Users.Single(u => u.Username == model.UserName);

            //     string sqlUserRole = "INSERT INTO UserRole (UserId, RoleId) VALUES (@param1, @param2)";
            //     SqlParameter[] parameters1 = {
            //         new SqlParameter("@param1", us.Id),
            //         new SqlParameter("@param2",  9)
            //     };
            //     int affectedRows1 = await _context.Database.ExecuteSqlRawAsync(sql, parameters1);
            //     _context.Database.CommitTransaction();
            // }
            // catch (Exception err)
            // {
            //     _context.Database.RollbackTransaction();
            // }

            //Thêm user vào db (Table user)
            User newUser = new User();
            newUser.Username = model.UserName;
            newUser.PasswordHash = PasswordHelper.HashPassword(model.Password);
            newUser.FullName = model.FullName;
            newUser.Email = model.Email;
            newUser.Deleted = false;
            newUser.CreatedAt = DateTime.Now;
            //Thêm user và role vào bảng userRole (để phân quyền)
            UserRole usRole = new UserRole();
            usRole.UserId = newUser.Id;
            usRole.RoleId = RoleId.Buyer;
            newUser.UserRoles.Add(usRole); // Add tham chiếu liên table 
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Ok("Đăng ký thành công!");
        }



        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginVM model)
        {
            //B1: Verify password
            User? us = _context.Users.SingleOrDefault(u => u.Email == model.userNameOrEmail || u.Username == model.userNameOrEmail);
            if (us == null)
            {
                return BadRequest("Tài khoản hoặc email không tồn tại!");
            }
            if (!PasswordHelper.VerifyPassword(model.password, us.PasswordHash))
            {
                //Đăng nhập thật bại
                return BadRequest("Sai password!");
            }
            //Đúng thì tạo token từ user và role 
            string token = _jwt.GenerateToken(us);

            //B2: Tạo token từ thông tin db
            var res = new
            {
                token = token
            };
            return Ok(res );
        }
        


    }
}