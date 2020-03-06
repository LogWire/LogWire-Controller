using System;
using System.Threading.Tasks;
using LogWire.Controller.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LogWire.Controller.Middleware
{
    public class ApiTokenAuthorizationMiddleware
    {

        private readonly RequestDelegate _next;

        public ApiTokenAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            string authHeader = context.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader))
            {
                //TODO
                //extract credentials from authHeader and do some sort or validation
                bool isHeaderValid = ValidateCredentials(authHeader);
                if (isHeaderValid)
                {
                    await _next.Invoke(context);
                    return;
                }

            }

            //Reject request if there is no authorization header or if it is not valid
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");

        }

        private bool ValidateCredentials(string authHeader)
        {
            return authHeader.Equals(ApiToken.Instance.Token);
        }
    }

    public static class APITokenAuthorizationMiddlewareExtension
    {
        public static IApplicationBuilder UseApiTokenAuthorization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ApiTokenAuthorizationMiddleware>();
        }
    }

}
