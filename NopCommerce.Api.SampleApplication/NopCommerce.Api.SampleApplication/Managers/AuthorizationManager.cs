using System;
using NopCommerce.Api.AdapterLibrary;
using NopCommerce.Api.SampleApplication.Parameters;

namespace NopCommerce.Api.SampleApplication.Managers
{
    public class AuthorizationManager
    {
        private readonly ApiAuthorizer _apiAuthorizer; 

        public AuthorizationManager(string clientId, string clientSecret, string serverUrl)
        {
            _apiAuthorizer = new ApiAuthorizer(clientId, clientSecret, serverUrl);
        }

        public string BuildAuthUrl(string redirectUrl, string[] requestedPermissions, string state = null)
        {
            // this is the URL where nopCommerce should return the authorization code
            var returnUrl = new Uri(redirectUrl);

            // get the Authorization URL and redirect the user
            var authUrl = _apiAuthorizer.GetAuthorizationUrl(returnUrl.ToString(), requestedPermissions, state);

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
            ValidateParameter("RedirectUrl", authParameters.RedirectUrl);
            ValidateParameter("GrantType", authParameters.GrantType);

            // get the access token
            string accessToken = _apiAuthorizer.AuthorizeClient(authParameters.Code, authParameters.GrantType, authParameters.RedirectUrl);

            return accessToken;
        }

        public string RefreshAuthorizationData(AuthParameters authParameters)
        {
            if (!String.IsNullOrEmpty(authParameters.Error))
            {
                throw new Exception(authParameters.Error);
            }

            // make sure we have the necessary parameters
            ValidateParameter("storeUrl", authParameters.ServerUrl);
            ValidateParameter("clientId", authParameters.ClientId);
            ValidateParameter("clientSecret", authParameters.ClientSecret);
            ValidateParameter("GrantType", authParameters.GrantType);
            ValidateParameter("RefreshToken", authParameters.RefreshToken);

            // get the access token
            string accessToken = _apiAuthorizer.RefreshToken(authParameters.RefreshToken, authParameters.GrantType);

            return accessToken;
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