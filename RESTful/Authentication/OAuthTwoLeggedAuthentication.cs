using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RESTful.Authentication
{
    public sealed class OAuthTwoLeggedAuthentication : IRequestAuthentication
    {
        private readonly string consumerKey;
        private readonly string consumerSecret;

        public OAuthTwoLeggedAuthentication(string consumerKey, string consumerSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
        }

        public void Authenticate(HttpWebRequest request)
        {
            string oauthHeader = OAuthUtilities.GenerateHeader(request.RequestUri, consumerKey, consumerSecret, request.Method);
            request.Headers.Add("Authorization", oauthHeader);
        }
    }
}
