using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace M2MqttLocation.Services
{
    /// <summary>
    /// MQTT based GeoService
    /// </summary>
    public class MqttGeoService : GeoServiceBase
    {
        #region Constants ...

        // location topic to publish data via MQTT
        private const string MQTT_LOCATION_TOPIC = "/location";
        // geofence topics to publish data via MQTT
        private const string MQTT_GEOFENCE_TOPIC = "/geofence";
        
        #endregion

        // MQTT connection
        private string clientId;
        private MqttClient mqttClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="brokerHostName">MQTT broker host name</param>
        /// <param name="brokerPort">MQTT broker port</param>
        /// <param name="secure">Use SSL/TLS</param>
        public MqttGeoService(string brokerHostName, 
            int brokerPort = MqttSettings.MQTT_BROKER_DEFAULT_PORT, 
            bool secure = false) : base()
        {
            this.clientId = Guid.NewGuid().ToString();
            this.mqttClient = new MqttClient(brokerHostName, brokerPort, secure);
        }

        #region GeoServiceBase implementation ...

        public override bool StartInternal()
        {
            // connect to the broker
            int mqttConnAck = this.mqttClient.Connect(this.clientId);

            return (mqttConnAck == MqttMsgConnack.CONN_ACCEPTED);
        }

        public override void StopInternal()
        {
            // disconnect from broker
            this.mqttClient.Disconnect();
        }

        public override void PublishLocation(Geoposition position)
        {
            // string location info
            string location = String.Format(PUBLISH_LOCATION_JSON_FORMAT,
                                            position.Coordinate.Latitude,
                                            position.Coordinate.Longitude);

            // MQTT client up, running and connected
            if ((this.mqttClient != null) && (this.mqttClient.IsConnected))
            {
                // public location on right topic
                this.mqttClient.Publish(MQTT_LOCATION_TOPIC, Encoding.UTF8.GetBytes(location));
            }
        }
        
        public override void PublishGeofence(Geoposition position, GeofenceState state)
        {
            // string geofence info
            string geofence = String.Format(PUBLISH_GEOFENCE_JSON_FORMAT,
                                            position.Coordinate.Latitude,
                                            position.Coordinate.Longitude,
                                            (state == GeofenceState.Entered));

            // MQTT client up, running and connected
            if ((this.mqttClient != null) && (this.mqttClient.IsConnected))
            {
                // public geofence
                this.mqttClient.Publish(MQTT_GEOFENCE_TOPIC, Encoding.UTF8.GetBytes(geofence));
            }
        }

        #endregion
    }
}
