using System.Collections.Generic;

namespace BPX.Domain.CustomModels
{
	public class UserMeta
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
		public string NekotNigol { get; set; }
		public int UserId { get; set; }

		public UserMeta()
		{
			FirstName = string.Empty;
			LastName = string.Empty;
			FullName = string.Empty;
			Email = string.Empty;
			Mobile = string.Empty;
			NekotNigol = string.Empty;
			UserId = 0;
		}
	}
}
