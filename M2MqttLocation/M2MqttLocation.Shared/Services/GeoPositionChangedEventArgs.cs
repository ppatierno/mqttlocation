using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace M2MqttLocation.Services
{
    /// <summary>
    /// Event Args for geo position changed event
    /// </summary>
    public class GeoPositionChangedEventArgs
    {
        /// <summary>
        /// Position
        /// </summary>
        public Geoposition Position { get; private set; }

        /// <summary>
        /// COnstructor
        /// </summary>
        /// <param name="position">Position</param>
        public GeoPositionChangedEventArgs(Geoposition position)
        {
            this.Position = position;
        }
    }
}
