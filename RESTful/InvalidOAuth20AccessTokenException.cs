using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WellDunne.REST
{
    public sealed class InvalidOAuth20AccessTokenException : Exception
    {
        public InvalidOAuth20AccessTokenException(string systemName, string userName)
            : base(String.Format("Could not retrieve OAuth 2.0 access token for {0} user '{1}'", systemName, userName))
        {
            UserName = userName;
        }

        public string UserName { get; private set; }
    }
}
