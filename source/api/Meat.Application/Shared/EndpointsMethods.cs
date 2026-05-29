using Microsoft.Extensions.Options;
using Meat.Application.Shared.Settings;

namespace Meat.Application.Shared
{
    public class EndpointsMethods
    {
        private readonly Endpoints Endpoints;

        public EndpointsMethods(IOptions<Endpoints> endpoints)
        {
            Endpoints = endpoints.Value;
        }

        public string Endpoint_WSAA_ADDRESS
        {
            get
            {
                return Endpoints.WSAA_ADDRESS;
            }
        }

        public string Endpoint_WSFEV1_ADDRESS
        {
            get
            {
                return Endpoints.WSFEV1_ADDRESS;
            }
        }

        public string Endpoint_WSSAP_ADDRESS
        {
            get
            {
                return Endpoints.WSSAP_ADDRESS;
            }
        }

        public string Endpoint_WSSAP_USER
        {
            get
            {
                return Endpoints.WSSAP_USER;
            }
        }

        public string Endpoint_WSSAP_PASSWORD
        {
            get
            {
                return Endpoints.WSSAP_PASSWORD;
            }
        }
    }
}