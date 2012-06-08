using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WellDunne.REST
{
    public interface IRequestAuthentication
    {
        void Authenticate(HttpWebRequest request, byte[] body);
    }
}
