using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.Domain.Authorization;

using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Api.Configurations
{
    public class DefaultData
    {
        /// <summary>
        /// <c>Initialize</c> Offers extension for default data initialization
        /// </summary>
        /// <param name="app"></param>
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DataContext>();
                context.Database.EnsureCreated();

                // Populate Claims table if it is null or empty
                if (context.Claim == null || !context.Claim.Any())
                {
                    var claimModel = LoadDefaultClaims().ToArray();

                    var set = context.Set<Claim>().AsNoTracking().ToList();
                    context.Claim.AddRange(set);
                    context.Claim.AddRange(claimModel);
                    context.SaveChanges();
                }

                // Populate Roles table if it is null or empty
                if (context.Roles == null || !context.Roles.Any())
                {
                    var roleModel = LoadDefaultRoles().ToArray();

                    var set = context.Set<Role>().AsNoTracking().ToList();
                    context.Roles.AddRange(set);

                    context.Roles.AddRange(roleModel);
                    context.SaveChanges();
                }


                if (context.RoleAuthClaims == null || !context.RoleAuthClaim.Any())
                {
                    //Assign claims to the default roles created

                    // Admin
                    int[] ClaimsLevelI = new int[] { 1, 2, 3, 4 };
                    // policyHolder
                    int[] ClaimsLevelII = new int[] { };

                    SaveRoleWithClaims(ClaimsLevelI, 1, app);
                    SaveRoleWithClaims(ClaimsLevelII, 2, app);
                }
            }



            //Creates the default User and Party for admin and also add the Admin permission to it
            LoadDefaultAdminUser(app);

            //Continue application load
            return;
        }

        /// <summary>
        /// Update roleClaim table with matching claim for default roles
        /// </summary>
        /// <param name="arrayClaimId"></param>
        /// <param name="roleId"></param>
        /// <param name="app"></param>
        public static void SaveRoleWithClaims(int[] arrayClaimId, int roleId, IApplicationBuilder app)
        {
            foreach (int claimId in arrayClaimId)
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<DataContext>();
                    context.Database.EnsureCreated();

                    Role roleModel = new Role { RoleId = roleId };
                    context.Roles.Add(roleModel);
                    context.Roles.Attach(roleModel);

                    Claim claimModel = new Claim { ClaimId = claimId };
                    context.Claim.Add(claimModel);
                    context.Claim.Attach(claimModel);

                    RoleAuthClaim roleClaimModel = new RoleAuthClaim
                    {
                        ClaimId = claimId,
                        RoleId = roleId
                    };

                    roleModel.RoleClaims.Add(roleClaimModel);

                    // call SaveChanges
                    context.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Populates the Role table with default data
        /// </summary>
        /// <returns></returns>
        public static List<Role> LoadDefaultRoles()
        {
            List<Role> role = new List<Role>() {
                new Role
                {
                    Name = "Administrator",
                    Description = "Can review",
                    RoleType = "System",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Role
                {
                    Name = "PolicyHolder",
                    Description = "Can request",
                    RoleType = "System",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
            };

            return role;
        }

        /// <summary>
        /// Populates the Claim table with default data
        /// </summary>
        /// <returns></returns>
        public static List<Claim> LoadDefaultClaims()
        {
            List<Claim> claims = new List<Claim>() {
                new Claim
                {
                    Name = claim.AcceptRequest,
                    Description = "Accept Request",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },
                new Claim
                {
                    Name = claim.CanRequest,
                    Description = "Can Request",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },
                new Claim
                {
                    Name = claim.EditRequest,
                    Description = "Edit Request",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },
                new Claim
                {
                    Name = claim.DeclineREquest,
                    Description = "Decline Request",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },

            };

            return claims;
        }

        /// <summary>
        /// Populates the user and party table with default data
        /// </summary>
        /// <param name="app"></param>
        public static void LoadDefaultAdminUser(IApplicationBuilder app)
        {
            try
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<DataContext>();
                    context.Database.EnsureCreated();
                    if (context.Users != null && context.Users.Any())
                    {
                        return;
                    }

                    //Populates default user

                    var users = new List<User>
                    {
                        new User
                        {
                            Name = "Abc",
                            Surname = "Admin",
                            DateOfBirth = DateTime.Now,
                            NationalID = 01,
                            PolicyNumber = "001",
                            RoleId = 1,
                            IsActive = true,
                            Salt = "asasasa",
                            HashPassword = "qwie38eucnidc"
                        },
                        new User
                        {
                           Name = "xyz",
                            Surname = "policyHolder",
                            DateOfBirth = DateTime.Now,
                            NationalID = 09,
                            PolicyNumber = "002",
                            RoleId = 2,
                            IsActive = true,
                            Salt = "adidnknskns",
                            HashPassword = "ja3283u38nsk"
                        }
                    };

                    context.Users.AddRange(users);
                    context.SaveChanges();
                    context.Entry(users).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
