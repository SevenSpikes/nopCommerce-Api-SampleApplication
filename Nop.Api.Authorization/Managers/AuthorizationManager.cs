using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Nop.Api.Authorization.DTOs;

namespace Nop.Api.Authorization.Managers
{
    public class AuthorizationManager
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _serverUrl;

        public AuthorizationManager(string clientId, string clientSecret, string serverUrl)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _serverUrl = serverUrl;
        }

        public string BuildAuthUrl(string redirectUrl, string[] requestedPermissions, string state = null)
        {
            // this is the URL where nopCommerce should return the authorization code
            var returnUrl = new Uri(redirectUrl);
            
            // get the Authorization URL and redirect the user
            var authUrl = GetAuthorizationUrl(returnUrl.ToString(), requestedPermissions, state);

            return authUrl;
        }

        public string GetAuthorizationData(AuthParameters authParameters)
        {
            if (!String.IsNullOrEmpty(authParameters.Error))
            {
                throw new Exception(authParameters.Error);
            }

            // make sure we have the necessary parameters
            ValidateParameter("code", authParameters.Code);
            ValidateParameter("storeUrl", authParameters.ServerUrl);
            ValidateParameter("clientId", authParameters.ClientId);
            ValidateParameter("clientSecret", authParameters.ClientSecret);
            ValidateParameter("RedirectUrl", authParameters.RedirectUri);
            ValidateParameter("GrantType", authParameters.GrantType);
            
            // get the access token
            string accessToken = AuthorizeClient(authParameters.Code, authParameters.GrantType, authParameters.RedirectUri);

            return accessToken;
        }

        private string GetAuthorizationUrl(string callbackUrl, string[] scope, string state = null)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0}/oauth/authorize", _serverUrl);
            stringBuilder.AppendFormat("?client_id={0}", HttpUtility.UrlEncode(_clientId));
            stringBuilder.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode(callbackUrl));
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

        private string AuthorizeClient(string code, string grantType, string callbackUrl)
        {
            string requestUriString = string.Format("{0}/api/token", _serverUrl);

            string queryParameters = string.Format("client_id={0}&client_secret={1}&code={2}&grant_type={3}&redirect_uri={4}", _clientId, _clientSecret, code, grantType, callbackUrl);

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

        private void ValidateParameter(string parameterName, string parameterValue)
        {
            if (string.IsNullOrWhiteSpace(parameterValue))
            {
                throw new Exception(string.Format("{0} parameter is missing", parameterName));
            }
        }
    }
}