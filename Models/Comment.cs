using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_Zaliczeniowy.Models
{
	public class Comment
	{
		public int Id { get; set; }
		[Required]
		[Display(Name = "Komentarz")]
		[StringLength(2000)]
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }

		public int? PostId { get; set; }
		public virtual Post Post { get; set; }

		public int? ParentCommentId { get; set; }
		public virtual Comment ParentComment { get; set; }

		public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

		public bool Visibility { get; set; } = true;
	}
}
