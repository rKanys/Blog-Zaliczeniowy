using System.ComponentModel.DataAnnotations;

namespace Blog_Zaliczeniowy.Models
{
	public class Post
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Tytuł jest wymagany")]
		[StringLength(40)]
		[Display(Name = "Tytuł")]
		public string Title { get; set; }
		[StringLength(8000)]
		[Display(Name = "Zawartość")]
		[Required(ErrorMessage = "Zawartość jest wymagana")]
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public string UserId { get; set; }  // Relacja z użytkownikiem
		public ApplicationUser User { get; set; }
		public List<Comment> Comments { get; set; } = new List<Comment>();
		public bool Visibility { get; set; } = true;
		public bool Approved { get; set; } = false; 
	}
}
