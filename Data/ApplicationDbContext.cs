
using ElamIgugu.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElamIgugu.Web.Data;


public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<DonationProof> DonationProofs => Set<DonationProof>();
    public DbSet<Webinar> Webinars => Set<Webinar>();
    public DbSet<WebinarRsvp> WebinarRsvps => Set<WebinarRsvp>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Donation>(e =>
        {
            e.HasOne(d => d.Donor)
             .WithMany(u => u.Donations)
             .HasForeignKey(d => d.DonorId)
             .OnDelete(DeleteBehavior.Restrict);

            
            e.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(d => d.Status);

            
            e.HasOne(d => d.Proof)
             .WithOne(p => p.Donation)
             .HasForeignKey<DonationProof>(p => p.DonationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<WebinarRsvp>(e =>
        {
            
            e.HasOne(r => r.Webinar)
             .WithMany(w => w.Rsvps)
             .HasForeignKey(r => r.WebinarId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
















































































//using ElamIgugu.Web.Models;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

//namespace ElamIgugu.Web.Data;

///// <summary>
///// The bridge between the C# classes and the SQL database. Inheriting IdentityDbContext
///// gives us the user, role and login tables for free; we add our own three tables.
///// </summary>
//public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//{
//    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
//    {
//    }

//    public DbSet<Donation> Donations => Set<Donation>();
//    public DbSet<Webinar> Webinars => Set<Webinar>();
//    public DbSet<WebinarRsvp> WebinarRsvps => Set<WebinarRsvp>();

//    protected override void OnModelCreating(ModelBuilder builder)
//    {
//        base.OnModelCreating(builder);

//        builder.Entity<Donation>(e =>
//        {
//            // One donor has many donations. Deleting a donor must not silently delete donations.
//            e.HasOne(d => d.Donor)
//             .WithMany(u => u.Donations)
//             .HasForeignKey(d => d.DonorId)
//             .OnDelete(DeleteBehavior.Restrict);

//            // Stored as the words "Submitted" / "Received" so the database is readable
//            e.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
//            e.HasIndex(d => d.Status);
//        });

//        builder.Entity<WebinarRsvp>(e =>
//        {
//            // One webinar has many RSVPs; removing a webinar removes its RSVPs
//            e.HasOne(r => r.Webinar)
//             .WithMany(w => w.Rsvps)
//             .HasForeignKey(r => r.WebinarId)
//             .OnDelete(DeleteBehavior.Cascade);
//        });
//    }
//}
