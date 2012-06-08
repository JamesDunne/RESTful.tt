using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WellDunne.REST.Google.GData.V1_4
{
    public sealed class OAuth10GData14 : OAuth10
    {
        public OAuth10GData14(string consumerKey, string consumerSecret)
            : base(consumerKey, consumerSecret)
        {
        }

        public override void Authenticate(System.Net.HttpWebRequest request, byte[] body)
        {
            base.Authenticate(request, body);
            request.Headers.Add(@"GData-Version: 1.4");
        }
    }
}
