using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WellDunne.REST;
using WellDunne.REST.Google;
using WellDunne.REST.Google.V3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Google.Apis.Calendar.v3.Data;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the authentication module to use oauth 1.0 2LO:
            // NOTE: Replace 'key' and 'secret' with your testing parameters, else you'll get a 401 (Unauthorized) response.
            var oauth = new OAuth10("myclientidhere.apps.googleusercontent.com", "mysecretkeyhere");

            // Create the client against googleapis.com:
            // NOTE: This client instance is reusable across threads.
            var client = new CalendarServiceAsyncClient(new Uri("https://www.googleapis.com"), oauth);

            // Create the request to get the user's calendar list:
            // NOTE: Each request object is NOT reusable.
            var req = client.GetMyCalendarList(null, null, null, /*requestorID:*/ "username@example.com");

            // Fetch the request synchronously:
            var rsp = req.Fetch<CalendarList>();

            // Write the response JSON object to Console.Out:
            using (var conWriter = new JsonTextWriter(Console.Out))
                JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented })
                    .Serialize(conWriter, rsp.Response);

            Console.WriteLine();
        }
    }
}
