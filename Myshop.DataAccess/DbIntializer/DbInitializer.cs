using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mshop.DataAccess;
using Mshop.Entities.Models;
using Shop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace Myshop.DataAccess.DbIntializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {

            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }

            //Roles
            if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
            }


            // User 

            //var user = new ApplicationUser
            //{
            //    UserName = "Admin0@admin.com",
            //    Email = "Admin0@admin.com",
            //    Name = "Admin",
            //    PhoneNumber = "1234567890",
            //    Adderss = "TantaStreet8",
            //    City = "Alexandria"
            //};


            //var result = _userManager.CreateAsync(user, "Admin0@admin.com").GetAwaiter().GetResult();

            //if (!result.Succeeded) 
            //{
            //    // handle the error
            //}

            //var adminRoleExists = _roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult();

            //if (!adminRoleExists)
            //{
            //    // Handel error
            //}


            //_userManager.AddToRolesAsync(user.Id, new string[] { "Admin" }).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "Admin0@admin.com",
                Email = "Admin0@admin.com",
                PhoneNumber = "1234567890",
                Name = "Administrator",
                City = "Alexandria",
                Adderss = "Alexandria",
            }, "Admin0@admin.com").GetAwaiter().GetResult();

            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u=> u.Email == "Admin0@admin.com");

            _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();

        }

        
    }
}
