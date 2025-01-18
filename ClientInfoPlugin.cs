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

            // Check if the request URL matches "/clientinfo"
            if (request.Url.Equals("/clientinfo", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // Extract client IP and port information from the request
                    IPEndPoint endpoint = IPEndPoint.Parse(request.getPropertyByKey("remoteendpoint"));
                    string clientIp = endpoint.Address.ToString();
                    int clientPort = endpoint.Port;
                    string browerinfo = request.getPropertyByKey("user-agent").Trim();
                    string accpectLanguage = request.getPropertyByKey("accept-language").Trim();
                    string accpectEncoding = request.getPropertyByKey("accept-encoding").Trim();

                    sb.Append(@"
                        <html>
                        <head>
                            <style>
                                body {
                                    font-family: 'Verdana', sans-serif;
                                    background-color: #F5F7FE;
                                    color: #333;
                                    margin: 0;
                                    padding: 20px;
                                    line-height: 1.6;
                                }
                                h1 {
                                    color: #2C3E50;
                                    font-family: 'Courier New', monospace;
                                    text-align: center;
                                    margin-bottom: 20px;
                                }
                                pre {
                                    background-color: #FFFFFF;
                                    padding: 15px;
                                    border-radius: 5px;
                                    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
                                    font-size: 14px;
                                    font-family: 'Courier New', monospace;
                                    line-height: 1.6;
                                    overflow: auto;
                                }
                                strong {
                                    color: #33495E;
                                }
                            </style>
                        </head>
                        <body>
                            <h1>Client Information</h1>
                            <pre>
                                <strong>Client IP:</strong> " + clientIp + @"<br/>
                                <strong>Client Port:</strong> " + clientPort + @"<br/>
                                <strong>Browser Information:</strong> " + browerinfo + @"<br/>
                                <strong>Accept Language:</strong> " + accpectLanguage + @"<br/>
                                <strong>Accept Encoding:</strong> " + accpectEncoding + @"<br/>
                            </pre>
                        </body>
                        </html>
                    ");
                }

                catch (Exception ex)
                {
                    sb.Clear();
                    sb.Append(@"
                        <html>
                        <head>
                            <style>
                                body { 
                                font-family: 'Verdana', sans-serif; color: red; }
                            </style>
                        </head>
                        <body>
                            <h1>Error</h1>
                            <pre>" + ex.Message + @"</pre>
                        </body>
                        </html>
                    ");
                }

                response = new HTTPResponse(200)
                {
                    body = Encoding.UTF8.GetBytes(sb.ToString())
                };
            }
            else
            {
                // Default response for other URLs
                response = new HTTPResponse(404)
                {
                    body = Encoding.UTF8.GetBytes("<html><body><h1>Not Found</h1></body></html>")
                };
            }

            return response;
        }

        public HTTPResponse PostProcessing(HTTPResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
