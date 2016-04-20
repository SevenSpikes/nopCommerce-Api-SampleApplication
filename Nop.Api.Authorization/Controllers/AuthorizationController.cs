using System;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Api.Authorization.DTOs;
using Nop.Api.Authorization.Managers;
using Nop.Api.Authorization.Models;

namespace Nop.Api.Authorization.Controllers
{
    public class AuthorizationController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        [HttpPost]
        //TODO: it is recommended to have an [Authorize] attribute set
        public ActionResult Submit(UserAccessModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var nopAuthorizationManager = new AuthorizationManager(model.ClientId, model.ClientSecret, model.ServerUrl);

                    var redirectUrl = Url.RouteUrl("GetAccessToken", null, Request.Url.Scheme);

                    if (redirectUrl != model.RedirectUrl)
                    {
                        return BadRequest();
                    }

                    // TODO: For now the data is saved into the TempData, but in production environment you should replace it with your database.
                    TempData["clientId"] = model.ClientId;
                    TempData["clientSecret"] = model.ClientSecret;
                    TempData["serverUrl"] = model.ServerUrl;
                    TempData["redirectUrl"] = redirectUrl;

                    // This should not be saved anywhere.
                    var state = Guid.NewGuid();
                    TempData["state"] = state;

                    string authUrl = nopAuthorizationManager.BuildAuthUrl(redirectUrl, new string[] { }, state.ToString());

                    return Redirect(authUrl);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAccessToken(string code, string state)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state))
            {
                if (state != TempData["state"].ToString())
                {
                    return BadRequest();
                }

                var model = new AccessModel();

                try
                {
                    // TODO: Here you should get the authorization user data from the database instead of TempData.
                    string clientId = TempData["clientId"].ToString();
                    string clientSecret = TempData["clientSecret"].ToString();
                    string serverUrl = TempData["serverUrl"].ToString();
                    string redirectUrl = TempData["redirectUrl"].ToString();

                    var authParameters = new AuthParameters()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        ServerUrl = serverUrl,
                        RedirectUri = redirectUrl,
                        GrantType = "authorization_code",
                        Code = code
                    };

                    var nopAuthorizationManager = new AuthorizationManager(authParameters.ClientId, authParameters.ClientSecret, authParameters.ServerUrl);

                    string responseJson = nopAuthorizationManager.GetAuthorizationData(authParameters);

                    AuthorizationModel authorizationModel = JsonConvert.DeserializeObject<AuthorizationModel>(responseJson);

                    model.AuthorizationModel = authorizationModel;
                    model.UserAccessModel = new UserAccessModel()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        ServerUrl = serverUrl,
                        RedirectUrl = redirectUrl
                    };

                    // TODO: Here you can save your access and refresh tokens in the database. For illustration purposes we will only show them in the view.
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return View("~/Views/AccessToken.cshtml", model);
            }

            return BadRequest();
        }

        private ActionResult BadRequest(string message = "Bad Request")
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
        }
    }
}