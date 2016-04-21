using System.Web.Mvc;
using Newtonsoft.Json;
using NopCommerce.Api.AdapterLibrary;
using NopCommerce.Api.SampleApplication.DTOs;

namespace NopCommerce.Api.SampleApplication.Controllers
{
    public class CustomersController : Controller
    {
        public ActionResult GetCustomers()
        {
            // TODO: Here you should get the data from your database instead of the current Session.
            // Note: This should not be done in the action! This is only for illustration purposes.
            var accessToken = Session["accessToken"].ToString();
            var serverUrl = Session["serverUrl"].ToString();

            var nopApiClient = new ApiClient(accessToken, serverUrl);

            string jsonUrl = $"/api/customers?fields=first_name,last_name";
            object customersData = nopApiClient.Get(jsonUrl);

            var customersRootObject = JsonConvert.DeserializeObject<CustomersRootObject>(customersData.ToString());
            
            return View("~/Views/Customers.cshtml", customersRootObject);
        }
    }
}