using ElamIgugu.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElamIgugu.Web.Data;

/// <summary>
/// Runs every time the app starts. It is safe to run repeatedly: it only adds what is missing.
///   1. Creates/updates the database from the migrations.
///   2. Makes sure the Admin and Donor roles exist.
///   3. Creates the first admin account from the "Seed" settings (admins never self-register).
///   4. Adds sample webinars if the Webinars table is empty.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");
        var db = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var config = services.GetRequiredService<IConfiguration>();

        // The tables are created from a "migration". If nobody has created it yet, say so clearly.
        if (!db.Database.GetMigrations().Any())
        {
            throw new InvalidOperationException(
                "No database migration found. Run:  dotnet ef migrations add InitialCreate  (see README, step 4), then start the app again.");
        }

        await db.Database.MigrateAsync();

        foreach (var role in new[] { AppRoles.Admin, AppRoles.Donor })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = config["Seed:AdminEmail"];
        var adminPassword = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("No admin account created: set Seed:AdminEmail and Seed:AdminPassword (see README).");
        }
        else if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "NPO Admin",
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
                logger.LogInformation("Admin account created for {Email}.", adminEmail);
            }
            else
            {
                logger.LogError("Could not create admin: {Errors}",
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        // Sample webinars. Replace the topics and speakers with the NPO's real sessions.
        if (!await db.Webinars.AnyAsync())
        {
            var next = DateTime.Today;
            db.Webinars.AddRange(
                new Webinar { Topic = "Sample: Careers in technology",  StartTime = next.AddDays(14).AddHours(10), Speaker = "To be confirmed" },
                new Webinar { Topic = "Sample: Careers in health care", StartTime = next.AddDays(28).AddHours(10), Speaker = "To be confirmed" },
                new Webinar { Topic = "Sample: Applying for bursaries", StartTime = next.AddDays(42).AddHours(10), Speaker = "To be confirmed" });
            await db.SaveChangesAsync();
        }
    }
}
