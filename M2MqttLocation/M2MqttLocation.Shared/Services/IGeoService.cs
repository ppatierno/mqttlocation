using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation.Geofencing;

namespace M2MqttLocation.Services
{
    public interface IGeoService
    {
        /// <summary>
        /// Start service tracking
        /// </summary>
        /// <returns>Started successfully or not</returns>
        bool Start();

        /// <summary>
        /// Stop service tracking
        /// </summary>
        void Stop();

        /// <summary>
        /// Add a geofence to monitor
        /// </summary>
        /// <param name="geofence">Geofence to monitor</param>
        void AddGeofence(Geofence geofence);

        /// <summary>
        /// Remove a geofence
        /// </summary>
        /// <param name="geofence">Geofence to remove</param>
        /// <returns>Geofence removed</returns>
        bool RemoveGeofence(Geofence geofence);

        /// <summary>
        /// Remove all geofences
        /// </summary>
        void RemoveGeofences();

        // event raised on geo position changed
        event EventHandler<GeoPositionChangedEventArgs> GeoPositionChanged;
    }
}
