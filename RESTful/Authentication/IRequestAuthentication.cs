using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RESTful.Authentication
{
    public interface IRequestAuthentication
    {
        void Authenticate(HttpWebRequest request);
    }
}
