using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MailHole.Db.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MailHole.Api.Auth
{
    public class AuthSeeder
    {
        private const string AdminUserName = "admin";
        private const string AdminUserMail = "admin@mailhole.io";
        private const string AdminUserPassword = "1n1t-R00t!";

        private readonly IReadOnlyList<MailHoleRole> _rolesToSeed = new List<MailHoleRole>
        {
            new MailHoleRole {Name = MailHoleRoles.User},
            new MailHoleRole {Name = MailHoleRoles.Admin}
        };

        private readonly RoleManager<MailHoleRole> _roleManager;
        private readonly UserManager<MailHoleUser> _userManager;

        public AuthSeeder(UserManager<MailHoleUser> userManager, RoleManager<MailHoleRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAdminUser()
        {
            foreach (var role in _rolesToSeed)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    await _roleManager.CreateAsync(role);
                    await _roleManager.AddClaimAsync(role, new Claim(ClaimTypes.Role, role.Name));
                }
            }

            if (!await _userManager.Users.AnyAsync(u => u.UserName == AdminUserName))
            {
                await _userManager.CreateAsync(new MailHoleUser
                {
                    UserName = AdminUserName,
                    Email = AdminUserMail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                }, AdminUserPassword);

                var adminUser = await _userManager.Users.FirstAsync(u => u.UserName == AdminUserName);

                await _userManager.AddToRolesAsync(adminUser, new[] {MailHoleRoles.Admin, MailHoleRoles.User});
            }
        }
    }
}