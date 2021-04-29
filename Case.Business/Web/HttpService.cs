using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

namespace Case.Business
{
    public class HttpService {
        public HttpContext CurrentHttpContext;
        private string _userName;
        private string _password;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private JsonSerializerSettings _jsonSerializerSettings;

        public string RootUrl { get; set; }

        public HttpService(string url, string userName = null, string password = null, string authorizationBearer=null) {
            RootUrl = url;
            _userName = userName;
            _password = password;
            if (!string.IsNullOrWhiteSpace(authorizationBearer))
                _headers.Add("Authorization", authorizationBearer);
            _jsonSerializerSettings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task<XmlDocument> PostXMLData(string route, string reqXml)
        {
            var apiUrl = $"{RootUrl.TrimEnd('/')}/{route}";

            XmlDocument responseText = await Task.Run(() =>
            {
                XmlDocument xmlResponse = null;
                HttpWebResponse httpWebResponse = null;
                Stream requestStream = null;
                Stream responseStream = null;
                // Create HttpWebRequest for the API URL.
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);

                try
                {
                    // Set HttpWebRequest properties
                    var bytes = System.Text.Encoding.ASCII.GetBytes(reqXml);
                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentLength = bytes.Length;
                    httpWebRequest.ContentType = "text/xml; encoding='utf-8'";

                    //Get Stream object
                    requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();

                    // Post the Request.
                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    // If the submission is success, Status Code would be OK
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // Read response
                        responseStream = httpWebResponse.GetResponseStream();

                        if (responseStream != null)
                        {
                            var objXmlReader = new XmlTextReader(responseStream);

                            // Convert Response stream to XML
                            var xmldoc = new XmlDocument();
                            xmldoc.Load(objXmlReader);
                            xmlResponse = xmldoc;
                            objXmlReader.Close();
                        }
                    }

                    // Close Response
                    httpWebResponse.Close();
                }
                catch (WebException webException)
                {
                    throw new Exception(webException.Message);
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    // Release connections
                    if (requestStream != null)
                    {
                        requestStream.Close();
                    }

                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }

                    if (httpWebResponse != null)
                    {
                        httpWebResponse.Close();
                    }
                }

                // Return API Response
                return xmlResponse;
            });

            return responseText;
        }

        public async Task<T> Post<T>(string route, object body) {
            var url = $"{RootUrl.TrimEnd('/')}/{route}";
            var client = GetClient();
            var dataAsString = JsonConvert.SerializeObject(body, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response =await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode && response.Content != null) {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
            }
            else {
                if (response.Content != null) {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }
        public void AddHeader(string key, string value) {
            if (_headers.ContainsKey(key))
                _headers[key] = value;
            else
                _headers.Add(key, value);
        }

        private HttpClient GetClient() {
            var httpClient = new HttpClient();
            if (!string.IsNullOrWhiteSpace(_userName) && !string.IsNullOrWhiteSpace(_password)) {
                var basicParams = $"{_userName}:{_password}";
                var basicAuthorization = $"Basic {Base64Encode(basicParams)}";
                httpClient.DefaultRequestHeaders.Add("Authorization", basicAuthorization);
            }
            foreach (var header in _headers) {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        private static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

    }
}
