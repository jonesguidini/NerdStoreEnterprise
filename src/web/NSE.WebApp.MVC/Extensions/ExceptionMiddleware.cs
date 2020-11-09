using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NSE.WebApp.MVC.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (CustomHttpRequestException ex)
            {
                HandleRequestExceptionAsync(httpContext, ex);
            }
        }

        private static void HandleRequestExceptionAsync(HttpContext context, CustomHttpRequestException httpRequestException)
        {
            // erro ocorre quando a pessoa não esta logada / não tem autorização... redireciona...
            if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
            {

                context.Response.Redirect($"/login?ReturnUrl={context.Request.Path}"); // redireciona para a url de onde foi solicitada
                return;
            }

            context.Response.StatusCode = (int)httpRequestException.StatusCode;
        }
    }
}