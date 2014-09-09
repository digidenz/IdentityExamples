using System;

namespace DD.Cloud.Aperture.Identity.Example.Common
{
	using Contracts;
	using Platform.Contracts;

	/// <summary>
	///		Data used for Identity examples.
	/// </summary>
	public static class Data
	{
		/// <summary>
		///		Organizations used in examples.
		/// </summary>
		public static class Organizations
		{
			/// <summary>
			///		Organization 'Cloud'
			/// </summary>
			public static readonly Organization Cloud = new Organization
			{
				Id = Guid.Parse("e98edfc6-fdff-4e82-bac8-93f16bf1f697"),
				Level = 0,
				Name = "Dimension Data Cloud",
				ParentOrganizationId = null,
				PrimaryDomainName = "dimensiondata.com",
				DefaultIdentityProviderId = null,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Organization 'Australia'
			/// </summary>
			public static readonly Organization Australia = new Organization
			{
				Id = Guid.Parse("3675860b-cce9-47e4-92a1-00729c2a0e2b"),
				Level = 1,
				Name = "Australia",
				ParentOrganizationId = Cloud.Id,
				PrimaryDomainName = "dimensiondata.com.au",
				DefaultIdentityProviderId = null,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Organization 'Optus' under Sydney.
			/// </summary>
			public static readonly Organization Optus = new Organization
			{
				Id = Guid.Parse("6bb83544-0af1-44ae-a3ba-3657a8600256"),
				Level = 2,
				Name = "Optus",
				ParentOrganizationId = Australia.Id,
				PrimaryDomainName = "optus.com.au",
				DefaultIdentityProviderId = null,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Organization 'Vodafone'
			/// </summary>
			public static readonly Organization Vodafone = new Organization
			{
				Id = Guid.Parse("a7afd863-d6f9-4839-b965-de80941ad88d"),
				Level = 2,
				Name = "Vodafone",
				ParentOrganizationId = Australia.Id,
				PrimaryDomainName = "vodafone.com.au",
				DefaultIdentityProviderId = null,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Organization 'Usa'
			/// </summary>
			public static readonly Organization Usa = new Organization
			{
				Id = Guid.Parse("530f3cee-ff66-45d6-85b4-2833b3e0666f"),
				Level = 1,
				Name = "USA",
				ParentOrganizationId = Cloud.Id,
				PrimaryDomainName = "us.dimensiondata.com",
				DefaultIdentityProviderId = null,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};
		}

		/// <summary>
		///		Users used in examples.
		/// </summary>
		public static class Users
		{
			/// <summary>
			///		User 'John Doe'
			/// </summary>
			public static readonly User JohnDoe = new User
			{
				Id = Guid.Parse("81dbe033-e84f-43de-8ca8-d1ce025d2acf"),
				DisplayName = "John Doe",
				FirstName = "John",
				LastName = "Doe",
				OrganizationId = Organizations.Cloud.Id,
				PrincipalType = PrincipalType.User,
				UserPrincipalName = "John.Doe@dimensiondata.com",
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};

			/// <summary>
			///		User 'Matt Smith'
			/// </summary>
			public static readonly User MattSmith = new User
			{
				Id = Guid.Parse("314d1a02-8ee2-40dc-8cf8-9366f3750fbe"),
				DisplayName = "Matt Smith",
				FirstName = "Matt",
				LastName = "Smith",
				OrganizationId = Organizations.Australia.Id,
				PrincipalType = PrincipalType.User,
				UserPrincipalName = "Matt.Smith@dimensiondata.com.au",
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};

			/// <summary>
			///		User 'Alex Woods'
			/// </summary>
			public static readonly User AlexWoods = new User
			{
				Id = Guid.Parse("eb744a58-3eab-47fb-b39c-647c13e7a4b3"),
				DisplayName = "Alex Woods",
				FirstName = "Alex",
				LastName = "Woods",
				OrganizationId = Organizations.Australia.Id,
				PrincipalType = PrincipalType.User,
				UserPrincipalName = "Alex.Woods@dimensiondata.com.au",
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};

			/// <summary>
			///		User 'Jenny Smith'
			/// </summary>
			public static readonly User JennySmith = new User
			{
				Id = Guid.Parse("416b53ec-59ad-4509-9319-a09de0a907ad"),
				DisplayName = "Jenny Smith",
				FirstName = "Jenny",
				LastName = "Smith",
				OrganizationId = Organizations.Australia.Id,
				PrincipalType = PrincipalType.User,
				UserPrincipalName = "Jenny.Smith@dimensiondata.com.au",
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};
		}

		/// <summary>
		///		Groups used in examples.
		/// </summary>
		public static class Groups
		{
			/// <summary>
			///		Group 'Cloud Admins'
			/// </summary>
			public static readonly Group CloudAdmins = new Group
			{
				Id = Guid.Parse("726cdd28-051a-4adb-880a-14ba681e9a21"),
				DisplayName = "Cloud Admins",
				Name = "Cloud Administrators",
				Description = "Administrators for the root Cloud organization",
				OrganizationId = Organizations.Cloud.Id,
				PrincipalType = PrincipalType.Group,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};

			/// <summary>
			///		Group 'Australia Admins'
			/// </summary>
			public static readonly Group AustraliaAdmins = new Group
			{
				Id = Guid.Parse("e811e3a2-dfd8-4d75-9525-e9e04a39c7b2"),
				DisplayName = "Australia Admins",
				Name = "Australia Administrators",
				Description = "Administrators for Cloud Australia",
				OrganizationId = Organizations.Australia.Id,
				PrincipalType = PrincipalType.Group,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};

			/// <summary>
			///		Group 'Australia Standard'
			/// </summary>
			public static readonly Group AustraliaStandard = new Group
			{
				Id = Guid.Parse("3604b4bc-c3ee-4a3f-b7fc-4a9bd830e754"),
				DisplayName = "Australia Standard",
				Name = "Australia Standard Users",
				Description = "Standard users for Cloud Australia",
				OrganizationId = Organizations.Australia.Id,
				PrincipalType = PrincipalType.Group,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};

			/// <summary>
			///		Group 'Optus Admins'
			/// </summary>
			public static readonly Group OptusAdmins = new Group
			{
				Id = Guid.Parse("bb0c824e-ef68-4d63-8fe0-37d257550513"),
				DisplayName = "Optus Admins",
				Name = "Optus Administrators",
				Description = "Administrators for Optus",
				OrganizationId = Organizations.Optus.Id,
				PrincipalType = PrincipalType.Group,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid(),
				Deleted = false
			};
		}

		/// <summary>
		///		Resource Type Names used in examples.
		/// </summary>
		public static class ResourceTypeNames
		{
			/// <summary>
			///		Resource Type 'Department' for System
			/// </summary>
			public const string SystemDepartment = "Department";

			/// <summary>
			///		Resource Type 'Location' for System
			/// </summary>
			public const string SystemLocation = "Location";
		}

		/// <summary>
		///		Platform resources used in examples.
		/// </summary>
		public static class PlatformResources
		{
			/// <summary>
			///		Department Resource 'HR Department' in Australia.
			/// </summary>
			public static readonly Resource DepartmentHR = new Resource
			{
				Id = new Guid("29076b71-2cda-40f8-b99a-4f62a39d711e"),
				Name = "HR Department",
				ServiceType = ServiceType.System,
				ResourceTypeName = ResourceTypeNames.SystemDepartment,
				OwnerId = Users.MattSmith.Id,
				OrganizationId = Users.MattSmith.OrganizationId,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Department Resource 'R and D Department' in Australia.
			/// </summary>
			public static readonly Resource DepartmentRD = new Resource
			{
				Id = new Guid("78fe0031-cb0a-4247-aaaa-52e6c34e1744"),
				Name = "R&D Department",
				ServiceType = ServiceType.System,
				ResourceTypeName = ResourceTypeNames.SystemDepartment,
				OwnerId = Groups.AustraliaAdmins.Id,
				OrganizationId = Groups.AustraliaAdmins.OrganizationId,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Location Resource 'North Ryde' in Australia.
			/// </summary>
			public static readonly Resource LocationNorthRyde = new Resource
			{
				Id = new Guid("b7a319eb-3c79-4d4f-8b29-696d250433cf"),
				Name = "North Ryde",
				ServiceType = ServiceType.System,
				ResourceTypeName = ResourceTypeNames.SystemLocation,
				OwnerId = Users.MattSmith.Id,
				OrganizationId = Users.MattSmith.OrganizationId,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Location Resource 'The Rocks' in Australia.
			/// </summary>
			public static readonly Resource LocationTheRocks = new Resource
			{
				Id = new Guid("741f8c4c-1338-4e5e-acd6-a01b34dd5a38"),
				Name = "The Rocks",
				ServiceType = ServiceType.System,
				ResourceTypeName = ResourceTypeNames.SystemLocation,
				OwnerId = Users.JennySmith.Id,
				OrganizationId = Users.JennySmith.OrganizationId,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};
		}
	}
}
