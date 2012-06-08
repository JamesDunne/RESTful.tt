using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WellDunne.REST.Pearson
{
    public sealed class OAuth20AssertionBuilder
    {
        private readonly string publicKey;
        private readonly string privateKey;
        private readonly string applicationId;
        private readonly string clientName;
        private readonly string clientString;

        public OAuth20AssertionBuilder(string publicKey, string privateKey, string applicationId, string clientName, string clientString)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
            this.applicationId = applicationId;
            this.clientName = clientName;
            this.clientString = clientString;
        }

        public OAuth20ConstructedAssertion ConstructAssertion(string userName)
        {
            // Get the UTC Date Timestamp
            string timestamp = DateTime.UtcNow.ToString("s") + "Z";

            // Setup the Assertion String
            string assertion = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", clientName, publicKey, applicationId, clientString, userName, timestamp);

            // Generate the CMAC used for Assertion Security
            string cmac = OAuthUtilities.GenerateCmacHEX(privateKey, assertion);

            // Add the CMAC to the Assertion String
            string assertionFinal = String.Format("{0}|{1}", assertion, cmac);

            return assertionFinal;
        }
    }
}
