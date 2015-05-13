﻿using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NLBLib.Misc
{
    /// <summary>
    /// This class encapsulates the http fetching functionality
    /// </summary>
    class HttpRequestProcessor
    {
        private HttpClient _client;

        public HttpRequestProcessor()
        {
            _client = new HttpClient();
        }

        /// <summary>
        /// Processes http request synchronously and copies over response from routed server
        /// to client context. Calls must call Response.End() to send response back to the actuall 
        /// request client
        /// </summary>
        /// <exception cref="SocketException">if the POI server is not accessible</exception>
        /// <param name="server">The routing server</param>
        /// <param name="request">Request from the client</param>        
        /// <param name="response">Response object to update</param>        
        public void ProcessSendRequest(AppServer server, HttpRequest request, HttpResponse response)
        {
            try
            {
                //
                // Change host and port to our app server
                //
                String uriString = String.Format("{0}://{1}:{2}{3}", request.Url.Scheme, server.Host, server.Port, request.Url.PathAndQuery);
                Uri newUri = new Uri(uriString);

                HttpMethod httpMethod = new HttpMethod(request.HttpMethod);
                HttpRequestMessage forwardRequest = new HttpRequestMessage(httpMethod, newUri);
                CopyHeaders(request, forwardRequest);

                HttpResponseMessage forwardResponse = _client.SendAsync(forwardRequest, HttpCompletionOption.ResponseHeadersRead).Result;
                CopyHeaders(forwardResponse, response);
                
                //
                // Copy contents
                //
                var stream = forwardResponse.Content.ReadAsStreamAsync().Result;
                stream.CopyTo(response.OutputStream);
            }
            catch (AggregateException ae)
            {
                Exception ex = ae.InnerException;
                while (ex.InnerException != null) ex = ex.InnerException;
                throw ex;                
            }
        }

        /// <summary>
        /// Checks if URL is accessible under certain time
        /// </summary>
        /// <param name="url">URL to check</param>
        /// <returns>True if URL is accessible</returns>
        public bool IsHttpAccessible(Uri url)
        {
            try
            {
                _client.Timeout = TimeSpan.FromSeconds(30);
                HttpResponseMessage response = _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Failed to get URL {0} with message {1}", url.ToString(), ex.Message));
                return false;
            }
        }

        private void CopyHeaders(HttpRequest request, HttpRequestMessage requestMessage)
        {
            foreach (var key in request.Headers.AllKeys)
            {
                string header = request.Headers[key];
                requestMessage.Headers.TryAddWithoutValidation(key, header);                
            }
        }

        private void CopyHeaders(HttpResponseMessage responseMessage, HttpResponse response)
        {
            //
            // Copy control headers
            //
            foreach (var header in responseMessage.Headers)
            {
                AddHeaderToResponse(response, header);
            }

            //
            // Copy content headers
            //
            foreach (var header in responseMessage.Content.Headers)
            {
                AddHeaderToResponse(response, header);
            }
        }

        private void AddHeaderToResponse(HttpResponse response, KeyValuePair<string, IEnumerable<string>> header)
        {
            StringBuilder headerValue = new StringBuilder();
            var valueEnum = header.Value.GetEnumerator();
            valueEnum.MoveNext();

            do
            {
                //
                // Remove duplicate headers
                //
                if (response.Headers.AllKeys.Contains(header.Key))
                {
                    response.Headers.Remove(header.Key);
                }

                headerValue.Append(valueEnum.Current);

                if (valueEnum.MoveNext())
                {
                    headerValue.Append(";");
                }
                else
                {
                    break;
                }
            } while (true);

            response.Headers.Add(header.Key, headerValue.ToString());
        }
    }
}
