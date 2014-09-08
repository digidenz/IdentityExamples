using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DD.Cloud.Aperture.Identity.Example
{
	using Common;
	using Contracts;
	using Platform.Contracts;
	using Constants = Common.Constants;

	/// <summary>
	///		Unit-test class for showing how to use Identity Context to secure pages.
	/// </summary>
	[TestClass]
	public class IdentityContextTests
	{
		/// <summary>
		///		The unit-test execution context.
		/// </summary>
		public TestContext TestContext
		{
			get;
			set;
		}

		/// <summary>
		///		Build the Autofac container.
		/// </summary>
		/// <returns>
		///		The Autofac container.
		/// </returns>
		public IContainer BuildContainer()
		{
			ContainerBuilder containerBuilder = new ContainerBuilder();

			// Auditing
			containerBuilder
				.ConfigureAuditing()
				.DirectToDatabase(Constants.AuditingConnectionString);

			containerBuilder
				.ConfigureIdentity()
				.WithAuthUsingDatabase(Constants.IdentityConnectionString)
				.WithManagementApiUsingDatabase(Constants.IdentityConnectionString);

			return containerBuilder.Build();
		}

		/// <summary>
		///		Resolve the Identity Context.
		/// </summary>
		[TestMethod]
		public void ResolveIdentityContext()
		{
			using (IContainer container = BuildContainer())
			{
				User currentUser = Data.Users.JohnDoe;

				using (new IdentityContextScope(Helpers.GetUserPrincipal(currentUser), container))
				{
					Assert.IsNotNull(IdentityContext.Current);
					Assert.AreEqual(currentUser.Id, IdentityContext.Current.PrincipalId);
				}
			}
		}

		/// <summary>
		///		Test Identity Rights.
		/// </summary>
		[TestMethod]
		public void RightTests()
		{
			using (IContainer container = BuildContainer())
			{
				Organization[] organizationsToCheck =
				{
					Data.Organizations.Cloud,
					Data.Organizations.Australia,
					Data.Organizations.Optus,
					Data.Organizations.Vodafone,
					Data.Organizations.Usa
				};

				User[] usersToCheck =
				{
					Data.Users.JohnDoe,
					Data.Users.MattSmith,
					Data.Users.AlexWoods,
					Data.Users.JennySmith
				};

				const string activity = ApertureAccessControl.Activity.RequestSecurityToken;

				foreach (User user in usersToCheck)
				{
					using (new IdentityContextScope(Helpers.GetUserPrincipal(user), container))
					{
						Assert.IsNotNull(IdentityContext.Current);
						foreach (Organization organization in organizationsToCheck)
						{
							TestContext.WriteLine(
								"User '{0}' has {1}right to perform activity '{2}' under organization '{3}'.",
								user.DisplayName,
								IdentityContext.Current.HasRight(ServiceType.System, activity, organization.Id) ? String.Empty : "no ",
								activity,
								organization.Name
							);
							TestContext.WriteLine(String.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		///		Test Identity Resource Type Permissions.
		/// </summary>
		[TestMethod]
		public void ResourceTypePermissionTests_Department()
		{
			using (IContainer container = BuildContainer())
			{
				Organization[] organizationsToCheck =
				{
					Data.Organizations.Cloud,
					Data.Organizations.Australia,
					Data.Organizations.Optus,
					Data.Organizations.Vodafone,
					Data.Organizations.Usa
				};

				User[] usersToCheck =
				{
					Data.Users.JohnDoe,
					Data.Users.MattSmith,
					Data.Users.AlexWoods,
					Data.Users.JennySmith
				};

				const string activity = ApertureAccessControl.Activity.ReadResource;
				const string resourceType = Data.ResourceTypeNames.SystemDepartment;

				foreach (User user in usersToCheck)
				{
					using (new IdentityContextScope(Helpers.GetUserPrincipal(user), container))
					{
						Assert.IsNotNull(IdentityContext.Current);
						foreach (Organization organization in organizationsToCheck)
						{
							TestContext.WriteLine(
								"User '{0}' has {1}permission to perform activity '{2}' for resource type '{3}' under organization '{4}'.",
								user.DisplayName,
								IdentityContext.Current.HasResourceTypePermission(ServiceType.System, activity, resourceType, organization.Id) ? String.Empty : "no ",
								activity,
								resourceType,
								organization.Name
							);
							TestContext.WriteLine(String.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		///		Test Identity Resource Permissions.
		/// </summary>
		[TestMethod]
		public void ResourcePermissionTests_NorthRyde()
		{
			using (IContainer container = BuildContainer())
			{
				User[] usersToCheck =
				{
					Data.Users.JohnDoe,
					Data.Users.MattSmith,
					Data.Users.AlexWoods,
					Data.Users.JennySmith
				};

				const string activity = ApertureAccessControl.Activity.ReadResource;
				Resource resource = Data.PlatformResources.LocationNorthRyde;

				foreach (User user in usersToCheck)
				{
					using (new IdentityContextScope(Helpers.GetUserPrincipal(user), container))
					{
						Assert.IsNotNull(IdentityContext.Current);
						TestContext.WriteLine(
							"User '{0}' has {1}permission to perform activity '{2}' for resource '{3}'.",
							user.DisplayName,
							IdentityContext.Current.HasResourcePermission(ServiceType.System, activity, resource.Id) ? String.Empty : "no ",
							activity,
							resource.Name
						);
						TestContext.WriteLine(String.Empty);
					}
				}
			}
		}

		/// <summary>
		///		Test Identity Resource Permissions.
		/// </summary>
		[TestMethod]
		public void ResourcePermissionTests_TheRocks()
		{
			using (IContainer container = BuildContainer())
			{
				User[] usersToCheck =
				{
					Data.Users.JohnDoe,
					Data.Users.MattSmith,
					Data.Users.AlexWoods,
					Data.Users.JennySmith
				};

				const string activity = ApertureAccessControl.Activity.ReadResource;
				Resource resource = Data.PlatformResources.LocationTheRocks;

				foreach (User user in usersToCheck)
				{
					using (new IdentityContextScope(Helpers.GetUserPrincipal(user), container))
					{
						Assert.IsNotNull(IdentityContext.Current);
						TestContext.WriteLine(
							"User '{0}' has {1}permission to perform activity '{2}' for resource '{3}'.",
							user.DisplayName,
							IdentityContext.Current.HasResourcePermission(ServiceType.System, activity, resource.Id) ? String.Empty : "no ",
							activity,
							resource.Name
						);
						TestContext.WriteLine(String.Empty);
					}
				}
			}
		}
	}
}
