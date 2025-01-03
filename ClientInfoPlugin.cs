using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DNWS
{
    class ClientInfoPlugin : IPlugin
    {
        protected static Dictionary<String, int> statDictionary = null;

        public ClientInfoPlugin()
        {
            if (statDictionary == null)
            {
                statDictionary = new Dictionary<String, int>();
            }
        }

        public void PreProcessing(HTTPRequest request)
        {
            if (statDictionary.ContainsKey(request.Url))
            {
                statDictionary[request.Url] = (int)statDictionary[request.Url] + 1;
            }
            else
            {
                statDictionary[request.Url] = 1;
            }
        }

        public HTTPResponse GetResponse(HTTPRequest request)
        {
            HTTPResponse response = null;
            StringBuilder sb = new StringBuilder();

            // Only respond if the request URL is '/clientinfo'
            if (request.Url.Equals("/clientinfo", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                  // Extract client IP and port information from the request
                  IPEndPoint endpoint = IPEndPoint.Parse(request.getPropertyByKey("remoteendpoint"));

                  sb.Append("<html><head>");
                  sb.Append("<style>");
                  sb.Append("body { font-family: 'Verdana', sans-serif; background-color: #D9DFC6; color: #333; padding: 20px; }"); 
                  sb.Append("h1 { color: #131010; font-family: 'Courier New', monospace; }"); 
                  sb.Append("pre { background-color: #FFFAEC; padding: 15px; border-radius: 5px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); font-size: 14px; font-family: 'Georgia', serif; }"); 
                  sb.Append("</style>");
                  sb.Append("</head><body>");

                  sb.Append("<h1>Client Information</h1>");
                  sb.Append("<pre>");
                  sb.AppendFormat("<strong>Client IP:</strong> {0}<br/>\n", endpoint.Address);
                  sb.AppendFormat("<strong>Client Port:</strong> {0}<br/>\n", endpoint.Port);
                  sb.AppendFormat("<strong>Browser Information:</strong> {0}<br/>\n", request.getPropertyByKey("user-agent").Trim());
                  sb.AppendFormat("<strong>Accept Language:</strong> {0}<br/>\n", request.getPropertyByKey("accept-language").Trim());
                  sb.AppendFormat("<strong>Accept Encoding:</strong> {0}<br/>\n", request.getPropertyByKey("accept-encoding").Trim());
                  sb.Append("</pre>");

                  sb.Append("</body></html>");
                }

                catch (Exception ex)
                {
                    sb.AppendFormat("<html><body><pre><strong>Error:</strong> {0}</pre></body></html>", ex.Message);
                }

                response = new HTTPResponse(200);
                response.body = Encoding.UTF8.GetBytes(sb.ToString());
            }
            else
            {
                // Default response for other URLs
                response = new HTTPResponse(404);
                response.body = Encoding.UTF8.GetBytes("<html><body><h1>Not Found</h1></body></html>");
            }

            return response;
        }

        public HTTPResponse PostProcessing(HTTPResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
