using System;
using System.Security.Claims;

namespace DD.Cloud.Aperture.Identity.Example
{
	using Claims;
	using Common;
	using Contracts;

	/// <summary>
	///		Helper methods.
	/// </summary>
	public static class Helpers
	{
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
			if (user.OrganizationId == Data.Organizations.Cloud.Id)
				matchingOrganization = Data.Organizations.Cloud;
			else if (user.OrganizationId == Data.Organizations.Australia.Id)
				matchingOrganization = Data.Organizations.Australia;

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
