namespace KeelteKool.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KeelteKool.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(KeelteKool.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            var roleManager = new RoleManager<IdentityRole>(
    new RoleStore<IdentityRole>(context));

            string[] roles = { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }
            }

            // Create test users
            var userManager = new UserManager<Models.ApplicationUser>(
                new UserStore<Models.ApplicationUser>(context));

            // Admin user
            if (userManager.FindByEmail("admin@keeltekool.ee") == null)
            {
                var admin = new Models.ApplicationUser
                {
                    UserName = "admin@keeltekool.ee",
                    Email = "admin@keeltekool.ee"
                };
                userManager.Create(admin, "Admin123!");
                userManager.AddToRole(admin.Id, "Admin");
            }

            // Teacher user
            if (userManager.FindByEmail("teacher@keeltekool.ee") == null)
            {
                var teacher = new Models.ApplicationUser
                {
                    UserName = "teacher@keeltekool.ee",
                    Email = "teacher@keeltekool.ee"
                };
                userManager.Create(teacher, "Teacher123!");
                userManager.AddToRole(teacher.Id, "Teacher");

                // Create teacher profile
                var teacherUser = userManager.FindByEmail("teacher@keeltekool.ee");
                context.Teachers.AddOrUpdate(t => t.ApplicationUserId,
                    new Models.Teacher
                    {
                        Nimi = "Mari Maasikas",
                        Kvalifikatsioon = "Magistrikraad saksa keele õpetamises",
                        ApplicationUserId = teacherUser.Id
                    });
            }

            // Student user
            if (userManager.FindByEmail("student@keeltekool.ee") == null)
            {
                var student = new Models.ApplicationUser
                {
                    UserName = "student@keeltekool.ee",
                    Email = "student@keeltekool.ee"
                };
                userManager.Create(student, "Student123!");
                userManager.AddToRole(student.Id, "Student");
            }

            context.SaveChanges();

            // Add sample courses
            context.Courses.AddOrUpdate(c => c.Nimetus,
                new Models.Course
                {
                    Nimetus = "Saksa keel algajatele",
                    Keel = "Saksa",
                    Tase = "A1",
                    Kirjeldus = "Algajatele mõeldud saksa keele kursus"
                },
                new Models.Course
                {
                    Nimetus = "Inglise keel edasijõudnutele",
                    Keel = "Inglise",
                    Tase = "B2",
                    Kirjeldus = "Edasijõudnute inglise keele kursus"
                },
                new Models.Course
                {
                    Nimetus = "Vene keel algajatele",
                    Keel = "Vene",
                    Tase = "A1",
                    Kirjeldus = "Algajatele mõeldud vene keele kursus"
                });

            context.SaveChanges();

            // Add sample training
            var teacherProfile = context.Teachers.FirstOrDefault();
            var course = context.Courses.FirstOrDefault();
            if (teacherProfile != null && course != null)
            {
                context.Trainings.AddOrUpdate(t => new { t.CourseId, t.AlgusKuupaev },
                    new Models.Training
                    {
                        CourseId = course.Id,
                        TeacherId = teacherProfile.Id,
                        AlgusKuupaev = DateTime.Now.AddDays(7),
                        LoppKuupaev = DateTime.Now.AddDays(37),
                        Hind = 150.00m,
                        MaxOsalejaid = 12
                    });
            }

            context.SaveChanges();
        }
    }
}
