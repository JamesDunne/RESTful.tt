using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace WellDunne.REST
{
    public class OAuth20AssertionClient
    {
        public RestfulServiceRequest RequestAccessToken(Uri tokenRequestUrl, string grantType, string assertionType, OAuth20ConstructedAssertion assertion)
        {
            var req = (HttpWebRequest)HttpWebRequest.Create(tokenRequestUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            // Create the data to send:
            StringBuilder data = new StringBuilder();
            data.Append("grant_type=" + HttpUtility.UrlEncode(grantType));
            data.Append("&assertion_type=" + HttpUtility.UrlEncode(assertionType));
            data.Append("&assertion=" + HttpUtility.UrlEncode(assertion));

            // Create a byte array of the data to be sent
            byte[] byteArray = Encoding.UTF8.GetBytes(data.ToString());
            req.ContentLength = byteArray.Length;

            return new RestfulServiceRequest(NullRequestAuthentication.Default, req, (st) => st.Write(byteArray, 0, byteArray.Length), RestfulServiceRequest.DeserializeJSON);
        }
    }
}
