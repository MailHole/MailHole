using System;
using Microsoft.AspNetCore.Authorization;

namespace MailHole.Api.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeUsersAttribute : AuthorizeAttribute
    {
        public AuthorizeUsersAttribute() : base(PolicyNames.UserPolicy)
        {
            AuthenticationSchemes = "Bearer";
        }
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeManagersAttribute : AuthorizeAttribute
    {
        public AuthorizeManagersAttribute() : base(PolicyNames.ManagerPolicy)
        {
            AuthenticationSchemes = "Bearer";
        }
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeAdminsAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminsAttribute() : base(PolicyNames.AdminPolicy)
        {
            AuthenticationSchemes = "Bearer";
        }
    }
}