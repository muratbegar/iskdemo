using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.Enums;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Domain.Aggregates;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.Repos;
using ELearningIskoop.Users.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Infrastructure.Seeds
{
    public static class UserDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserRepository>>();

                try
                {
                    //Seed roles
                    await SeedRolesAsync(roleRepository, unitOfWork);

                    //Seed Admin user
                    await SeedAdminUserAsync(userRepository, roleRepository, unitOfWork);


                    // Seed Test Users
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        await SeedTestUsersAsync(userRepository, roleRepository, unitOfWork);
                    }

                    logger.LogInformation("User data seeding completed successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding user data");
                    throw;
                }
            }
        }


        private static async Task SeedRolesAsync(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            var roles = new[]
            {
                ("Admin", "System administrator with full access"),
                ("Instructor", "Course instructor who can create and manage courses"),
                ("Student", "Student who can enroll and take courses"),
                ("Guest", "Guest user with limited access")
            };

            foreach (var (name, description) in roles)
            {
                var existingRole = await roleRepository.GetByNameAsync(name);
                if (existingRole == null)
                {
                    var role = Role.Create(name, description);

                    //Add permission
                    if (name == "Admin")
                    {
                        role.AddPermission("users.manage");
                        role.AddPermission("courses.manage");
                        role.AddPermission("system.configure");
                    }
                    else if (name == "Instructor")
                    {
                        role.AddPermission("courses.create");
                        role.AddPermission("courses.update");
                        role.AddPermission("courses.delete");
                        role.AddPermission("lessons.manage");
                    }
                    else if (name == "Student")
                    {
                        role.AddPermission("courses.view");
                        role.AddPermission("courses.enroll");
                        role.AddPermission("lessons.view");
                    }

                    await roleRepository.AddAsync(role);
                }
            }

            await unitOfWork.SaveChangesAsync();
        }

        

        private static async Task SeedAdminUserAsync(IUserRepository userRepository, IRoleRepository roleRepository, 
            IUnitOfWork unitOfWork)
        {
            var adminEmail =new Email("admin@iskoop.org");
            
            var existingAdmin = await userRepository.GetByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var admin = User.Create(adminEmail, new PersonName("System", "Admin"), "Test123!",
                    enUserRole.Admin.GetHashCode());

                admin.VerifyEmail(enUserRole.Admin.GetHashCode());

                var adminRole = await roleRepository.GetByNameAsync(enUserRole.Admin.ToString());
                if (adminRole != null)
                {
                    admin.AssignRole(adminRole, enUserRole.Admin.GetHashCode());
                }

                

                await userRepository.AddAsync(admin);
                await unitOfWork.SaveChangesAsync();

            }
        }


        private static async Task SeedTestUsersAsync(IUserRepository userRepository, IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            var testUsers = new[]
            {
                ("instructor@test.com", "Test", enUserRole.Instructor.ToString(), "Test123!", enUserRole.Instructor.ToString()),
                ("student@test.com", "Test", enUserRole.Student.ToString(), "Test123!", enUserRole.Student.ToString())
            };

            foreach (var (email, firstName, lastName, password, roleName) in testUsers)
            {
                var ConvertEmail = new Email(email);
                var existingUser = await userRepository.GetByEmailAsync(ConvertEmail);
                if (existingUser == null)
                {
                    var user = User.Create(
                       ConvertEmail,
                        new PersonName(firstName, lastName),
                        password,
                        enUserRole.Admin.GetHashCode()
                        );


                    user.VerifyEmail(enUserRole.Admin.GetHashCode());

                    var role = await roleRepository.GetByNameAsync(roleName);
                    if (role != null)
                    {
                        user.AssignRole(role, enUserRole.Admin.GetHashCode());
                    }



                    await userRepository.AddAsync(user);

                    //await refreshTokenRepository.AddAsync(RefreshToken.Create(user.ObjectId,"","",7);
                }
            }

            await unitOfWork.SaveChangesAsync();
        }



    }

}
