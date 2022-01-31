//This file is auto generated on 1/31/2022 3:36:53 PM
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
			public static class Menu
			{
				public const int Create = 27;            //Root.Menu.Create
				public const int Delete = 30;            //Root.Menu.Delete
				public const int Filter = 32;            //Root.Menu.Filter
				public const int List = 31;              //Root.Menu.List
				public const int ListDeleted = 33;           //Root.Menu.ListDeleted
				public const int Read = 28;              //Root.Menu.Read
				public const int Undelete = 34;              //Root.Menu.Undelete
				public const int Update = 29;            //Root.Menu.Update
			}

			public static class MenuPermit
			{
				public const int CRUD = 37;              //Identity.MenuPermit.CRUD
			}

			public static class Permit
			{
				public const int Create = 19;            //Identity.Permit.Create
				public const int Delete = 22;            //Identity.Permit.Delete
				public const int Filter = 24;            //Identity.Permit.Filter
				public const int List = 23;              //Identity.Permit.List
				public const int ListDeleted = 25;           //Identity.Permit.ListDeleted
				public const int Read = 20;              //Identity.Permit.Read
				public const int Undelete = 26;              //Identity.Permit.Undelete
				public const int Update = 21;            //Identity.Permit.Update
			}

			public static class Role
			{
				public const int Create = 11;            //Identity.Role.Create
				public const int Delete = 14;            //Identity.Role.Delete
				public const int Filter = 16;            //Identity.Role.Filter
				public const int List = 15;              //Identity.Role.List
				public const int ListDeleted = 17;           //Identity.Role.ListDeleted
				public const int Read = 12;              //Identity.Role.Read
				public const int Undelete = 18;              //Identity.Role.Undelete
				public const int Update = 13;            //Identity.Role.Update
			}

			public static class RolePermit
			{
				public const int CRUD = 36;              //Identity.RolePermit.CRUD
			}

			public static class User
			{
				public const int ChangePassword = 10;            //Identity.User.ChangePassword
				public const int Create = 2;             //Identity.User.Create
				public const int Delete = 5;             //Identity.User.Delete
				public const int Filter = 7;             //Identity.User.Filter
				public const int List = 6;           //Identity.User.List
				public const int ListDeleted = 8;            //Identity.User.ListDeleted
				public const int Read = 3;           //Identity.User.Read
				public const int Undelete = 9;           //Identity.User.Undelete
				public const int Update = 4;             //Identity.User.Update
			}

			public static class UserRole
			{
				public const int CRUD = 35;              //Identity.UserRole.CRUD
			}

		}

	}
}
