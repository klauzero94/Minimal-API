using Microsoft.AspNetCore.Authorization;
using Utils;

namespace Routes;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    public CustomAuthorizeAttribute(params UserRoles[] roles) : base() => Roles = String.Join(",", roles);
}