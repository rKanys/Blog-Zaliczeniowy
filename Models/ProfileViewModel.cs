namespace Blog_Zaliczeniowy.Models
{
	public class ProfileViewModel
	{
		public string UserId { get; set; }
		public string Nickname { get; set; }
		public DateTime RegistrationDate { get; set; }
		public DateTime? LastLoginDate { get; set; }
		public int LoginCount { get; set; }
		public int PostCount { get; set; }
		public bool IsOwner { get; set; }
	}
}
