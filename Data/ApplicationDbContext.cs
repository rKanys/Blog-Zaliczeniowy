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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Konfiguracja dla Post-Comment: w tym przypadku wyłączenie kaskady na usuwanie
			//modelBuilder.Entity<Comment>()
			//	.HasOne(c => c.Post)
			//	.WithMany(p => p.Comments)
			//	.HasForeignKey(c => c.PostId)
			//	.OnDelete(DeleteBehavior.Restrict);  // Można także użyć DeleteBehavior.SetNull


			// Relacja jeden-do-wielu: jeden komentarz może mieć wiele odpowiedzi
			modelBuilder.Entity<Comment>()
				.HasOne(c => c.ParentComment)
				.WithMany(c => c.Replies)
				.HasForeignKey(c => c.ParentCommentId)
				.OnDelete(DeleteBehavior.Restrict);
			// lub Cascade, ale Restrict może być bezpieczniejsze, aby nie usuwać łańcuchowo wszystkich dzieci
		}
		// Możesz dodać podobne konfiguracje dla innych relacji, jeśli je masz.
	}
}
