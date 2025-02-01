using Blog_Zaliczeniowy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog_Zaliczeniowy.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<ApplicationUser> Users { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<PrivateMessage> PrivateMessages { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Comment>()
				.HasOne(c => c.ParentComment)
				.WithMany(c => c.Replies)
				.HasForeignKey(c => c.ParentCommentId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PrivateMessage>()
				.HasOne(pm => pm.Sender)
				.WithMany()
				.HasForeignKey(pm => pm.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PrivateMessage>()
				.HasOne(pm => pm.Recipient)
				.WithMany()
				.HasForeignKey(pm => pm.RecipientId)
				.OnDelete(DeleteBehavior.Restrict);
		}

	}
}
