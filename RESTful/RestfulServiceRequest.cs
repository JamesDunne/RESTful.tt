using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace WellDunne.REST
{
    public delegate object DeserializationDelegate(System.IO.Stream inputStream, System.Type type);
    public delegate void SerializeDelegate(System.IO.Stream outputStream);

    public sealed class RestfulServiceRequest
    {
        private IRequestAuthentication _auth;

        private HttpWebRequest _request;
        public HttpWebRequest Request { get { return _request; } }

        private SerializeDelegate _serializeBody;
        private DeserializationDelegate _deserializeBody;

        public RestfulServiceRequest(IRequestAuthentication authentication, HttpWebRequest httpWebRequest, SerializeDelegate serializeBody, DeserializationDelegate deserializeBody)
        {
            _auth = authentication;
            _request = httpWebRequest;
            _serializeBody = serializeBody;
            _deserializeBody = deserializeBody;
        }

        private void Clear()
        {
            _auth = null;
            _request = null;
            _serializeBody = null;
            _deserializeBody = null;
            _bodyBytes = null;
        }

        private byte[] _bodyBytes;
        public byte[] SerializedBody { get { return _bodyBytes; } }

        private static readonly Newtonsoft.Json.JsonSerializer _json =
            Newtonsoft.Json.JsonSerializer.Create(
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    Converters = new Newtonsoft.Json.JsonConverter[] {
                        new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                    }
                }
            );

        public static void SerializeJSON(System.IO.Stream outputStream, object body)
        {
            using (var tw = new System.IO.StreamWriter(outputStream, UTF8EncodingNoBOM))
            using (var jtw = new Newtonsoft.Json.JsonTextWriter(tw))
            {
                _json.Serialize(jtw, body);
            }
        }

        public static object DeserializeJSON(System.IO.Stream rs, System.Type type)
        {
            using (var tr = new System.IO.StreamReader(rs, UTF8EncodingNoBOM))
            using (var jtr = new Newtonsoft.Json.JsonTextReader(tr))
            {
                object result = _json.Deserialize(jtr, type);
                return result;
            }
        }

        public static void SerializeXML(System.IO.Stream outputStream, object body)
        {
            using (var tw = new System.IO.StreamWriter(outputStream, UTF8EncodingNoBOM))
            using (var xtw = new System.Xml.XmlTextWriter(tw))
            {
                // NOTE(jsd): Aware of the possible null reference on `body.GetType()`.
                var xs = new System.Xml.Serialization.XmlSerializer(body.GetType());
                xs.Serialize(xtw, body);
            }
        }

        public static object DeserializeXML(System.IO.Stream rs, System.Type type)
        {
            using (var tr = new System.IO.StreamReader(rs, UTF8EncodingNoBOM))
            using (var xtr = new System.Xml.XmlTextReader(tr))
            {
                var xs = new System.Xml.Serialization.XmlSerializer(type);
                object result = xs.Deserialize(xtr);
                return result;
            }
        }

        private static readonly Encoding UTF8EncodingNoBOM = new UTF8Encoding(false);

        #region Asynchronous execution

        private sealed class AsyncExecutionState<TResponse>
        {
            public byte[] bodyBytes { get; private set; }
            public HttpWebRequest request { get; private set; }
            public Action<WebException> handleWebException { get; private set; }
            public Action<Exception> handleException { get; private set; }
            public Action<Exception> completedWithError { get; private set; }
            public DeserializationDelegate deserializeBody { get; private set; }
            public Action<TResponse> handleResponse { get; private set; }

            public AsyncExecutionState(byte[] bodyBytes, HttpWebRequest request, Action<WebException> handleWebException, Action<Exception> handleException, Action<Exception> completedWithError, DeserializationDelegate deserializeBody, Action<TResponse> handleResponse)
            {
                this.bodyBytes = bodyBytes;
                this.request = request;
                this.handleWebException = handleWebException;
                this.handleException = handleException;
                this.completedWithError = completedWithError;
                this.deserializeBody = deserializeBody;
                this.handleResponse = handleResponse;
            }

            public void clear()
            {
                bodyBytes = null;
                request = null;
                handleException = null;
                handleWebException = null;
                handleResponse = null;
                completedWithError = null;
                deserializeBody = null;
            }
        }

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
                r.completedWithError(wex);
                r.clear();
                r = null;
                return;
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError(ex);
                r.clear();
                r = null;
                return;
            }

            try
            {
                using (var rs = rsp.GetResponseStream())
                {
                    Debug.Assert(r.deserializeBody != null);
                    var result = (TResponse)r.deserializeBody(rs, typeof(TResponse));
                    r.handleResponse(result);
                }
            }
            catch (WebException wex)
            {
                r.handleWebException(wex);
                r.completedWithError(wex);
                r.clear();
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError(ex);
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
                r.completedWithError(wex);
                r.clear();
                r = null;
                reqstr = null;
                return;
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError(ex);
                r.clear();
                r = null;
                reqstr = null;
                return;
            }

            try
            {
                using (reqstr)
                    reqstr.Write(r.bodyBytes, 0, r.bodyBytes.Length);
            }
            catch (WebException wex)
            {
                r.handleWebException(wex);
                r.completedWithError(wex);
                r.clear();
                r = null;
                reqstr = null;
                return;
            }
            catch (Exception ex)
            {
                r.handleException(ex);
                r.completedWithError(ex);
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

        private static void _defaultHandleException(Exception ex)
        {
            Trace.TraceError(ex.ToString());
        }

        private static void _defaultDoNothing(Exception ex)
        {
        }

        private byte[] serializeBody()
        {
            byte[] bodyBytes = null;
            if (_serializeBody != null)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    _serializeBody(ms);
                    bodyBytes = ms.ToArray();
                }
            }
            return bodyBytes;
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
            Action<Exception> completedWithError,
            Action<TResponse> handleResponse)
        {
            if (handleResponse == null) throw new ArgumentNullException("handleResponse");

            var _handleWebException = handleWebException ?? _defaultHandleWebException;
            var _handleException = handleGenericException ?? _defaultHandleException;
            var _completedWithError = completedWithError ?? _defaultDoNothing;

            try
            {
                _bodyBytes = serializeBody();
                _auth.Authenticate(_request, _bodyBytes);
                _auth = null;

                var _handleResponse = handleResponse;

                var ast = new AsyncExecutionState<TResponse>(_bodyBytes, _request, _handleWebException, _handleException, _completedWithError, _deserializeBody, _handleResponse);

                if (_bodyBytes != null)
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
                _completedWithError(wex);
            }
            catch (Exception ex)
            {
                _handleException(ex);
                _completedWithError(ex);
            }
        }

        #endregion

        #region Synchronous execution

        /// <summary>
        /// Fetches the request synchronously and deserializes the response body.
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize the response object to</typeparam>
        /// <returns></returns>
        public RestfulServiceResponse<TResponse> Fetch<TResponse>()
        {
            _bodyBytes = serializeBody();
            _auth.Authenticate(_request, _bodyBytes);

            if (_bodyBytes != null)
            {
                using (var reqstr = _request.GetRequestStream())
                    reqstr.Write(_bodyBytes, 0, _bodyBytes.Length);
            }

#if false
            Console.WriteLine(String.Empty);
            Console.WriteLine(String.Format("{0} {1}", _request.Method, _request.RequestUri.AbsoluteUri));
            Console.WriteLine(String.Empty);
            foreach (string hd in _request.Headers)
            {
                Console.WriteLine(String.Format("{0}: {1}", hd, _request.Headers[hd]));
            }
            Console.WriteLine(String.Empty);
            if (_bodyBytes != null)
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine(UTF8EncodingNoBOM.GetString(_bodyBytes));
                Console.WriteLine(String.Empty);
            }
#endif

            HttpWebResponse rsp;
            try
            {
                rsp = (HttpWebResponse)_request.GetResponse();
            }
            catch (WebException wex)
            {
                rsp = (HttpWebResponse)wex.Response;
                if (rsp == null)
                    throw wex;
            }
            catch
            {
                throw;
            }

            using (var rs = rsp.GetResponseStream())
            {
                var result = (TResponse)_deserializeBody(rs, typeof(TResponse));
                return new RestfulServiceResponse<TResponse>(rsp.StatusCode, result);
            }
        }

        public TResponse DeserializeResponse<TResponse>(System.IO.Stream responseStream)
        {
            using (responseStream)
            {
                var result = (TResponse)_deserializeBody(responseStream, typeof(TResponse));
                return result;
            }
        }

        public System.IO.Stream FetchStream(out HttpStatusCode statusCode, out WebHeaderCollection responseHeaders)
        {
            _bodyBytes = serializeBody();
            _auth.Authenticate(_request, _bodyBytes);

            if (_bodyBytes != null)
            {
                using (var reqstr = _request.GetRequestStream())
                    reqstr.Write(_bodyBytes, 0, _bodyBytes.Length);
            }

#if false
            Console.WriteLine(String.Empty);
            Console.WriteLine(String.Format("{0} {1}", _request.Method, _request.RequestUri.AbsoluteUri));
            Console.WriteLine(String.Empty);
            foreach (string hd in _request.Headers)
            {
                Console.WriteLine(String.Format("{0}: {1}", hd, _request.Headers[hd]));
            }
            Console.WriteLine(String.Empty);
            if (_bodyBytes != null)
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine(UTF8EncodingNoBOM.GetString(_bodyBytes));
                Console.WriteLine(String.Empty);
            }
#endif

            HttpWebResponse rsp;
            try
            {
                rsp = (HttpWebResponse)_request.GetResponse();
            }
            catch (WebException wex)
            {
                rsp = (HttpWebResponse)wex.Response;
                if (rsp == null)
                    throw wex;
            }
            catch
            {
                throw;
            }

            responseHeaders = rsp.Headers;
            statusCode = rsp.StatusCode;
            return rsp.GetResponseStream();
        }

        #endregion
    }
}
