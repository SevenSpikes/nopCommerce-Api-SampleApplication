using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace NopCommerce.Api.AdapterLibrary
{
    public class ApiClient
    {
        protected const string DefaultContentType = "application/json";
        private readonly string _accessToken;
        private readonly string _serverUrl;

        public ApiClient(string accessToken, string serverUrl)
        {
            _accessToken = accessToken;
            _serverUrl = serverUrl;
        }

        public object Call(HttpMethods method, string path)
        {
            return Call(method, path, string.Empty);
        }

        public object Call(HttpMethods method, string path, object callParams)
        {
            string requestUriString = string.Format("{0}/{1}", _serverUrl, path);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);

            httpWebRequest.ContentType = DefaultContentType;

            httpWebRequest.Headers.Add("Authorization", string.Format("Bearer {0}", _accessToken));

            httpWebRequest.Method = ((object)method).ToString();

            if (callParams != null)
            {
                if (method == HttpMethods.Get || method == HttpMethods.Delete)
                {
                    return string.Format("{0}?{1}", requestUriString, callParams);
                }

                if (method == HttpMethods.Post || method == HttpMethods.Put)
                {
                    using (new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(callParams);
                            streamWriter.Close();
                        }
                    }
                }
            }

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string encodedData = string.Empty;

            using (Stream responseStream = httpWebResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream);
                    encodedData = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }

            return encodedData;
        }

        public object Get(string path)
        {
            return Get(path, null);
        }

        public object Get(string path, NameValueCollection callParams)
        {
            return Call(HttpMethods.Get, path, callParams);
        }

        public object Post(string path, object data)
        {
            return Call(HttpMethods.Post, path, data);
        }

        public object Put(string path, object data)
        {
            return Call(HttpMethods.Put, path, data);
        }

        public object Delete(string path)
        {
            return Call(HttpMethods.Delete, path);
        }
    }
}