using System;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using NopCommerce.Api.SampleApplication.Managers;
using NopCommerce.Api.SampleApplication.Models;
using NopCommerce.Api.SampleApplication.Parameters;

namespace NopCommerce.Api.SampleApplication.Controllers
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

                    // For demo purposes this data is kept into the current Session, but in production environment you should keep it in your database
                    Session["clientId"] = model.ClientId;
                    Session["clientSecret"] = model.ClientSecret;
                    Session["serverUrl"] = model.ServerUrl;
                    Session["redirectUrl"] = redirectUrl;

                    // This should not be saved anywhere.
                    var state = Guid.NewGuid();
                    Session["state"] = state;

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
                if (state != Session["state"].ToString())
                {
                    return BadRequest();
                }

                var model = new AccessModel();

                try
                {
                    // TODO: Here you should get the authorization user data from the database instead from the current Session.
                    string clientId = Session["clientId"].ToString();
                    string clientSecret = Session["clientSecret"].ToString();
                    string serverUrl = Session["serverUrl"].ToString();
                    string redirectUrl = Session["redirectUrl"].ToString();

                    var authParameters = new AuthParameters()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        ServerUrl = serverUrl,
                        RedirectUrl = redirectUrl,
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

                    // TODO: Here you can save your access and refresh tokens in the database. For illustration purposes we will save them in the Session and show them in the view.
                    Session["accessToken"] = authorizationModel.AccessToken;
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return View("~/Views/AccessToken.cshtml", model);
            }

            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult RefreshAccessToken(string refreshToken, string clientId, string clientSecret, string serverUrl)
        {
            string json = string.Empty;

            if (ModelState.IsValid &&
                !string.IsNullOrEmpty(refreshToken) &&
                !string.IsNullOrEmpty(clientId) &&
                 !string.IsNullOrEmpty(clientSecret) &&
                !string.IsNullOrEmpty(serverUrl))
            {
                var model = new AccessModel();

                try
                {
                    var authParameters = new AuthParameters()
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        ServerUrl = serverUrl,
                        RefreshToken = refreshToken,
                        GrantType = "refresh_token"
                    };

                    var nopAuthorizationManager = new AuthorizationManager(authParameters.ClientId,
                        authParameters.ClientSecret, authParameters.ServerUrl);

                    string responseJson = nopAuthorizationManager.RefreshAuthorizationData(authParameters);

                    AuthorizationModel authorizationModel =
                        JsonConvert.DeserializeObject<AuthorizationModel>(responseJson);

                    model.AuthorizationModel = authorizationModel;
                    model.UserAccessModel = new UserAccessModel()
                    {
                        ClientId = clientId,
                        ServerUrl = serverUrl
                    };

                    // Here we use the temp data because this method is called via ajax and here we can't hold a session.
                    // This is needed for the GetCustomers method in the CustomersController.
                    TempData["accessToken"] = authorizationModel.AccessToken;
                    TempData["serverUrl"] = serverUrl;
                }
                catch (Exception ex)
                {
                    json = string.Format("error: '{0}'", ex.Message);

                    return Json(json, JsonRequestBehavior.AllowGet);
                }

                json = JsonConvert.SerializeObject(model.AuthorizationModel);
            }
            else
            {
                json = "error: 'something went wrong'";
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private ActionResult BadRequest(string message = "Bad Request")
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
        }
    }
}