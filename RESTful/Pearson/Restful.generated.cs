using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace WellDunne.REST.Pearson.LearningStudio
{
    public sealed partial class CourseServiceClient
    {
        private Uri baseUri;
        private IRequestAuthentication authentication;

        public CourseServiceClient(Uri baseUri, IRequestAuthentication authentication)
        {
            this.baseUri = baseUri;
            this.authentication = authentication;
        }

        #region Private helpers

        private static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string EncodeRFC3986(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        private Uri constructRequestUri(string route, List<KeyValuePair<string, string>> queryValues)
        {
            StringBuilder qb = new StringBuilder(queryValues.Sum(x => x.Key.Length + x.Value.Length + 2));

            bool first = true;
            foreach (var qp in queryValues)
            {
                qb.AppendFormat("{2}{0}={1}", EncodeRFC3986(qp.Key), EncodeRFC3986(qp.Value), first ? "" : "&");
                first = false;
            }

            var ub = new UriBuilder(baseUri);
            ub.Path = route;
            ub.Query = qb.ToString();
            return ub.Uri;
        }

        private Uri constructRequestUri(string route)
        {
            var ub = new UriBuilder(baseUri);
            ub.Path = route;
            return ub.Uri;
        }

        #endregion

        #region RESTful type system

        private bool _optional_commaDelimitedString_isSupplied(string[] value)
        {
            return value != null;
        }

        private string _optional_commaDelimitedString_encodeString(string[] value)
        {
            return value.ToCommaDelimitedString();
        }

        private bool _required_int_isSupplied(int value)
        {
            return true;
        }

        private string _required_int_encodeString(int value)
        {
            return value.ToString();
        }

        private bool _required_string_isSupplied(string value)
        {
            return value != null;
        }

        private string _required_string_encodeString(string value)
        {
            return value;
        }

        #endregion

        #region Instance-level query parameters

        #endregion

        #region API Methods

        public RestfulServiceRequest GetMyCourses([Optional] string[] expand)
        {
            var _queryValues = new List<KeyValuePair<string, string>>(1);
            if (_optional_commaDelimitedString_isSupplied(expand))
                _queryValues.Add(new KeyValuePair<string, string>("expand", _optional_commaDelimitedString_encodeString(expand)));

            var requestUri = constructRequestUri("/me/courses", _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";

            return new RestfulServiceRequest(authentication, req, null, RestfulServiceRequest.DeserializeJSON);
        }

        public RestfulServiceRequest GetCourseByID(int courseId, [Optional] string[] expand)
        {
            string r_courseId;
            if (!_required_int_isSupplied(courseId)) throw new ArgumentNullException("courseId");
            else r_courseId = _required_int_encodeString(courseId);

            var _queryValues = new List<KeyValuePair<string, string>>(1);
            if (_optional_commaDelimitedString_isSupplied(expand))
                _queryValues.Add(new KeyValuePair<string, string>("expand", _optional_commaDelimitedString_encodeString(expand)));

            var requestUri = constructRequestUri("/courses/" + EncodeRFC3986(r_courseId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";

            return new RestfulServiceRequest(authentication, req, null, RestfulServiceRequest.DeserializeJSON);
        }

        public RestfulServiceRequest GetCourseByCallNumber(string courseCallNumber, [Optional] string[] expand)
        {
            string r_courseCallNumber;
            if (!_required_string_isSupplied(courseCallNumber)) throw new ArgumentNullException("courseCallNumber");
            else r_courseCallNumber = _required_string_encodeString(courseCallNumber);

            var _queryValues = new List<KeyValuePair<string, string>>(1);
            if (_optional_commaDelimitedString_isSupplied(expand))
                _queryValues.Add(new KeyValuePair<string, string>("expand", _optional_commaDelimitedString_encodeString(expand)));

            var requestUri = constructRequestUri("/courses/ccn=" + EncodeRFC3986(r_courseCallNumber), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";

            return new RestfulServiceRequest(authentication, req, null, RestfulServiceRequest.DeserializeJSON);
        }

        #endregion
    }
}

namespace WellDunne.REST.Pearson.LearningStudio
{
    public sealed partial class DocSharingServiceClient
    {
        private Uri baseUri;
        private IRequestAuthentication authentication;

        public DocSharingServiceClient(Uri baseUri, IRequestAuthentication authentication)
        {
            this.baseUri = baseUri;
            this.authentication = authentication;
        }

        #region Private helpers

        private static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string EncodeRFC3986(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        private Uri constructRequestUri(string route, List<KeyValuePair<string, string>> queryValues)
        {
            StringBuilder qb = new StringBuilder(queryValues.Sum(x => x.Key.Length + x.Value.Length + 2));

            bool first = true;
            foreach (var qp in queryValues)
            {
                qb.AppendFormat("{2}{0}={1}", EncodeRFC3986(qp.Key), EncodeRFC3986(qp.Value), first ? "" : "&");
                first = false;
            }

            var ub = new UriBuilder(baseUri);
            ub.Path = route;
            ub.Query = qb.ToString();
            return ub.Uri;
        }

        private Uri constructRequestUri(string route)
        {
            var ub = new UriBuilder(baseUri);
            ub.Path = route;
            return ub.Uri;
        }

        #endregion

        #region RESTful type system

        private bool _required_int_isSupplied(int value)
        {
            return true;
        }

        private string _required_int_encodeString(int value)
        {
            return value.ToString();
        }

        #endregion

        #region Instance-level query parameters

        #endregion

        #region API Methods

        public RestfulServiceRequest GetCategoriesForCourse(int courseId)
        {
            string r_courseId;
            if (!_required_int_isSupplied(courseId)) throw new ArgumentNullException("courseId");
            else r_courseId = _required_int_encodeString(courseId);

            var requestUri = constructRequestUri("/courses/" + EncodeRFC3986(r_courseId) + "/docSharingCategories");
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";

            return new RestfulServiceRequest(authentication, req, null, RestfulServiceRequest.DeserializeJSON);
        }

        public RestfulServiceRequest GetDocumentsInCategoryForCourse(int courseId, int categoryId)
        {
            string r_courseId;
            if (!_required_int_isSupplied(courseId)) throw new ArgumentNullException("courseId");
            else r_courseId = _required_int_encodeString(courseId);
            string r_categoryId;
            if (!_required_int_isSupplied(categoryId)) throw new ArgumentNullException("categoryId");
            else r_categoryId = _required_int_encodeString(categoryId);

            var requestUri = constructRequestUri("/courses/" + EncodeRFC3986(r_courseId) + "/docSharingCategories/" + EncodeRFC3986(r_categoryId) + "/docSharingDocuments");
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";

            return new RestfulServiceRequest(authentication, req, null, RestfulServiceRequest.DeserializeJSON);
        }

        public RestfulServiceRequest DownloadDocument(int courseId, int categoryId, int documentId)
        {
            string r_courseId;
            if (!_required_int_isSupplied(courseId)) throw new ArgumentNullException("courseId");
            else r_courseId = _required_int_encodeString(courseId);
            string r_categoryId;
            if (!_required_int_isSupplied(categoryId)) throw new ArgumentNullException("categoryId");
            else r_categoryId = _required_int_encodeString(categoryId);
            string r_documentId;
            if (!_required_int_isSupplied(documentId)) throw new ArgumentNullException("documentId");
            else r_documentId = _required_int_encodeString(documentId);

            var requestUri = constructRequestUri("/courses/" + EncodeRFC3986(r_courseId) + "/docSharingCategories/" + EncodeRFC3986(r_categoryId) + "/docSharingDocuments/" + EncodeRFC3986(r_documentId) + "/content");
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";

            return new RestfulServiceRequest(authentication, req, null, RestfulServiceRequest.DeserializeJSON);
        }

        #endregion
    }
}
