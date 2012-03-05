using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using RESTful.Authentication;

namespace RESTful
{
    public sealed class RestfulServiceRequest
    {
        private IRequestAuthentication _auth;

        private HttpWebRequest _request;
        public HttpWebRequest Request { get { return _request; } }

        private object _body;
        public object Body { get { return _body; } }

        public RestfulServiceRequest(IRequestAuthentication authentication, HttpWebRequest httpWebRequest)
        {
            _auth = authentication;
            _request = httpWebRequest;
            _body = null;
        }

        public RestfulServiceRequest(IRequestAuthentication authentication, HttpWebRequest httpWebRequest, object body)
        {
            _auth = authentication;
            _request = httpWebRequest;
            _body = body;
        }

        private void Clear()
        {
            _auth = null;
            _request = null;
            _body = null;
        }

        private static readonly Encoding UTF8EncodingNoBOM = new UTF8Encoding(false);

        #region Asynchronous execution

        private sealed class AsyncExecutionState<TResponse>
        {
            public object body { get; private set; }
            public HttpWebRequest request { get; private set; }
            public Action<WebException> handleWebException { get; private set; }
            public Action<Exception> handleException { get; private set; }
            public Action completedWithError { get; private set; }
            public Action<TResponse> handleResponse { get; private set; }

            public AsyncExecutionState(object body, HttpWebRequest request, Action<WebException> handleWebException, Action<Exception> handleException, Action completedWithError, Action<TResponse> handleResponse)
            {
                this.body = body;
                this.request = request;
                this.handleWebException = handleWebException;
                this.handleException = handleException;
                this.completedWithError = completedWithError;
                this.handleResponse = handleResponse;
            }

            public void clear()
            {
                body = null;
                request = null;
                handleException = null;
                handleWebException = null;
                handleResponse = null;
                completedWithError = null;
            }
        }

        private static readonly Newtonsoft.Json.JsonSerializer _json =
            Newtonsoft.Json.JsonSerializer.Create(
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                }
            );

        private static void _completeGetResponse<TResponse>(IAsyncResult ar)
        {
            AsyncExecutionState<TResponse> r = (AsyncExecutionState<TResponse>)ar.AsyncState;

            HttpWebResponse rsp;
            try
            {
                rsp = (HttpWebResponse)r.request.EndGetResponse(ar);
            }
            catch (WebException wex)
            {
                r.handleWebException(wex);
                r.completedWithError();
                r.clear();
                r = null;
                return;
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError();
                r.clear();
                r = null;
                return;
            }

            try
            {
                using (var rs = rsp.GetResponseStream())
                using (var tr = new System.IO.StreamReader(rs, UTF8EncodingNoBOM))
                using (var jtr = new Newtonsoft.Json.JsonTextReader(tr))
                {
                    var result = _json.Deserialize<TResponse>(jtr);
                    r.handleResponse(result);
                }
            }
            catch (WebException wex)
            {
                r.handleWebException(wex);
                r.completedWithError();
                r.clear();
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError();
                r.clear();
            }
            finally
            {
                rsp.Close();
                r.clear();
            }
        }

        private static void _completeGetRequestStream<TResponse>(IAsyncResult ar)
        {
            AsyncExecutionState<TResponse> r = (AsyncExecutionState<TResponse>)ar.AsyncState;

            System.IO.Stream reqstr;
            try
            {
                reqstr = r.request.EndGetRequestStream(ar);
            }
            catch (WebException wex)
            {
                r.handleWebException(wex);
                r.completedWithError();
                r.clear();
                r = null;
                reqstr = null;
                return;
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError();
                r.clear();
                r = null;
                reqstr = null;
                return;
            }

            try
            {
                using (reqstr)
                using (var tw = new System.IO.StreamWriter(reqstr, UTF8EncodingNoBOM))
                using (var jtw = new Newtonsoft.Json.JsonTextWriter(tw))
                {
                    _json.Serialize(jtw, r.body);
                }
            }
            catch (WebException wex)
            {
                r.handleWebException(wex);
                r.completedWithError();
                r.clear();
                r = null;
                reqstr = null;
                return;
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError();
                r.clear();
                r = null;
                reqstr = null;
                return;
            }

            // Start an async operation to get the response:
            r.request.BeginGetResponse(_completeGetResponse<TResponse>, r);
        }

        private static void _defaultHandleWebException(WebException wex)
        {
            Trace.TraceError(wex.ToString());
        }

        private static void _defaultHandleException(Exception wex)
        {
            Trace.TraceError(wex.ToString());
        }

        private static void _defaultDoNothing()
        {
        }

        /// <summary>
        /// Fetches the request asynchronously and calls one of the appropriate handlers when completed (likely on a separate thread).
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize the response object to</typeparam>
        /// <param name="handleWebException">Called if a WebException is generated</param>
        /// <param name="handleGenericException">Called if an Exception is generated</param>
        /// <param name="completedWithError">Called when either error handler completes</param>
        /// <param name="handleResponse">Called to handle the successful response with the deserialized response object</param>
        public void FetchAsync<TResponse>(
            Action<WebException> handleWebException,
            Action<Exception> handleGenericException,
            Action completedWithError,
            Action<TResponse> handleResponse)
        {
            if (handleResponse == null) throw new ArgumentNullException("handleResponse");

            _auth.Authenticate(_request);
            _auth = null;

            var _handleWebException = handleWebException ?? _defaultHandleWebException;
            var _handleException = handleGenericException ?? _defaultHandleException;
            var _completedWithError = completedWithError ?? _defaultDoNothing;
            var _handleResponse = handleResponse;

            var ast = new AsyncExecutionState<TResponse>(_body, _request, _handleWebException, _handleException, _completedWithError, _handleResponse);

            try
            {
                if (Body != null)
                {
                    // Send the body:
                    _request.BeginGetRequestStream(_completeGetRequestStream<TResponse>, ast);
                }
                else
                {
                    _request.BeginGetResponse(_completeGetResponse<TResponse>, ast);
                }
            }
            catch (WebException wex)
            {
                _handleWebException(wex);
                _completedWithError();
            }
            catch (Exception ex)
            {
                _handleException(ex);
                _completedWithError();
            }
        }

        #endregion

        #region Synchronous execution

        /// <summary>
        /// Fetches the request synchronously and deserializes the response body.
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize the response object to</typeparam>
        /// <returns></returns>
        public TResponse Fetch<TResponse>()
        {
            _auth.Authenticate(_request);

            if (Body != null)
            {
                using (var reqstr = Request.GetRequestStream())
                using (var tw = new System.IO.StreamWriter(reqstr, UTF8EncodingNoBOM))
                using (var jtw = new Newtonsoft.Json.JsonTextWriter(tw))
                {
                    _json.Serialize(jtw, Body);
                }
            }

            var rsp = (HttpWebResponse)Request.GetResponse();

            using (var rs = rsp.GetResponseStream())
            using (var tr = new System.IO.StreamReader(rs, UTF8EncodingNoBOM))
            using (var jtr = new Newtonsoft.Json.JsonTextReader(tr))
            {
                var result = _json.Deserialize<TResponse>(jtr);
                return result;
            }
        }

        #endregion
    }
}
