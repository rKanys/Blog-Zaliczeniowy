using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_Zaliczeniowy.Models
{
	public class Comment
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }

		// Informacja o autorze komentarza
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }

		// Powiązanie z postem (dla komentarzy na poziomie "głównym")
		public int? PostId { get; set; }
		public virtual Post Post { get; set; }

		// Klucz obcy do komentarza-rodzica (jeśli komentarz to odpowiedź na inny komentarz)
		public int? ParentCommentId { get; set; }
		public virtual Comment ParentComment { get; set; }

		// Kolekcja podrzędnych komentarzy (dzieci), czyli odpowiedzi
		public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
	}
}
