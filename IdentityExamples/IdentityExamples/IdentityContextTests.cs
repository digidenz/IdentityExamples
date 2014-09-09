using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DD.Cloud.Aperture.Identity.Example
{
	using CloudServicesPortal.SecurityTemplate;
	using Common;
	using Contracts;
	using Contracts.Exceptions;
	using DistributedManagement;
	using Platform.Contracts;
	using Platform.Core.Linq;
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

			// Identity
			containerBuilder
				.ConfigureIdentity()
				.WithAuthUsingDatabase(Constants.IdentityConnectionString)
				.WithManagementApiUsingDatabase(Constants.IdentityConnectionString);

			// Security template importer
			containerBuilder
				.RegisterType<SecurityTemplateImporter>()
				.As<ISecurityTemplateImporter>()
				.InstancePerLifetimeScope();

			// Mock the DMS IProvisionedServiceManager and IResourceManager required to resolve the SecurityTemplateImporter
			containerBuilder
				.Register(
					componentContext =>
						(new Mock<IProvisionedServiceManager>()).Object
				)
				.As<IProvisionedServiceManager>()
				.InstancePerLifetimeScope();
			containerBuilder
				.Register(
					componentContext =>
						(new Mock<IResourceManager>()).Object
				)
				.As<IResourceManager>()
				.InstancePerLifetimeScope();

			return containerBuilder.Build();
		}

		/// <summary>
		///		Create an Identity Context scope using John Doe's data, then resolve the Identity Context.
		/// </summary>
		[TestMethod]
		public void ResolveIdentityContextTest()
		{
			using (IContainer container = BuildContainer())
			{
				User currentUser = Data.Users.JohnDoe;

				using (new IdentityContextScope(Helpers.GetUserPrincipal(currentUser), container))
				{
					Assert.IsNotNull(IdentityContext.Current);
					Assert.AreEqual(currentUser.Id, IdentityContext.Current.PrincipalId);
					Assert.AreEqual(currentUser.DisplayName, IdentityContext.Current.PrincipalName);
					Assert.AreEqual(currentUser.OrganizationId, IdentityContext.Current.OrganizationId);
				}
			}
		}

		/// <summary>
		///		Test Identity Rights.
		/// </summary>
		[TestMethod]
		public void RightTest()
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
		///		Test to demostrate demanding a Right using Identity Context.
		/// </summary>
		[TestMethod]
		public void DemandRightTest()
		{
			using (IContainer container = BuildContainer())
			{
				using (new IdentityContextScope(Helpers.GetUserPrincipal(Data.Users.JohnDoe), container))
				{
					Assert.IsNotNull(IdentityContext.Current);

					// Should not throw an exception as user John Doe should have the RequestSecurityToken Right under the Cloud organization.
					IdentityContext.Current.DemandRight(ServiceType.System, ApertureAccessControl.Activity.RequestSecurityToken);

					try
					{
						// Should throwAuthorizationException as user John Doe does not have the RequestSecurityToken Right under the Cloud organization.
						IdentityContext.Current.DemandRight(ServiceType.System, ApertureAccessControl.Activity.RequestSecurityTokenForThirdParty);
						Assert.Fail("Failed to throw AuthorizationException when the user does not have right.");
					}
					catch (AuthorizationException exception)
					{
						Assert.AreEqual(AuthorizationError.NotAuthorised, exception.Reason, "AuthorizationException Reason is not NotAuthorised");
					}
				}
			}
		}

		/// <summary>
		///		Test Identity Resource Type Permissions for Department resource type.
		/// </summary>
		[TestMethod]
		public void ResourceTypePermissionTest_Department()
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
		///		Test Identity Resource Type Permissions for Location resource type.
		/// </summary>
		[TestMethod]
		public void ResourceTypePermissionTest_Location()
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
				const string resourceType = Data.ResourceTypeNames.SystemLocation;

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
		public void ResourcePermissionTest_LocationNorthRyde()
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
		public void ResourcePermissionTest_LocationTheRocks()
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

		/// <summary>
		///		Test Identity Resource Permissions.
		/// </summary>
		[TestMethod]
		public void ResourcePermissionTest_DepartmentHR()
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
				Resource resource = Data.PlatformResources.DepartmentHR;

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
		public void ResourcePermissionTest_DepartmentRD()
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
				Resource resource = Data.PlatformResources.DepartmentRD;

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
		///		Test Identity effective Right.
		/// </summary>
		[TestMethod]
		public void EffectiveRightTest()
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

				const string intent = "SecurityTokens";

				foreach (User user in usersToCheck)
				{
					using (new IdentityContextScope(Helpers.GetUserPrincipal(user), container))
					{
						Assert.IsNotNull(IdentityContext.Current);
						foreach (Organization organization in organizationsToCheck)
						{
							ISet<string> allowedActivities = IdentityContext.Current.GetEffectiveRights(
								organization.Id,
								ServiceType.System,
								intent
							);

							if (allowedActivities.Any())
							{
								TestContext.WriteLine(
									"User '{0}' has right to perform activities '{1}' associated with intent '{2}' under organization '{3}'.",
									user.DisplayName,
									String.Join(", ", allowedActivities),
									intent,
									organization.Name
								);
							}
							else
							{
								TestContext.WriteLine(
									"User '{0}' has no right to perform any activities associated with intent '{1}' under organization '{2}'.",
									user.DisplayName,
									intent,
									organization.Name
								);
							}
							TestContext.WriteLine(String.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		///		Test Identity effective Resource Type Permission for Department resource type.
		/// </summary>
		[TestMethod]
		public void EffectiveResourceTypePermissionTest_Department()
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

				const string intent = ApertureAccessControl.Intent.ManageResources;
				const string resourceType = Data.ResourceTypeNames.SystemDepartment;

				foreach (User user in usersToCheck)
				{
					using (new IdentityContextScope(Helpers.GetUserPrincipal(user), container))
					{
						Assert.IsNotNull(IdentityContext.Current);
						foreach (Organization organization in organizationsToCheck)
						{
							ISet<string> allowedActivities = IdentityContext.Current.GetEffectiveResourceTypePermissions(
								organization.Id,
								ServiceType.System,
								intent,
								resourceType
							);

							if (allowedActivities.Any())
							{
								TestContext.WriteLine(
									"User '{0}' has permission to perform activities '{1}' associated with intent '{2}' for resource type '{3}' under organization '{4}'.",
									user.DisplayName,
									String.Join(", ", allowedActivities),
									intent,
									resourceType,
									organization.Name
								);
							}
							else
							{
								TestContext.WriteLine(
									"User '{0}' has no right to perform any activities associated with intent '{1}' for resource type '{2}' under organization '{3}'.",
									user.DisplayName,
									intent,
									resourceType,
									organization.Name
								);
							}
							TestContext.WriteLine(String.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		///		Test Identity effective Resource Type Permission for Location resource type.
		/// </summary>
		[TestMethod]
		public void EffectiveResourceTypePermissionTest_Location()
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

				const string intent = ApertureAccessControl.Intent.ManageResources;
				const string resourceType = Data.ResourceTypeNames.SystemLocation;

				foreach (User user in usersToCheck)
				{
					using (new IdentityContextScope(Helpers.GetUserPrincipal(user), container))
					{
						Assert.IsNotNull(IdentityContext.Current);
						foreach (Organization organization in organizationsToCheck)
						{
							ISet<string> allowedActivities = IdentityContext.Current.GetEffectiveResourceTypePermissions(
								organization.Id,
								ServiceType.System,
								intent,
								resourceType
							);

							if (allowedActivities.Any())
							{
								TestContext.WriteLine(
									"User '{0}' has permission to perform activities '{1}' associated with intent '{2}' for resource type '{3}' under organization '{4}'.",
									user.DisplayName,
									String.Join(", ", allowedActivities),
									intent,
									resourceType,
									organization.Name
								);
							}
							else
							{
								TestContext.WriteLine(
									"User '{0}' has no right to perform any activities associated with intent '{1}' for resource type '{2}' under organization '{3}'.",
									user.DisplayName,
									intent,
									resourceType,
									organization.Name
								);
							}
							TestContext.WriteLine(String.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		///		Test Identity Resource Permissions.
		/// </summary>
		/// <returns>
		///		A <see cref="Task"/> representing the asynchronous test execution.
		/// </returns>
		[TestMethod]
		[DeploymentItem(@"TestData", @"IdentityExamples")]
		public async Task ImportSecurityTemplateTest()
		{
			using (IContainer container = BuildContainer())
			{
				String templateFile = Path.Combine(
					TestContext.DeploymentDirectory,
					"IdentityExamples",
					"SecurityTemplate.xml"
				);

				ISecurityTemplateImporter stImporter = container.Resolve<ISecurityTemplateImporter>();

				// Load the security template
				await stImporter.LoadSecurityTemplateAsync(templateFile);
				Assert.IsTrue(stImporter.IsSecurityTemplateLoaded);

				// Validate the security template against the organization we want to import the template to.
				Guid targetOrganizationId = Data.Organizations.Cloud.Id;
				IReadOnlyList<string> validationErrors = await stImporter.ValidateSecurityTemplateAsync(targetOrganizationId, ServiceTypes.All);
				validationErrors
					.ForEach(
						error =>
							TestContext.WriteLine(error)
					);
				Assert.IsTrue(stImporter.IsSecurityTemplateValidated);

				// Import the security template.
				await stImporter.ImportSecurityTemplateAsync(targetOrganizationId, ServiceTypes.All);
			}
		}
	}
}
