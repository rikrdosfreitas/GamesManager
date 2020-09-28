using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace Games.Api.Filters
{
    public static class HandlerErrorExceptions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseExceptionHandler(app =>
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    //AuthException is your custom exception class
                        if (ex != null && ex.GetType() == typeof(SecurityTokenValidationException))
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        await Task.CompletedTask;
                    }
                });
            });
        }
    }
}