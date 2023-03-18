using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using WebApiOauth2.Models;
using Microsoft.Owin.Security.OAuth;
using WebApiOauth2.Helper_Code.OAuth2;
using WebApiOauth2.Providers;

namespace WebApiOauth2
{
    public partial class Startup
    {
        #region Public /Protected Properties.

        /// <summary>
        /// OAUTH options property.
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        /// <summary>
        /// Public client ID property.
        /// </summary>
        public static string PublicClientId { get; private set; }

        #endregion

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth/Token"),
                Provider = new AppOAuthProvider(PublicClientId),
                //AuthorizeEndpointPath = new PathString("/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(10),
                AllowInsecureHttp = true, //Don't do this in production ONLY FOR DEVELOPING: ALLOW INSECURE HTTP!
                RefreshTokenProvider = new OAuthCustomRefreshTokenProvider()
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
    }
}