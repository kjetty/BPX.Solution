using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX.Domain.CustomModels
{
	public class DeveloperMeta
	{
		public string ViewBagOverride { get; set; }
		public string PermitAttributeOverride { get; set; }
		public string PasswordOverride { get; set; }

		public DeveloperMeta()
		{
			ViewBagOverride = "NO";
			PermitAttributeOverride = "NO";
			PasswordOverride = "NO";
		}
	}
}
