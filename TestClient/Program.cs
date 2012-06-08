using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WellDunne.REST;
using WellDunne.REST.Google;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the authentication module to use oauth 1.0 2LO:
            // NOTE: Replace 'key' and 'secret' with your testing parameters, else you'll get a 401 (Unauthorized) response.
            var oauth = new OAuth10("key", "secret");

            // Create the client against googleapis.com:
            // NOTE: This client instance is reusable across threads.
            var client = new WellDunne.REST.Google.V3.CalendarServiceAsyncClient(new Uri("https://www.googleapis.com"), oauth);

            // Create the request to get the user's calendar list:
            // NOTE: Each request object is NOT reusable.
            var req = client.GetMyCalendarList(null, null, null, /*requestorID:*/ "user@example.com");

            // Fetch the request synchronously:
            var rsp = req.Fetch<JObject>();

            // Write the response JSON object to Console.Out:
            using (var conWriter = new JsonTextWriter(Console.Out))
                rsp.Response.WriteTo(conWriter);
        }
    }
}
