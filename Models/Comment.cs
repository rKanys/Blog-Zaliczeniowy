using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_Zaliczeniowy.Models
{
	public class Comment
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int PostId { get; set; }  // Relacja z postem
		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public string UserId { get; set; }  // Relacja z użytkownikiem
		public ApplicationUser User { get; set; }
	}
}
