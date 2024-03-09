using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain;
using OnlineAuctionWeb.Infrastructure.Exceptions;
using System.Data;
using System.Security.Claims;

namespace OnlineAuctionWeb.Infrastructure.Authorize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public int[] RequiredRoles { get; set; }

        public CustomAuthorizeAttribute(params int[] roles)
        {
            RequiredRoles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                throw new CustomException(StatusCodes.Status403Forbidden, "You do not have access to this resource!");
            }

            if (!RequiredRoles.Contains(int.Parse(userRole)))
            {
                throw new CustomException(StatusCodes.Status403Forbidden, "You do not have access to this resource!");
            }
        }
    }

}
