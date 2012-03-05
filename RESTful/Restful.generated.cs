﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using RESTful;
using RESTful.Authentication;

namespace Google.V3
{
    public sealed class CalendarServiceClient
    {
        private Uri baseUri;
        private IRequestAuthentication authentication;

        public CalendarServiceClient(Uri baseUri, IRequestAuthentication authentication)
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

        #endregion

        #region RESTful type system

        private bool _optional_commaDelimitedString_isSupplied(string[] value)
        {
            return value != null;
        }

        private string _optional_commaDelimitedString_encodeString(string[] value)
        {
            return String.Join(",", value);
        }

        private bool _optional_string_isSupplied(string value)
        {
            return value != null;
        }

        private string _optional_string_encodeString(string value)
        {
            return value;
        }

        private bool _optional_bool_isSupplied(bool? value)
        {
            return value.HasValue;
        }

        private string _optional_bool_encodeString(bool? value)
        {
            return value.ToString().ToLower();
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

        public RestfulServiceRequest GetMyCalendarList([Optional] string pageToken, [Optional] bool? showHidden, [Optional] string[] fields, [Optional] string requestorID)
        {
            var _queryValues = new List<KeyValuePair<string, string>>(4);
            if (_optional_string_isSupplied(pageToken))
                _queryValues.Add(new KeyValuePair<string, string>("pageToken", _optional_string_encodeString(pageToken)));
            if (_optional_bool_isSupplied(showHidden))
                _queryValues.Add(new KeyValuePair<string, string>("showHidden", _optional_bool_encodeString(showHidden)));
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            var requestUri = constructRequestUri("/calendar/v3/users/me/calendarList", _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req);
        }

        public RestfulServiceRequest GetMyCalendar(string calendarId, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);

            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            var requestUri = constructRequestUri("/calendar/v3/users/me/calendarList/" + EncodeRFC3986(r_calendarId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req);
        }

        public RestfulServiceRequest GetCalendarEvents(string calendarId, [Optional] string iCalUID, [Optional] bool? singleEvents, [Optional] bool? showDeleted, [Optional] string orderBy, [Optional] string pageToken, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);

            var _queryValues = new List<KeyValuePair<string, string>>(7);
            if (_optional_string_isSupplied(iCalUID))
                _queryValues.Add(new KeyValuePair<string, string>("iCalUID", _optional_string_encodeString(iCalUID)));
            if (_optional_bool_isSupplied(singleEvents))
                _queryValues.Add(new KeyValuePair<string, string>("singleEvents", _optional_bool_encodeString(singleEvents)));
            if (_optional_bool_isSupplied(showDeleted))
                _queryValues.Add(new KeyValuePair<string, string>("showDeleted", _optional_bool_encodeString(showDeleted)));
            if (_optional_string_isSupplied(orderBy))
                _queryValues.Add(new KeyValuePair<string, string>("orderBy", _optional_string_encodeString(orderBy)));
            if (_optional_string_isSupplied(pageToken))
                _queryValues.Add(new KeyValuePair<string, string>("pageToken", _optional_string_encodeString(pageToken)));
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId) + "/events", _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req);
        }

        public RestfulServiceRequest GetCalendarEvent(string calendarId, string eventId, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);
            string r_eventId;
            if (!_required_string_isSupplied(eventId)) throw new ArgumentNullException("eventId");
            else r_eventId = _required_string_encodeString(eventId);

            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId) + "/events/" + EncodeRFC3986(r_eventId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "GET";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req);
        }

        public RestfulServiceRequest CreateCalendar(object body, [Optional] string[] fields, [Optional] string requestorID)
        {
            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            if (body == null) throw new ArgumentNullException("body");

            var requestUri = constructRequestUri("/calendar/v3/calendars", _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "POST";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req, body);
        }

        public RestfulServiceRequest CreateCalendarList(object body, [Optional] string[] fields, [Optional] string requestorID)
        {
            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            if (body == null) throw new ArgumentNullException("body");

            var requestUri = constructRequestUri("/calendar/v3/users/me/calendarList", _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "POST";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req, body);
        }

        public RestfulServiceRequest CreateEvent(string calendarId, object body, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);

            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            if (body == null) throw new ArgumentNullException("body");

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId) + "/events", _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "POST";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req, body);
        }

        public RestfulServiceRequest ImportEvent(string calendarId, object body, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);

            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            if (body == null) throw new ArgumentNullException("body");

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId) + "/events/import", _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "POST";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req, body);
        }

        public RestfulServiceRequest UpdateCalendar(string calendarId, object body, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);

            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            if (body == null) throw new ArgumentNullException("body");

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "PUT";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req, body);
        }

        public RestfulServiceRequest UpdateEvent(string calendarId, string eventId, object body, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);
            string r_eventId;
            if (!_required_string_isSupplied(eventId)) throw new ArgumentNullException("eventId");
            else r_eventId = _required_string_encodeString(eventId);

            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            if (body == null) throw new ArgumentNullException("body");

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId) + "/events/" + EncodeRFC3986(r_eventId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "PUT";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req, body);
        }

        public RestfulServiceRequest DeleteCalendar(string calendarId, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);

            var _queryValues = new List<KeyValuePair<string, string>>(1);
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "DELETE";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req);
        }

        public RestfulServiceRequest DeleteEvent(string calendarId, string eventId, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);
            string r_eventId;
            if (!_required_string_isSupplied(eventId)) throw new ArgumentNullException("eventId");
            else r_eventId = _required_string_encodeString(eventId);

            var _queryValues = new List<KeyValuePair<string, string>>(1);
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            var requestUri = constructRequestUri("/calendar/v3/calendars/" + EncodeRFC3986(r_calendarId) + "/events/" + EncodeRFC3986(r_eventId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "DELETE";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req);
        }

        public RestfulServiceRequest DeleteMyCalendarList(string calendarId, [Optional] string[] fields, [Optional] string requestorID)
        {
            string r_calendarId;
            if (!_required_string_isSupplied(calendarId)) throw new ArgumentNullException("calendarId");
            else r_calendarId = _required_string_encodeString(calendarId);

            var _queryValues = new List<KeyValuePair<string, string>>(2);
            if (_optional_commaDelimitedString_isSupplied(fields))
                _queryValues.Add(new KeyValuePair<string, string>("fields", _optional_commaDelimitedString_encodeString(fields)));
            if (_optional_string_isSupplied(requestorID))
                _queryValues.Add(new KeyValuePair<string, string>("xoauth_requestor_id", _optional_string_encodeString(requestorID)));

            var requestUri = constructRequestUri("/calendar/v3/users/me/calendarList/" + EncodeRFC3986(r_calendarId), _queryValues);
            var req = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            req.Method = "DELETE";
            // TODO: accept gzip compressed responses.
            //req.Accept = "application/json";
            // TODO: no longer hard-code JSON.
            req.ContentType = "application/json";

            return new RestfulServiceRequest(authentication, req);
        }

        #endregion
    }
}