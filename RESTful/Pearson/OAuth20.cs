using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WellDunne.REST.Pearson
{
    public sealed class OAuth20 : IRequestAuthentication
    {
        private readonly OAuth20AccessToken accessToken;

        public OAuth20(OAuth20AccessToken accessToken)
        {
            this.accessToken = accessToken;
        }

        public void Authenticate(HttpWebRequest request, byte[] body)
        {
            request.Headers.Add("X-Authorization", String.Format("Access_Token access_token={0}", (string)accessToken));
        }
    }
}
