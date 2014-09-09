using Autofac;
using Dapper;
using System;
using System.Data;

namespace DD.Cloud.Aperture.Identity.Example.PopulateDatabase
{
	using System.Transactions;
	using Common;
	using Contracts;
	using Data;
	using Management.Sql;
	using Platform.Contracts;
	using Platform.Core.DataAccess;
	using InheritanceFlags = Contracts.AccessControl.InheritanceFlags;

	/// <summary>
	///		The populate database tool.
	/// </summary>
	class Program
	{
		/// <summary>
		///		Program execution entry point.
		/// </summary>
		/// <param name="args">
		///		Command line arguments.
		/// </param>
		static void Main(string[] args)
		{
			using (TransactionScope transaction = TransactionScopeHelpers.RequiredWithReadCommitted())
			{
				CreateEntities();
				CreateMemberships();
				CreateAces();

				transaction.Complete();
			}
		}

		/// <summary>
		///		Build the Autofac container.
		/// </summary>
		/// <returns>
		///		The Autofac container.
		/// </returns>
		private static IContainer BuildContainer()
		{
			ContainerBuilder containerBuilder = new ContainerBuilder();

			containerBuilder
				.RegisterModule(
					new IdentityManagementSqlModule(Common.Constants.IdentityConnectionString)
				);

			return containerBuilder.Build();
		}

		/// <summary>
		///		Populate the Identity database with our example entity data.
		/// </summary>
		private static void CreateEntities()
		{
			using (IContainer container = BuildContainer())
			{
				// Create the organizatons.
				IOrganizationRepository organizationRepository = container.Resolve<IOrganizationRepository>();
				Organization[] organizationsToCreate =
				{
					Data.Organizations.Australia, 
					Data.Organizations.Usa, 
					Data.Organizations.Optus, 
					Data.Organizations.Vodafone
				};
				foreach (Organization organization in organizationsToCreate)
				{
					organizationRepository.Create(
						organization.Name, 
						organization.ParentOrganizationId.GetValueOrDefault(), 
						organization.PrimaryDomainName, 
						organization.Id
					);
				}

				// Create the users.
				IUserRepository userRepository = container.Resolve<IUserRepository>();
				User[] usersToCreate =
				{
					Data.Users.JohnDoe, 
					Data.Users.MattSmith, 
					Data.Users.AlexWoods, 
					Data.Users.JennySmith
				};
				foreach (User user in usersToCreate)
				{
					userRepository.Create(
						user.FirstName, 
						user.LastName, 
						user.DisplayName, 
						user.UserPrincipalName, 
						user.OrganizationId, 
						user.Id
					);
				}

				// Create the groups.
				IGroupRepository groupRepository = container.Resolve<IGroupRepository>();
				Group[] groupsToCreate =
				{
					Data.Groups.CloudAdmins, 
					Data.Groups.AustraliaAdmins, 
					Data.Groups.AustraliaStandard, 
					Data.Groups.OptusAdmins
				};
				foreach (Group group in groupsToCreate)
				{
					groupRepository.Create(
						group.Name, 
						group.Description, 
						group.DisplayName, 
						group.OrganizationId, 
						group.Id
					);
				}

				// Create the resource types;
				IDbConnection identityConnection = container.ResolveNamed<IDbConnection>(Management.Sql.Repository.RepositoryHelpers.ConnectionComponentName);
				const string command = @"
					Insert Into
						[Identity].[ResourceType]
						(				
							[ServiceTypeId],
							[Name],
							[Description]
						)
						Values
						(
							1,
							'Department',
							'Platform department resource'
						),
						(
							1,
							'Location',
							'Platform location resource'
						);
				";

				identityConnection
					.Execute(
						command,
						commandType: CommandType.Text
					);

				// Create the resources.
				IResourceRepository resourceRepository = container.Resolve<IResourceRepository>();
				Resource[] resourcesToCreate =
				{
					Data.PlatformResources.DepartmentHR, 
					Data.PlatformResources.DepartmentRD, 
					Data.PlatformResources.LocationNorthRyde, 
					Data.PlatformResources.LocationTheRocks
				};
				foreach (Resource resource in resourcesToCreate)
				{
					resourceRepository.Create(
						resource.Name, 
						resource.ServiceType, 
						resource.ResourceTypeName, 
						resource.OwnerId, 
						resource.Id
					);
				}
			}
		}
		
		/// <summary>
		///		Populate the Identity database with group membership data.
		/// </summary>
		static void CreateMemberships()
		{
			using (IContainer container = BuildContainer())
			{
				IGroupRepository groupRepository = container.Resolve<IGroupRepository>();
				IGroupMemberRepository groupMemberRepository = container.Resolve<IGroupMemberRepository>();

				// Cloud Admins
				Group cloudAdmin = groupRepository.GetById(Data.Groups.CloudAdmins.Id);
				groupMemberRepository.AddUserToGroup(
					cloudAdmin.Id, 
					Data.Users.JohnDoe.Id, 
					cloudAdmin.UpdateToken
				);

				// Australia Admins
				Group australiaAdmin = groupRepository.GetById(Data.Groups.AustraliaAdmins.Id);
				Guid updateToken = groupMemberRepository.AddGroupToGroup(
					australiaAdmin.Id, 
					Data.Groups.CloudAdmins.Id, 
					australiaAdmin.UpdateToken
				);
				updateToken = groupMemberRepository.AddUserToGroup(
					australiaAdmin.Id, 
					Data.Users.MattSmith.Id, 
					updateToken
				);
				groupMemberRepository.AddUserToGroup(
					australiaAdmin.Id, 
					Data.Users.AlexWoods.Id, 
					updateToken
				);

				// Australia Standard
				Group australiaStandard = groupRepository.GetById(Data.Groups.AustraliaStandard.Id);
				updateToken = groupMemberRepository.AddGroupToGroup(
					australiaStandard.Id, 
					Data.Groups.AustraliaAdmins.Id, 
					australiaStandard.UpdateToken
				);
				groupMemberRepository.AddUserToGroup(
					australiaStandard.Id, 
					Data.Users.JennySmith.Id, 
					updateToken
				);

				// Optus Admin
				Group optusAdmin = groupRepository.GetById(Data.Groups.OptusAdmins.Id);
				groupMemberRepository.AddGroupToGroup(
					optusAdmin.Id, 
					Data.Groups.AustraliaAdmins.Id, 
					optusAdmin.UpdateToken
				);
			}
		}

		/// <summary>
		///		Populate the Identity database with access control entries.
		/// </summary>
		static void CreateAces()
		{
			using (IContainer container = BuildContainer())
			{
				IAccessControlRepository accessControlRepository = container.Resolve<IAccessControlRepository>();
				IOrganizationRepository organizationRepository = container.Resolve<IOrganizationRepository>();
				IResourceRepository resourceRepository = container.Resolve<IResourceRepository>();
				
				// Cloud Admins
				Organization cloud = organizationRepository.GetById(Data.Organizations.Cloud.Id);
				accessControlRepository.CreateRight(
					cloud.Id, 
					Data.Groups.CloudAdmins.Id, 
					ServiceType.System, 
					ApertureAccessControl.Activity.RequestSecurityToken,
					InheritanceFlags.Organization | InheritanceFlags.Children,
					cloud.UpdateToken
				);

				// Australia Admins
				Organization australia = organizationRepository.GetById(Data.Organizations.Australia.Id);
				Guid updateToken = accessControlRepository.CreateResourceTypePermission(
					australia.Id, 
					Data.Groups.AustraliaAdmins.Id, 
					ServiceType.System, 
					ApertureAccessControl.Activity.ReadResource, 
					Data.ResourceTypeNames.SystemDepartment, 
					InheritanceFlags.Children,
					australia.UpdateToken
				);
				Resource locationNorthRyde = resourceRepository.GetById(Data.PlatformResources.LocationNorthRyde.Id);
				accessControlRepository.CreateResourcePermission(
					locationNorthRyde.Id, 
					Data.Groups.AustraliaAdmins.Id, 
					ServiceType.System, 
					ApertureAccessControl.Activity.ReadResource,
					locationNorthRyde.UpdateToken
				);

				// Australia Standard
				updateToken = accessControlRepository.CreateResourceTypePermission(
					australia.Id,
					Data.Groups.AustraliaStandard.Id,
					ServiceType.System,
					ApertureAccessControl.Activity.UpdateResource,
					Data.ResourceTypeNames.SystemDepartment,
					InheritanceFlags.Children,
					updateToken
				);

				// Resource Owner Rights
				accessControlRepository.CreateResourceOwnerRight(
					australia.Id,
					ServiceType.System, 
					ApertureAccessControl.Activity.ReadResource, 
					Data.ResourceTypeNames.SystemLocation, 
					InheritanceFlags.Organization | InheritanceFlags.Children,
					updateToken
				);
			}
		}
	}
}
