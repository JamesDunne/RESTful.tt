using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WellDunne.REST.Pearson.LearningStudio
{
    public static class LearningStudioClients
    {
        private static readonly Uri baseUri = new Uri("https://m-api.ecollege.com");

        public static CourseServiceClient GetCourseClient(string userName)
        {
            var token = AccessTokens.Get(userName);
            if (token == OAuth20AccessToken.Invalid)
                throw new InvalidOAuth20AccessTokenException("eCollege API", userName);
            var auth = new OAuth20(token);
            var client = new CourseServiceClient(baseUri, auth);
            return client;
        }

        public static DocSharingServiceClient GetDocSharingClient(string userName)
        {
            var token = AccessTokens.Get(userName);
            if (token == OAuth20AccessToken.Invalid)
                throw new InvalidOAuth20AccessTokenException("eCollege API", userName);
            var auth = new OAuth20(token);
            var client = new DocSharingServiceClient(baseUri, auth);
            return client;
        }
    }
}
