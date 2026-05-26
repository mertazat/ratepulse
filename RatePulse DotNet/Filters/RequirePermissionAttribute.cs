using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RatePulse.Repositories;

namespace RatePulse.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(params string[] permissions)
        : base(typeof(RequirePermissionFilter))
    {
        Arguments = new object[] { permissions };
    }
}

public class RequirePermissionFilter : IAsyncActionFilter
{
    private readonly string[] _permissions;
    private readonly IUserRepository _userRepository;

    public RequirePermissionFilter(string[] permissions, IUserRepository userRepository)
    {
        _permissions = permissions;
        _userRepository = userRepository;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var uid = context.HttpContext.Session.GetString("AdminUid");
        if (string.IsNullOrEmpty(uid))
        {
            context.Result = new RedirectToActionResult("Login", "AdminAuth", null);
            return;
        }

        var user = await _userRepository.GetByUidAsync(uid);
        if (user == null || user.IsBanned)
        {
            context.Result = new RedirectToActionResult("Login", "AdminAuth", null);
            return;
        }

        if (user.Role == "superadmin")
        {
            await next();
            return;
        }

        if (user.Role == "admin" && !_permissions.Contains("manage_admins"))
        {
            await next();
            return;
        }

        bool hasPermission = _permissions.All(p => user.Permissions.Contains(p));
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
