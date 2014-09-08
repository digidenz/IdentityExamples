using System;

namespace DD.Cloud.Aperture.Identity.Example
{
	using System.Security.Claims;
	using Claims;
	using Contracts;

	/// <summary>
	///		Helper methods.
	/// </summary>
	public static class Helpers
	{
		/// <summary>
		///		Auditing database connection string.
		/// </summary>
		public const string AuditingConnectionString = "Data Source=.;Initial Catalog=Auditing;Integrated Security=True;MultipleActiveResultSets=True;Asynchronous Processing=False providerName=System.Data.SqlClient";

		/// <summary>
		///		Identity database connection string.
		/// </summary>
		public const string IdentityConnectionString = "Data Source=.;Initial Catalog=IdentityModelTests;Integrated Security=True;MultipleActiveResultSets=True;Asynchronous Processing=False providerName=System.Data.SqlClient";

		/// <summary>
		///		Organizations used in repository tests.
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
			///		Organization 'Sydney'
			/// </summary>
			public static readonly Organization Sydney = new Organization
			{
				Id = Guid.Parse("6bb83544-0af1-44ae-a3ba-3657a8600256"),
				Level = 2,
				Name = "Sydney",
				ParentOrganizationId = Australia.Id,
				PrimaryDomainName = "sydney.dimensiondata.com.au",
				DefaultIdentityProviderId = null,
				CreatedUtc = DateTime.UtcNow,
				UpdatedUtc = null,
				UpdateToken = Guid.NewGuid()
			};

			/// <summary>
			///		Organization 'Melbourne'
			/// </summary>
			public static readonly Organization Melbourne = new Organization
			{
				Id = Guid.Parse("a7afd863-d6f9-4839-b965-de80941ad88d"),
				Level = 2,
				Name = "Melbourne",
				ParentOrganizationId = Australia.Id,
				PrimaryDomainName = "melbourne.dimensiondata.com.au",
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
		///		Users used in repository tests.
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
		///		Create a <see cref="ClaimsPrincipal"/> representing a registered user.
		/// </summary>
		/// <param name="user">
		///		The user to get the ClaimsPrincipal for.
		/// </param>
		/// <returns>
		///		The <see cref="ClaimsPrincipal"/>.
		/// </returns>
		public static ClaimsPrincipal GetUserPrincipal(User user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			Organization matchingOrganization = null;
			if (user.OrganizationId == Organizations.Cloud.Id)
				matchingOrganization = Organizations.Cloud;
			else if (user.OrganizationId == Organizations.Australia.Id)
				matchingOrganization = Organizations.Australia;

			if (matchingOrganization == null)
				throw new InvalidOperationException("Unrecognized organization Id from user.");

			ClaimsPrincipal authenticatedPrincipal = new ClaimsPrincipal(
				new ClaimsIdentity[]
				{
					new ClaimsIdentity(
						new Claim[]
						{
							new Claim(
								type: ClaimTypes.Name,
								value: user.DisplayName,
								valueType: ClaimValueTypes.String
							),
							new Claim(
								type: ClaimTypes.Upn,
								value: user.UserPrincipalName,
								valueType: ClaimValueTypes.String
							),
							new Claim(
								type: ApertureClaimTypes.SecurityPrincipalId,
								value: user.Id.ToString(),
								valueType: ClaimValueTypes.String
							),
							new Claim(
								type: ApertureClaimTypes.OrganizationId,
								value: matchingOrganization.Id.ToString(),
								valueType: ClaimValueTypes.String
							),
							new Claim(
								type: ApertureClaimTypes.OrganizationName,
								value: matchingOrganization.Name,
								valueType: ClaimValueTypes.String
							),
							new Claim(
								type: ApertureClaimTypes.AuthenticationType,
								value: ApertureAuthenticationTypes.Registered,
								valueType: ClaimValueTypes.String
							)
						},
						ApertureAuthenticationTypes.Registered,
						ClaimTypes.Name,
						ApertureClaimTypes.ActivityRight
					)
				}
			);

			return authenticatedPrincipal;
		}
	}
}
