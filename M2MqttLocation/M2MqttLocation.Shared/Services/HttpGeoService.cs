using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace M2MqttLocation.Services
{
    /// <summary>
    /// HTTP based GeoService
    /// </summary>
    public class HttpGeoService : GeoServiceBase
    {
        #region Constants ...

        // location path to publish data via HTTP
        private const string HTTP_LOCATION_PATH = "/location";
        // geofence path to publish data via HTTP
        private const string HTTP_GEOFENCE_PATH = "/geofence";
        // default port for HTTP requests
        private const int HTTP_DEFAULT_SERVE_PORT = 80;

        #endregion

        private HttpClient httpClient;


        public HttpGeoService(string serverHostName, int serverPort = HTTP_DEFAULT_SERVE_PORT)
            : base()
        {    
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(String.Format("http://{0}:{1}", serverHostName, serverPort));
        }
        

        #region GeoServiceBase implementation ...

        public async override void PublishLocation(Geoposition position)
        {
            // string location info
            string location = String.Format(PUBLISH_LOCATION_JSON_FORMAT,
                                            position.Coordinate.Latitude,
                                            position.Coordinate.Longitude);

            HttpResponseMessage resp = await this.httpClient.PostAsync(HTTP_LOCATION_PATH, new StringContent(location));
        }

        public async override void PublishGeofence(Geoposition position, GeofenceState state)
        {
            // string geofence info
            string geofence = String.Format(PUBLISH_GEOFENCE_JSON_FORMAT,
                                            position.Coordinate.Latitude,
                                            position.Coordinate.Longitude,
                                            (state == GeofenceState.Entered));

            HttpResponseMessage resp = null;
            // public geofence
            resp = await this.httpClient.PostAsync(HTTP_GEOFENCE_PATH, new StringContent(geofence));
        }

        public override bool StartInternal()
        {
            return true;
        }

        public override void StopInternal()
        {
            
        }

        #endregion
    }
}
