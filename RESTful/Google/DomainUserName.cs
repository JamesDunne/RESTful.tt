using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WellDunne.REST.Google
{
    /// <summary>
    /// Represents a user, domain pair for a username.
    /// </summary>
    [DebuggerDisplay(@"{Full}")]
    public struct DomainUserName
    {
        private string full;

        private void setUserDomain(string user, string domain)
        {
            User = user.ToLowerInvariant();
            Domain = domain.ToLowerInvariant();
            full = String.Concat(User, "@", Domain);
        }

        /// <summary>
        /// Creates a DomainUserName struct given both a user name and a domain name.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        public DomainUserName(string user, string domain)
            : this()
        {
            if (String.IsNullOrEmpty(user)) throw new ArgumentNullException("Parameter must be non-empty", "user");
            if (String.IsNullOrEmpty(domain)) throw new ArgumentNullException("Parameter must be non-empty", "domain");

            setUserDomain(user, domain);
        }

        /// <summary>
        /// Creates a DomainUserName struct given a `user@domain` formatted name.
        /// </summary>
        /// <param name="userAtDomain"></param>
        public DomainUserName(string userAtDomain)
            : this()
        {
            if (String.IsNullOrEmpty(userAtDomain)) throw new ArgumentNullException("Parameter must be non-empty", "userAtDomain");

            string[] split = userAtDomain.Split('@');
            if (split.Length != 2) throw new ArgumentException("Parameter must be of the format `user@domain`", "userAtDomain");

            setUserDomain(split[0].ToLowerInvariant(), split[1].ToLowerInvariant());
        }

        /// <summary>
        /// Gets the username part.
        /// </summary>
        public string User { get; private set; }

        /// <summary>
        /// Gets the domain name part.
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Gets the full `user@domain` format.
        /// </summary>
        public string Full { get { return full; } }

        /// <summary>
        /// Changes the user name without affecting the domain name.
        /// </summary>
        /// <param name="user">New user name to set</param>
        /// <returns></returns>
        public DomainUserName SetUser(string user)
        {
            User = user;
            full = String.Concat(User, "@", Domain);
            return this;
        }

        /// <summary>
        /// Changes the domain name without affecting the user name.
        /// </summary>
        /// <param name="domain">New domain name to set</param>
        /// <returns></returns>
        public DomainUserName SetDomain(string domain)
        {
            Domain = domain;
            full = String.Concat(User, "@", Domain);
            return this;
        }
    }
}
