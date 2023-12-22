using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Utility.MiddleWare
{
    public class RedirectToDashboardMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectToDashboardMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated && 
                context.Request.Path.StartsWithSegments("/Identity/Account/Login"))
            {
                context.Response.Redirect("/");
                return;
            }

            await _next(context);
        }
    }

}