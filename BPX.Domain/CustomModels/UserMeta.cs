using System;
using System.Collections.Generic;

namespace BPX.Domain.CustomModels
{
	public class UserMeta
	{
		public string LoginToken { get; set; }
		public DateTime LastLoginDate { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
		public int UserId { get; set; }
		public List<int> UserRoleIds { get; set; }
		public List<int> UserPermitIds { get; set; }

		public UserMeta()
		{
			LoginToken = string.Empty;
			LastLoginDate = DateTime.MinValue;
			FirstName = string.Empty;
			LastName = string.Empty;
			FullName = string.Empty;
			Email = string.Empty;
			Mobile = string.Empty;
			UserId = 0;
			UserRoleIds = new List<int>() { };
			UserPermitIds = new List<int>() { };
		}
	}
}
