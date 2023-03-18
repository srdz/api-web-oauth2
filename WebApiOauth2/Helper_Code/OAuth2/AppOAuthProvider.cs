namespace WebApiOauth2.Helper_Code.OAuth2
{
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OAuth;
    using Models;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using WebApiOauth2.App_Start;

    /// <summary>
    /// Application OAUTH Provider class.
    /// </summary>
    public class AppOAuthProvider : OAuthAuthorizationServerProvider
    {
        public MongoDBContext dbcontext = new MongoDBContext();
        public IMongoCollection<LoginOAuthModel> loginCollection;

        #region Private Properties

        /// <summary>
        /// Public client ID property.
        /// </summary>
        private readonly string _publicClientId;

        /// <summary>
        /// Database Store property.
        /// </summary>
        //private Oauth_APIEntities databaseManager = new Oauth_APIEntities();

        #endregion

        #region Default Constructor method.

        /// <summary>
        /// Default Constructor method.
        /// </summary>
        /// <param name="publicClientId">Public client ID parameter</param>
        public AppOAuthProvider(string publicClientId)
        {
            //TODO: Pull from configuration
            if (publicClientId == null)
            {
                throw new ArgumentNullException(nameof(publicClientId));
            }

            // Settings.
            _publicClientId = publicClientId;
        }

        #endregion

        #region Grant resource owner credentials override method.

        /// <summary>
        /// Grant resource owner credentials overload method.
        /// </summary>
        /// <param name="context">Context parameter</param>
        /// <returns>Returns when task is completed</returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // Initialization.
            string usernameVal = context.UserName;
            string passwordVal = context.Password;

            //dbcontext = new MongoDBContext();
            loginCollection = dbcontext.database.GetCollection<LoginOAuthModel>("LoginOAuth");

            var user = loginCollection.AsQueryable<LoginOAuthModel>().Where(w => w.username.Equals(usernameVal) && w.password.Equals(passwordVal)).ToList();

            // Verification.
            if (user == null || user.Count() <= 0)
            {
                // Settings.
                context.SetError("invalid_grant", "Either username or password is incorrect");

                // Return info.
                return;
            }

            // Initialization.
            var claims = new List<Claim>();
            var userInfo = user.FirstOrDefault();

            // Setting
            claims.Add(new Claim(ClaimTypes.Name, userInfo.username));
            claims.Add(new Claim(ClaimTypes.Role, String.Join(", ", userInfo.role)));

            // Setting Claim Identities for OAUTH 2 protocol.
            ClaimsIdentity oAuthClaimIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);

            // Setting user authentication.
            AuthenticationProperties properties = CreateProperties(userInfo.username, String.Join(", ", userInfo.role));
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthClaimIdentity, properties);

            // Grant access to authorize user.
            context.Validated(ticket);
        }

        #endregion

        #region Token endpoint override method.

        /// <summary>
        /// Token endpoint override method
        /// </summary>
        /// <param name="context">Context parameter</param>
        /// <returns>Returns when task is completed</returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                // Adding.
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            // Return info.
            return Task.FromResult<object>(null);
        }

        #endregion

        #region Validate Client authentication override method

        /// <summary>
        /// Validate Client authentication override method
        /// </summary>
        /// <param name="context">Contect parameter</param>
        /// <returns>Returns validation of client authentication</returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                // Validate Authoorization.
                context.Validated();
            }

            // Return info.
            return Task.FromResult<object>(null);
        }

        #endregion

        #region Validate client redirect URI override method

        /// <summary>
        /// Validate client redirect URI override method
        /// </summary>
        /// <param name="context">Context parmeter</param>
        /// <returns>Returns validation of client redirect URI</returns>
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            // Verification.
            if (context.ClientId == _publicClientId)
            {
                // Initialization.
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                // Verification.
                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    // Validating.
                    context.Validated();
                }
            }

            // Return info.
            return Task.FromResult<object>(null);
        }

        #endregion

        #region Create Authentication properties method.

        /// <summary>
        /// Create Authentication properties method.
        /// </summary>
        /// <param name="userName">User name parameter</param>
        /// <returns>Returns authenticated properties.</returns>
        public static AuthenticationProperties CreateProperties(string userName, string role)
        {
            // Settings.
            IDictionary<string, string> data = new Dictionary<string, string>
                                               {
                                                   { "userName", userName },
                                                   { "role", role }
                                               };

            // Return info.
            return new AuthenticationProperties(data);
        }

        #endregion

        #region Grant Refresh Token

        /// <summary>
        /// Refresh the Token
        /// </summary>
        /// <param name="context">Context parmeter</param>
        /// <returns></returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        #endregion
    }
}