using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WellDunne.REST.Google
{
    public class OAuth10 : IRequestAuthentication
    {
        private readonly string consumerKey;
        private readonly string consumerSecret;

        public OAuth10(string consumerKey, string consumerSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
        }

        public virtual void Authenticate(HttpWebRequest request, byte[] body)
        {
            string oauthHeader = OAuthUtilities.GenerateHeaderForGoogle(request.RequestUri, consumerKey, consumerSecret, request.Method);
            request.Headers.Add("Authorization", oauthHeader);
        }
    }
}
