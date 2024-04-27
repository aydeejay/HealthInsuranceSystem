using HealthInsuranceSystem.Core.Data;
using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.Domain.Authorization;
using HealthInsuranceSystem.Core.Security;

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
                if (context.AuthClaims == null || !context.AuthClaims.Any())
                {
                    var claimModel = LoadDefaultClaims().ToArray();

                    var set = context.Set<AuthClaim>().AsNoTracking().ToList();
                    context.AuthClaims.AddRange(set);
                    context.AuthClaims.AddRange(claimModel);
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


                if (context.RoleAuthClaims == null || !context.RoleAuthClaims.Any())
                {
                    //Assign claims to the default roles created

                    // Admin
                    int[] ClaimsLevelI = new int[] { 1, 2, 3, 4 };

                    // Claim Processor
                    int[] ClaimsLevelII = new int[] { 1, 2, 3 };

                    //Policy Holder
                    int[] ClaimsLevelIII = new int[] { 4 };

                    SaveRoleWithClaims(ClaimsLevelI, 1, app);
                    SaveRoleWithClaims(ClaimsLevelII, 2, app);
                    SaveRoleWithClaims(ClaimsLevelIII, 3, app);
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
                    context.Claims.Add(claimModel);
                    context.Claims.Attach(claimModel);

                    RoleAuthClaim roleClaimModel = new RoleAuthClaim
                    {
                        AuthClaimId = claimId,
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
                    Name = RoleType.Administrator.ToString(),
                    Description = "The administrator of the system",
                    RoleType = "System Created",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Role
                {
                    Name = RoleType.ClaimProcessor.ToString(),
                    Description = "The user responsible for claim review",
                    RoleType = "System Created",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Role
                {
                    Name = RoleType.PolicyHolder.ToString(),
                    Description = "A policy holder inthe system",
                    RoleType = "System created",
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
        public static List<AuthClaim> LoadDefaultClaims()
        {
            List<AuthClaim> authClaims = new List<AuthClaim>() {
                new AuthClaim
                {
                    Name = Claims.CanViewAllUsers,
                    Description = "Allows a user to view all Users",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },
                new AuthClaim
                {
                    Name = Claims.CanViewAllClaims,
                    Description = "Allows a user to view all Claims",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },
                new AuthClaim
                {
                    Name = Claims.CanReview,
                    Description = "Allows a user to review Claims",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },

                new AuthClaim
                {
                    Name = Claims.CanEditClaims,
                    Description = "Allows a user to edit a claim",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                }
            };

            return authClaims;
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
                            FirstName = "Admin",
                            LastName = "Admin",
                            DateOfBirth = DateTime.Now,
                            NationalID = "AdminNationalId",
                            UserPolicyNumber = "MEfwibNUCYI=",
                            RoleId = 1,
                            IsActive = true,
                            Salt = "f178314aa377b0051d1565a9cc5e51c1",
                            HashPassword = "RO8NBn+uwzq4mUuNXgVPZqvwzyt0BSpq3Ih4QRVzsAg="
                        },
                        new User
                        {
                            FirstName = "ClaimProcessor",
                            LastName = "ClaimProcessor",
                            DateOfBirth = DateTime.Now,
                            NationalID = "ClaimProcessorNationalId",
                            UserPolicyNumber = "vNwNLd9lJBY=",
                            RoleId = 2,
                            IsActive = true,
                            Salt = "fb8f1d30cf96154c4953c531cc45d942",
                            HashPassword = "50tkzURyXXLr6kUeU/hnfuGVCRCVzqDLvfE5Eeru+WI="
                        }
                    };

                    context.Users.AddRange(users);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
