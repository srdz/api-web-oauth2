using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebApiOauth2.Providers
{
    public class OAuthCustomRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        #region Create Async

        /// <summary>
        /// Create Async
        /// </summary>
        /// <param name="context">Context parameter</param>
        /// <returns></returns>

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();

            // Set lifetime of refresh token
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(40)
            };

            var refreshTokenTicket = await Task.Run(() => new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties));

            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle  
            context.SetToken(guid);
        }

        #endregion

        #region Receive Async

        /// <summary>
        /// Receive Async
        /// </summary>
        /// <param name="context">Context parameter</param>
        /// <returns></returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            string header = await Task.Run(() => context.OwinContext.Request.Headers["Authorization"]);

            if (_refreshTokens.TryRemove(context.Token, out ticket))
                context.SetTicket(ticket);
        }

        #endregion

        #region Create

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="context">Context parameter</param>
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Receive

        /// <summary>
        /// Receive
        /// </summary>
        /// <param name="context">Context parameter</param>
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}