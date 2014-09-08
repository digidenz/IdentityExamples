using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DD.Cloud.Aperture.Identity.Example
{
	using Contracts;

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
				.DirectToDatabase(Helpers.AuditingConnectionString);

			containerBuilder
				.ConfigureIdentity()
				.WithAuthUsingDatabase(Helpers.IdentityConnectionString)
				.WithManagementApiUsingDatabase(Helpers.IdentityConnectionString);

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
				User currentUser = Helpers.Users.JohnDoe;

				using (new IdentityContextScope(Helpers.GetUserPrincipal(currentUser), container))
				{
					Assert.IsNotNull(IdentityContext.Current);
					Assert.AreEqual(currentUser.Id, IdentityContext.Current.PrincipalId);
				}
			}
		}
	}
}
