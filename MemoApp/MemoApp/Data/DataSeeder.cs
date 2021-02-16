using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.Data
{
    public class DataSeeder
    {
        private readonly MemoEntities _entities;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(MemoEntities entities, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _entities = entities;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedStatusData()
        {
            try
            {
                if (!_entities.Statuses.Any())
                {
                    _entities.Statuses.AddRange(
                        new Status
                        {
                            Id = 1,
                            Name = "Active",
                            Description = "The memo is active."
                        },
                        new Status
                        {
                            Id = 2,
                            Name = "Deleted",
                            Description = "The memo is deleted."
                        });
                    _entities.SaveChanges();
                    Log.Information("Status data successfully added to the database.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to add status data to the database: {ex.Message}");
            }
        }

        public async Task CreateAdminRole()
        {
            try
            {
                //checking if admin role exist
                bool existAdminRole = await _roleManager.RoleExistsAsync("Admin");
                if (!existAdminRole)
                {
                    //creating role for administrator
                    var adminRole = new IdentityRole
                    {
                        Name = "Admin"
                    };
                    var resultOfCreatingAdminRole = await _roleManager.CreateAsync(adminRole);
                    //if admin role creating succeeded, create the admin
                    if (resultOfCreatingAdminRole.Succeeded)
                    {
                        var admin = new IdentityUser
                        {
                            UserName = "admin@gmail.com",
                            Email = "admin@gmail.com"
                        };
                        var resultOfCreating = await _userManager.CreateAsync(admin, "P@ssw0rd");
                        //if user creation succeeded, assigning administrator role to the user
                        if (resultOfCreating.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(admin, "Admin");
                            _entities.SaveChanges();
                            Log.Information("Admin role and user successfully added to the database.");
                        }                        
                    }                   
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to add Admin role and user to the database: {ex.Message}");
            }
        }

        public async Task CreateUserRole()
        {
            try
            {
                //checking if user role exists
                var existUserRole = await _roleManager.RoleExistsAsync("User");
                if (!existUserRole)
                {
                    //creating role for user
                    var userRole = new IdentityRole
                    {
                        Name = "User"
                    };
                    var resultOfCreating = await _roleManager.CreateAsync(userRole);
                    if (resultOfCreating.Succeeded)
                    {
                        _entities.SaveChanges();
                        Log.Information("User role successfully added to database.");
                    }                    
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to add User role to the database: {ex.Message}");
            }
        }
    }
}
