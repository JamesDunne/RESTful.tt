using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WellDunne.REST
{
    public sealed class NullRequestAuthentication : IRequestAuthentication
    {
        public void Authenticate(System.Net.HttpWebRequest request, byte[] body)
        {
            // Do nothing.
        }

        public static readonly NullRequestAuthentication Default = new NullRequestAuthentication();
    }
}
