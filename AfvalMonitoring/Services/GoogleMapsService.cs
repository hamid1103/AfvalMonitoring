namespace AfvalMonitoring.Services
{
    public class GoogleMapsService
    {
        private readonly IConfiguration _config;

        public GoogleMapsService(IConfiguration config)
        {
            _config = config;
        }

        public string GetApiKey()
        {
            var key = _config["GoogleAPIKey"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("GoogleAPIKey not found in configuration. Make sure it's set in user secrets.");
            }
            return key;
        }
    }
}
