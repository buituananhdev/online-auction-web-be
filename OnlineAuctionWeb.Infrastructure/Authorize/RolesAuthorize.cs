using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain.Enums;
using OnlineAuctionWeb.Application.Exceptions;
using System.Security.Claims;

namespace OnlineAuctionWeb.Infrastructure.Authorize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RolesAuthorize : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public RoleEnum[] RequiredRoles { get; set; }

        public RolesAuthorize(params RoleEnum[] roles)
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

            if (!RequiredRoles.Select(r => r.ToString()).Contains(userRole))
            {
                throw new CustomException(StatusCodes.Status403Forbidden, "You do not have access to this resource!");
            }
        }
    }

}
