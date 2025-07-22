

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class DemoFilterAsync : ActionFilterAttribute
{
    public string name { get; set; }
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        //xử lý sau model binding và trước khi action handler (ứng với action excuting) 
        var httpContext = context.HttpContext;
        string? ip = httpContext.Connection.RemoteIpAddress?.ToString();
        Console.WriteLine($@"ip đã request tới : {ip}");
        var contexResult =  await next();
        //xử lý sau khi action handler có kết quả trả về
        // await httpContext.Response.WriteAsJsonAsync(@$"ip này bị chặn: {ip}");
        contexResult.Result = new ContentResult()
        {
            StatusCode = 401,
            Content = @$"ip này bị chặn: {ip}"
        };
        // httpContext.Response.StatusCode = 401;
        // await httpContext.Response.WriteAsync(@$"ip này bị chặn: {ip}");
     
        
    }



}

