namespace NopCommerce.Api.SampleApplication.Parameters
{
    public class AuthParameters
    {
        public string ServerUrl { get; set; }
        public string Code { get; set; }
        public string Error { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string GrantType { get; set; }
        public string RefreshToken { get; set; }
    }
}