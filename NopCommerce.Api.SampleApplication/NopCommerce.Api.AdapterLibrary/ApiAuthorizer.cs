using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace NopCommerce.Api.AdapterLibrary
{
    public class ApiAuthorizer
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _serverUrl;

        public ApiAuthorizer(string clientId, string clientSecret, string serverUrl)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _serverUrl = serverUrl;
        }

        public string GetAuthorizationUrl(string redirectUrl, string[] scope, string state = null)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0}/oauth/authorize", _serverUrl);
            stringBuilder.AppendFormat("?client_id={0}", HttpUtility.UrlEncode(_clientId));
            stringBuilder.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode(redirectUrl));
            stringBuilder.Append("&response_type=code");

            if (!string.IsNullOrEmpty(state))
            {
                stringBuilder.AppendFormat("&state={0}", state);
            }

            if (scope != null && scope.Length > 0)
            {
                string scopeJoined = string.Join(",", scope);
                stringBuilder.AppendFormat("&scope={0}", HttpUtility.UrlEncode(scopeJoined));
            }

            return stringBuilder.ToString();
        }

        public string AuthorizeClient(string code, string grantType, string redirectUrl)
        {
            string requestUriString = string.Format("{0}/api/token", _serverUrl);

            string queryParameters = string.Format("client_id={0}&client_secret={1}&code={2}&grant_type={3}&redirect_uri={4}", _clientId, _clientSecret, code, grantType, redirectUrl);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            using (new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(queryParameters);
                    streamWriter.Close();
                }
            }

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string json = string.Empty;

            using (Stream responseStream = httpWebResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream);
                    json = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }

            return json;
        }

        public string RefreshToken(string refreshToken, string grantType)
        {
            string requestUriString = string.Format("{0}/api/token", _serverUrl);

            string queryParameters = string.Format("client_id={0}&client_secret={1}&grant_type={2}&refresh_token={3}", _clientId, _clientSecret, grantType, refreshToken);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            using (new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(queryParameters);
                    streamWriter.Close();
                }
            }

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string json = string.Empty;

            using (Stream responseStream = httpWebResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream);
                    json = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }

            return json;
        }
    }
}