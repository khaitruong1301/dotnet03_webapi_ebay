using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using EbayProject.Api.models;

namespace EbayProject.Api.Middleware
{
    public class BlockIpMiddleware : IMiddleware
    {
        private readonly EbayContext _contextDB;


        // ✅ Không cần RequestDelegate, chỉ inject những service cần thiết
        public BlockIpMiddleware(EbayContext context)
        {
            _contextDB = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            // _logger.LogInformation("Middleware BlockIpMiddleware bắt đầu xử lý...");

            string clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            if (clientIp == "Unknown")
            {
                // _logger.LogWarning("Client IP bị chặn do không xác định được.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Unknown IP address");
                return;
            }

            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            ConnectionCountLog? logCount = _contextDB.ConnectionCountLogs
                .SingleOrDefault(n => n.IpAddress == clientIp &&
                            n.ConnectionTime.HasValue &&
                            n.ConnectionTime >= today &&
                            n.ConnectionTime < tomorrow);
            if (logCount == null)
            {
                var newLog = new ConnectionCountLog
                {
                    IpAddress = clientIp,
                    ConnectionTime = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    ConnectionCount = 1
                };
                _contextDB.ConnectionCountLogs.Add(newLog);

            }
            else if (logCount.ConnectionCount > 2000)
            {
                // _logger.LogWarning($"IP {clientIp} bị chặn do kết nối quá nhiều.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Too many connections");
                return;
            }
            else
            {
                logCount.ConnectionCount += 1;
            }



            await _contextDB.SaveChangesAsync();

            // _logger.LogInformation("Middleware hoàn tất xử lý request.");
            await next(context);
        }
    }
}
