using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RESTful.Authentication
{
    public sealed class OAuthThreeLeggedAuthentication : IRequestAuthentication
    {
        private readonly string token;

        public OAuthThreeLeggedAuthentication(string token)
        {
            this.token = token;
        }

        public void Authenticate(HttpWebRequest request)
        {
            request.Headers.Add("Authorization", "OAuth " + token);
        }
    }
}
