using System.Net;
using Outputs;
using Utils;

namespace Middlewares;

public class UnauthorizedResponseMiddleware
{
    private readonly RequestDelegate Next;
    public UnauthorizedResponseMiddleware(RequestDelegate next) => Next = next;
    public async Task Invoke(HttpContext context)
    {
        await Next(context);
        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(new ResponseBase("401", false, error: new ErrorOutput 
            { 
                Message = "Não autorizado.",
                Code = "ER001"
            }).ToString());
        }
    }
}