//This file is auto generated on 1/17/2022 9:52:53 AM
namespace BPX.Website.CustomCode.Authorize
{
	public static class Permits
	{
		public static class Admin
		{
			public static class GenerateScripts
			{
				public const int PermitConstants = 1;            //Admin.GenerateScripts.PermitConstants
			}

		}

		public static class Identity
		{
			public static class Permit
			{
				public const int Create = 18;            //Identity.Permit.Create
				public const int Delete = 21;            //Identity.Permit.Delete
				public const int Filter = 23;            //Identity.Permit.Filter
				public const int List = 22;              //Identity.Permit.List
				public const int ListDeleted = 24;           //Identity.Permit.ListDeleted
				public const int Read = 19;              //Identity.Permit.Read
				public const int Restore = 25;           //Identity.Permit.Restore
				public const int Update = 20;            //Identity.Permit.Update
			}

			public static class Role
			{
				public const int Create = 10;            //Identity.Role.Create
				public const int Delete = 13;            //Identity.Role.Delete
				public const int Filter = 15;            //Identity.Role.Filter
				public const int List = 14;              //Identity.Role.List
				public const int ListDeleted = 16;           //Identity.Role.ListDeleted
				public const int Read = 11;              //Identity.Role.Read
				public const int Restore = 17;           //Identity.Role.Restore
				public const int Update = 12;            //Identity.Role.Update
			}

			public static class RolePermit
			{
				public const int CRUD = 27;              //Identity.RolePermit.CRUD
			}

			public static class User
			{
				public const int Create = 2;             //Identity.User.Create
				public const int Delete = 5;             //Identity.User.Delete
				public const int Filter = 7;             //Identity.User.Filter
				public const int List = 6;           //Identity.User.List
				public const int ListDeleted = 8;            //Identity.User.ListDeleted
				public const int Read = 3;           //Identity.User.Read
				public const int Restore = 9;            //Identity.User.Restore
				public const int Update = 4;             //Identity.User.Update
			}

			public static class UserRole
			{
				public const int CRUD = 26;              //Identity.UserRole.CRUD
			}

		}

		public static class Root
		{
			public static class Menu
			{
				public const int Create = 28;            //Root.Menu.Create
				public const int Delete = 31;            //Root.Menu.Delete
				public const int Filter = 33;            //Root.Menu.Filter
				public const int List = 32;              //Root.Menu.List
				public const int ListDeleted = 34;           //Root.Menu.ListDeleted
				public const int Read = 29;              //Root.Menu.Read
				public const int Restore = 35;           //Root.Menu.Restore
				public const int Update = 30;            //Root.Menu.Update
			}

		}

	}
}
