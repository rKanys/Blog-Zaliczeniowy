using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_Zaliczeniowy.Models
{
	public class AdvancedSearchViewModel
	{

		// Pola kryteriów wyszukiwania
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public string UserId { get; set; }

		// Wyniki wyszukiwania
		public List<Post> Results { get; set; } = new List<Post>();

		// Agregaty
		public int TotalCount { get; set; }   
		public int TotalContentLength { get; set; }
		public List<PostsByUserAggregate> PostsByUser { get; set; } = new List<PostsByUserAggregate>();
	}
	public class PostsByUserAggregate
	{
		public string UserId { get; set; }
		public string Nickname { get; set; }
		public int Count { get; set; }
		public int TotalContentLength { get; set; }
	}
}

