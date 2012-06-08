using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WellDunne.REST
{
    public struct OAuth20AccessToken
    {
        private readonly string _token;

        public OAuth20AccessToken(string token)
        {
            _token = token;
        }

        public static implicit operator string(OAuth20AccessToken token)
        {
            return token._token;
        }

        public static implicit operator OAuth20AccessToken(string token)
        {
            return new OAuth20AccessToken(token);
        }

        public static readonly OAuth20AccessToken Invalid = new OAuth20AccessToken();
    }
}
