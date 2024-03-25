using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data.Models;

namespace MovieAPI.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<MoviesComments> MoviesComments { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<MoviesRatings> MoviesRatings { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<MoviesImages> MoviesImages { get; set; } = null!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MoviesComments>().HasKey(x => new { x.MovieId, x.CommentId });
            builder.Entity<MoviesRatings>().HasKey(x => new { x.MovieId, x.RatingId });
            builder.Entity<MoviesImages>().HasKey(x => new { x.MovieId, x.ImageId });

            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole()
                {
                    Name = "User",
                    NormalizedName = "USER"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            var hasher = new PasswordHasher<ApplicationUser>();
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin");

            builder.Entity<ApplicationUser>().HasData(adminUser);
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roles[0].Id,
                UserId = adminUser.Id
            });

            base.OnModelCreating(builder);
        }

    }
}
