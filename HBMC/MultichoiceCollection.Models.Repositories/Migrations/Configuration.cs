using System.Collections.Generic;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MultichoiceCollection.Common.Entities.Base;
using MultichoiceCollection.Models.Repositories.Context;

namespace MultichoiceCollection.Models.Repositories.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<MultichoiceCollection.Models.Repositories.Context.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MultichoiceCollection.Models.Repositories.Context.AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
           
            var passwordHasher = new PasswordHasher();
            context.Users.AddOrUpdate(u => u.UserName, new ApplicationUser
            {
                AgentName = "Admin",
                AccountNumber = "Admin",
                Email = "admin@multichoice.com",
                PhoneNumber = "08056376287",
                UserName = "admin@multichoice.com",
                PasswordHash = passwordHasher.HashPassword("admin"),
                SecurityStamp = "hjsdjkdf90emnfkldflkjodkjdfkjkj"
            });
            context.SaveChanges();
            // var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new AppDbContext()));
            if (!roleManager.RoleExists(RoleNames.RoleAdmin))
            {
                roleManager.Create(new IdentityRole(RoleNames.RoleAdmin));
            }
            if (!roleManager.RoleExists(RoleNames.RoleCustomer))
            {
                roleManager.Create(new IdentityRole(RoleNames.RoleCustomer));
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = userManager.FindByName("admin@multichoice.com");
            var rolesForUser = Roles.GetRolesForUser("admin@multichoice.com");
            if (!rolesForUser.Any(r => r.Contains(RoleNames.RoleAdmin)))
                userManager.AddToRole(user.Id, RoleNames.RoleAdmin);
            
            context.SaveChanges();

        }
    }
}
