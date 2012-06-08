using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WellDunne.REST
{
    public struct OAuth20ConstructedAssertion
    {
        private readonly string _assertion;

        public OAuth20ConstructedAssertion(string assertion)
        {
            _assertion = assertion;
        }

        public static implicit operator string(OAuth20ConstructedAssertion assert)
        {
            return assert._assertion;
        }

        public static implicit operator OAuth20ConstructedAssertion(string assertion)
        {
            return new OAuth20ConstructedAssertion(assertion);
        }
    }
}
