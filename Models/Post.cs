namespace Blog_Zaliczeniowy.Models
{
	public class Post
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public string UserId { get; set; }  // Relacja z użytkownikiem
		public ApplicationUser User { get; set; }
		public List<Comment> Comments { get; set; } = new List<Comment>();
		public bool Visibility { get; set; } = true;
		public bool Approved { get; set; } = false; 
	}
}
