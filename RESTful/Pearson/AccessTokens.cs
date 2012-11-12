using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace WellDunne.REST.Pearson
{
    public static class AccessTokens
    {
        // NOTE: Fill these out with actual values provided by Pearson.
        private static string publicKey { get { return "eCollege.consumerKey"; } }
        private static string privateKey { get { return "eCollege.consumerSecret"; } }
        private static string applicationId { get { return ("eCollege.application_id"); } }
        private static string clientName { get { return ("eCollege.client_name"); } }
        private static string clientString { get { return ("eCollege.client_string"); } }
        private static Uri tokenRequestUri { get { return new Uri("https://m-api.ecollege.com/token"); } }

        private static readonly object _mutex = new object();
        private static OAuth20AssertionBuilder _assertionBuilder = null;
        private static OAuth20AssertionBuilder assertionBuilder
        {
            get
            {
                lock (_mutex)
                {
                    if (_assertionBuilder == null)
                        Interlocked.Exchange(ref _assertionBuilder, new OAuth20AssertionBuilder(publicKey, privateKey, applicationId, clientName, clientString));
                    return _assertionBuilder;
                }
            }
        }

        /// <summary>
        /// Gets an OAuth 2.0 access token for the given <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static OAuth20AccessToken Get(string userName)
        {
            string tokenValue;

            // Build an assertion for the username:
            var assertion = assertionBuilder.ConstructAssertion(userName);

            // Request an access token:
            var tokenClient = new OAuth20AssertionClient();
            var tokenRequest = tokenClient.RequestAccessToken(tokenRequestUri, "assertion", "urn:ecollege:names:moauth:1.0:assertion", assertion);

            try
            {
                var tokenResponse = tokenRequest.Fetch<JObject>().Response;
                if (tokenResponse == null) return null;

                JToken accessToken = tokenResponse["access_token"];
                if (accessToken == null)
                {
                    // TODO(jsd): Log an exception with the "error" object from the response.
                    //Trace.WriteLine();
                    return null;
                }

                tokenValue = accessToken.Value<string>();

                // NOTE(jsd): The token response comes with an expiration timeout value; we should use this for the cache.
                // According to eCollege it is always 1 hour.

                // int expiresTimeout = tokenResponse["expires"].Value<int>();
            }
            catch (Exception ex)
            {
                // TODO: log exception
                Trace.WriteLine(ex.ToString());
                return null;
            }

            if (tokenValue == null) return OAuth20AccessToken.Invalid;

            return (OAuth20AccessToken)tokenValue;
        }
    }
}
