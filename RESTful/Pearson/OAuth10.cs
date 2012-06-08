using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WellDunne.REST.Pearson
{
    public sealed class OAuth10 : IRequestAuthentication
    {
        private readonly string consumerKey;
        private readonly string consumerSecret;
        private readonly string applicationId;

        public OAuth10(string consumerKey, string consumerSecret, string applicationId)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.applicationId = applicationId;
        }

        public void Authenticate(HttpWebRequest request, byte[] body)
        {
            string oauthHeader = OAuthUtilities.GenerateHeaderForPearson(request.RequestUri, request.Method, body, applicationId, consumerKey, consumerSecret);
            request.Headers.Add("X-Authorization", oauthHeader);
        }
    }
}
