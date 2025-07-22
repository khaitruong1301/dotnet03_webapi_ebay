

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class DemoFilter : ActionFilterAttribute
{
    public string name { get; set; }
    public override void OnActionExecuting(ActionExecutingContext context)//httpContext 
    {
        Console.WriteLine($@"OnActionExecuting {name}");
        //Trước khi xử lý action 
        
        
    }
    public override  void OnActionExecuted(ActionExecutedContext context)//httpContext 
    {
        //Sau khi xử lý sau khi action handler có kết quả
        Console.WriteLine($@"OnActionExecuted: ");
        context.Result = new ContentResult()
        {
            StatusCode = 200,
            Content = "Không thể trả về kết quả !"
        };
        
    }


}

