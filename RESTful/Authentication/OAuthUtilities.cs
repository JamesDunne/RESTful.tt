﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Web;

namespace RESTful.Authentication
{
    internal static class OAuthUtilities
    {
        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }

        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        private class QueryParameter
        {
            private string name = null;
            private string value = null;

            public QueryParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get { return name; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        private class QueryParameterComparer : IComparer<QueryParameter>
        {
            #region IComparer<QueryParameter> Members

            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }

        private const string OAuthVersion = "1.0";
        private const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        private const string OAuthConsumerKeyKey = "oauth_consumer_key";
        private const string OAuthCallbackKey = "oauth_callback";
        private const string OAuthVersionKey = "oauth_version";
        private const string OAuthSignatureMethodKey = "oauth_signature_method";
        private const string OAuthSignatureKey = "oauth_signature";
        private const string OAuthTimestampKey = "oauth_timestamp";
        private const string OAuthNonceKey = "oauth_nonce";
        private const string OAuthTokenKey = "oauth_token";
        private const string OAuthTokenSecretKey = "oauth_token_secret";

        private const string HMACSHA1SignatureType = "HMAC-SHA1";
        private const string PlainTextSignatureType = "PLAINTEXT";
        private const string RSASHA1SignatureType = "RSA-SHA1";

        private static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeStamp = ts.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            // remove any fractions of seconds
            int pointIndex = timeStamp.IndexOf(".");
            if (pointIndex != -1)
                timeStamp = timeStamp.Substring(0, pointIndex);
            return timeStamp;
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns>A nonce suitable for Google's two-legged OAuth implementation</returns>
        public static string GenerateNonce()
        {
            // changed from the original oauth code to use Guid
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private static List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            // now temp[1], the value, might contain encoded data, that would be double encoded later. 
                            // also it MIGHT contain encoding of the lowercase kind, which throws OAUTH off
                            // the same is true for the name
                            string name = HttpUtility.UrlDecode(temp[0]);
                            string value = HttpUtility.UrlDecode(temp[1]);
                            result.Add(new QueryParameter(name, value));
                        }
                        else
                        {
                            result.Add(new QueryParameter(HttpUtility.UrlDecode(s), string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// All parameter names and values are escaped using the [RFC3986]  
        /// percent-encoding (%xx) mechanism. Characters not in the unreserved character 
        /// MUST be encoded. Characters in the unreserved character set MUST NOT be encoded. 
        /// Hexadecimal characters in encodings MUST be upper case. Text names and values MUST be 
        /// encoded as UTF-8 octets before percent-encoding them per [RFC3629] 
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public static string EncodingPerRFC3986(string value)
        {
            StringBuilder result = new StringBuilder(value.Length + 10);

            for (int i = 0; i < value.Length; ++i)
            {
                char symbol = value[i];
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

        /// <summary>
        /// Normalizes the request parameters according to the spec for the signature generation.
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        private static string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder(parameters.Sum(p => p.Name.Length + p.Value.Length + 10));

            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter p = parameters[i];
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}",
                                            EncodingPerRFC3986(p.Name),
                                            EncodingPerRFC3986(p.Value));

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <returns>The signature base</returns>
        public static string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret,
                                                    string httpMethod, string timeStamp, string nonce, string signatureType)
        {
            if (token == null)
            {
                token = string.Empty;
            }

            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (string.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("signatureType");
            }

            string normalizedUrl = null;
            string normalizedRequestParameters = null;

            List<QueryParameter> parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, token));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder(httpMethod.Length + normalizedUrl.Length + normalizedRequestParameters.Length + 30);
            signatureBase.AppendFormat(CultureInfo.InvariantCulture, "{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat(CultureInfo.InvariantCulture, "{0}&", EncodingPerRFC3986(normalizedUrl));
            signatureBase.AppendFormat(CultureInfo.InvariantCulture, "{0}", EncodingPerRFC3986(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public static string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>          
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <returns>A base64 string of the hash value</returns>
        public static string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1);
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>          
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
        public static string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType)
        {
            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType);

                    using (HMACSHA1 hmacsha1 = new HMACSHA1())
                    {
                        hmacsha1.Key = Encoding.ASCII.GetBytes(GenerateOAuthSignature(consumerSecret, tokenSecret));

                        return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                    }
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// oauth_signature is set to the concatenated encoded values of the Consumer Secret and Token Secret, 
        /// separated by a & character (ASCII code 38), even if either secret is empty. This version calls
        /// GenerateOAuthSignature and encodes the whole signature again
        /// The result MUST be encoded again. 
        /// </summary>
        /// <param name="consumerSecret"></param>
        /// <param name="tokenSecret"></param>
        /// <returns></returns>
        public static string GenerateOAuthSignatureEncoded(string consumerSecret, string tokenSecret)
        {
            return EncodingPerRFC3986(GenerateOAuthSignature(consumerSecret, tokenSecret));
        }

        /// <summary>
        /// oauth_signature is set to the concatenated encoded values of the Consumer Secret and Token Secret, 
        /// separated by a & character (ASCII code 38), even if either secret is empty. 
        /// The result MUST be encoded again. 
        /// </summary>
        /// <param name="consumerSecret"></param>
        /// <param name="tokenSecret"></param>
        /// <returns></returns>
        public static string GenerateOAuthSignature(string consumerSecret, string tokenSecret)
        {
            return string.Format("{0}&{1}",
                EncodingPerRFC3986(consumerSecret),
                string.IsNullOrEmpty(tokenSecret) ? "" : EncodingPerRFC3986(tokenSecret)
            );
        }

        /// <summary>
        /// Generates an OAuth header.
        /// </summary>
        /// <param name="uri">The URI of the request</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="httpMethod">The http method</param>
        /// <returns>The OAuth authorization header</returns>
        public static string GenerateHeader(Uri uri, String consumerKey, String consumerSecret, String httpMethod)
        {
            return GenerateHeader(uri, consumerKey, consumerSecret, string.Empty, string.Empty, httpMethod);
        }

        public static string GenerateHeader(Uri uri, String consumerKey, String consumerSecret, String token, String tokenSecret, String httpMethod)
        {
            string timeStamp = GenerateTimeStamp();
            string nonce = GenerateNonce();

            string signature = GenerateSignature(uri, consumerKey, consumerSecret, token, tokenSecret, httpMethod.ToUpper(), timeStamp, nonce);

            StringBuilder sb = new StringBuilder(150 + nonce.Length + timeStamp.Length + consumerKey.Length + (token ?? "").Length + signature.Length);
            sb.Append("OAuth oauth_version=\"1.0\",");
            sb.AppendFormat("oauth_nonce=\"{0}\",", EncodingPerRFC3986(nonce));
            sb.AppendFormat("oauth_timestamp=\"{0}\",", EncodingPerRFC3986(timeStamp));
            sb.AppendFormat("oauth_consumer_key=\"{0}\",", EncodingPerRFC3986(consumerKey));
            if (!String.IsNullOrEmpty(token))
            {
                sb.AppendFormat("oauth_token=\"{0}\",", EncodingPerRFC3986(token));
            }
            sb.Append("oauth_signature_method=\"HMAC-SHA1\",");
            sb.AppendFormat("oauth_signature=\"{0}\"", EncodingPerRFC3986(signature));

            return sb.ToString();
        }
    }
}
