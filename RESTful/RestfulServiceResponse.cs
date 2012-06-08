using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WellDunne.REST
{
    public sealed class RestfulServiceResponse<TResponse>
    {
        public HttpStatusCode HttpStatusCode { get; private set; }
        public TResponse Response { get; private set; }

        public RestfulServiceResponse(HttpStatusCode httpStatusCode, TResponse response)
        {
            HttpStatusCode = httpStatusCode;
            Response = response;
        }
    }
}
