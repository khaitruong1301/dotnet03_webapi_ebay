using System.Net;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;

using EbayProject.Api.Helpers;
using EbayProject.Api.Middleware;
using EbayProject.Api.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//DI:
//Service của blazor server app
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


//Service jwt
//Thêm middleware authentication
var privateKey = builder.Configuration["jwt:Serect-Key"];
var Issuer = builder.Configuration["jwt:Issuer"];
var Audience = builder.Configuration["jwt:Audience"];
// Thêm dịch vụ Authentication vào ứng dụng, sử dụng JWT Bearer làm phương thức xác thực
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Thiết lập các tham số xác thực token
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        // Kiểm tra và xác nhận Issuer (nguồn phát hành token)
        ValidateIssuer = true,
        ValidIssuer = Issuer, // Biến `Issuer` chứa giá trị của Issuer hợp lệ
                              // Kiểm tra và xác nhận Audience (đối tượng nhận token)
        ValidateAudience = true,
        ValidAudience = Audience, // Biến `Audience` chứa giá trị của Audience hợp lệ
                                  // Kiểm tra và xác nhận khóa bí mật được sử dụng để ký token
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)),
        // Sử dụng khóa bí mật (`privateKey`) để tạo SymmetricSecurityKey nhằm xác thực chữ ký của token
        // Giảm độ trễ (skew time) của token xuống 0, đảm bảo token hết hạn chính xác
        ClockSkew = TimeSpan.Zero,
        // Xác định claim chứa vai trò của user (để phân quyền)
        RoleClaimType = ClaimTypes.Role,
        // Xác định claim chứa tên của user
        NameClaimType = ClaimTypes.Name,
        // Kiểm tra thời gian hết hạn của token, không cho phép sử dụng token hết hạn
        ValidateLifetime = true
    };
});



// Thêm dịch vụ Authorization để hỗ trợ phân quyền người dùng
builder.Services.AddAuthorization();

//add jwt service (tự viết)
builder.Services.AddScoped<JwtAuthService>();



//service controller
builder.Services.AddControllers();
//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // 🔥 Thêm hỗ trợ Authorization header tất cả api
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token vào ô bên dưới theo định dạng: Bearer {token}"
    });

    // 🔥 Định nghĩa yêu cầu sử dụng Authorization trên từng api
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
//orm EntityFramework

string? connectionEbay = builder.Configuration.GetConnectionString("connectionStringEbay");

builder.Services.AddDbContext<EbayContext>(options =>
    options.UseSqlServer(connectionEbay));
//DI middleware
builder.Services.AddScoped<BlockIpMiddleware>();

//DI service CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("allowLocalhost", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .WithMethods("GET", "POST") //.AllowAnyMethod
            .AllowCredentials();

    });
});
var app = builder.Build();

//Cấu hình middleware error 
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        // Đặt response content-type thành JSON
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        // Trả về JSON chứa thông tin lỗi
        var errorResponse = new { message = exceptionFeature?.Error.Message ?? "Lỗi không xác định!" };
        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});
app.UseCors("allowLocalhost");
app.UseSwagger();
app.UseSwaggerUI();

//middleware
app.UseMiddleware<BlockIpMiddleware>();

app.MapControllers(); // api/ ....
app.UseHttpsRedirection();
app.UseAuthentication(); // yêu cầu verify token
app.UseAuthorization(); // yêu cầu verify roles của token

//Cấu hình các tệp tỉnh 
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(builder.Environment.ContentRootPath, "assets/img")),
//     RequestPath = "/images"

// });
app.UseStaticFiles(); // wwwroot

// app.UseRouting();
// app.UseEndpoints(endpoint => );

//Sử dụng middleware của blazor map file host để làm file chạy đầu tiên
app.MapBlazorHub(); 
app.MapFallbackToPage("/_Host");


app.Run();

