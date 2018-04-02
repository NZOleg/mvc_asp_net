using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";
        public static async Task EnsurePopulated(UserManager<AppUser>
        userManager)
        {
            IdentityUser user = await userManager.FindByIdAsync(adminUser);
            if (user == null)
            {
                user = new AppUser {
                    UserName = adminUser,
                    Email = adminUser //the only exception
                };

                await userManager.CreateAsync((AppUser)user, adminPassword);
            }
        }
    }
}
