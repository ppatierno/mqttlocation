using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace M2MqttLocation.Services
{
    public abstract class GeoServiceBase : IGeoService
    {
        #region Constants ...

        // JSON format for publishing position
        protected const string PUBLISH_LOCATION_JSON_FORMAT = "{{ \"lat\" : {0}, \"lon\" : {1} }}";
        // JSON format for publishing geofence
        protected const string PUBLISH_GEOFENCE_JSON_FORMAT = "{{ \"lat\" : {0}, \"lon\" : {1}, \"entered\" : {2} }}";

        #endregion

        // access to location
        protected Geolocator geoLocator;
        // geofences list
        protected IList<Geofence> geofences;

        /// <summary>
        /// Constructor
        /// </summary>
        public GeoServiceBase()
        {
            this.geoLocator = new Geolocator();
            this.geofences = GeofenceMonitor.Current.Geofences;

#if WINDOWS_PHONE_APP
            // You must set the MovementThreshold for 
            // distance-based tracking or ReportInterval for
            // periodic-based tracking before adding event handlers.
            // If not set, an exceptikn is thrown
            //
            // Value of 1000 milliseconds (1 second)
            // isn't a requirement, it is just an example.
            //this.geoLocator.ReportInterval = 1000;
            this.geoLocator.MovementThreshold = 10;
#endif

            // from MSDN documentation :
            // We need to set DesideredAccuracy to PositionAccuracy.High for using location
            // sensor simulator. If you leave the accuracy at its default value of 
            // PositionAccuracy.Default, the PositionChanged event doesn’t recognize position
            // changes that occur in the location sensor simulator.
            this.geoLocator.DesiredAccuracy = PositionAccuracy.High;
        }

        #region IGeoService interface ...

        public event EventHandler<GeoPositionChangedEventArgs> GeoPositionChanged;

        public bool Start()
        {
            // if location service is available and not disabled on current platform
            if ((geoLocator.LocationStatus != PositionStatus.NotAvailable) &&
                (geoLocator.LocationStatus != PositionStatus.Disabled))
            {
                this.geoLocator.StatusChanged += geoLocator_StatusChanged;
                this.geoLocator.PositionChanged += geoLocator_PositionChanged;
                
                GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
                GeofenceMonitor.Current.StatusChanged += Current_StatusChanged;

                // internal start based on protocol implementation
                if (this.StartInternal())
                    return true;
                else
                {
                    this.Stop();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Stop()
        {
            this.StopInternal();

            this.geoLocator.StatusChanged -= geoLocator_StatusChanged;
            this.geoLocator.PositionChanged -= geoLocator_PositionChanged;

            GeofenceMonitor.Current.GeofenceStateChanged -= Current_GeofenceStateChanged;
            GeofenceMonitor.Current.StatusChanged -= Current_StatusChanged;
        }

        public void AddGeofence(Geofence geofence)
        {
            this.geofences.Add(geofence);
        }

        public bool RemoveGeofence(Geofence geofence)
        {
            if (this.geofences.Contains(geofence))
                return this.geofences.Remove(geofence);
            return false;
        }

        public void RemoveGeofences()
        {
            GeofenceMonitor.Current.Geofences.Clear();
            this.geofences.Clear();
        }

        #endregion

        protected void geoLocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition pos = args.Position;

            this.PublishLocation(pos);
        }

        protected void geoLocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            // TODO

            switch (args.Status)
            {
                case PositionStatus.NotAvailable:
                case PositionStatus.NotInitialized:
                case PositionStatus.Initializing:
                case PositionStatus.NoData:
                case PositionStatus.Ready:
                case PositionStatus.Disabled:
                    break;
            }
        }

        protected void Current_StatusChanged(GeofenceMonitor sender, object args)
        {
            //throw new NotImplementedException();
        }

        protected void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            foreach (var report in reports)
            {
                GeofenceState state = report.NewState;

                Geofence geofence = report.Geofence;

                if (state == GeofenceState.Removed)
                {
                    // remove the geofence from the client side geofences collection
                    this.RemoveGeofence(geofence);
                }
                else if (state == GeofenceState.Entered || state == GeofenceState.Exited)
                {
                    this.PublishGeofence(report.Geoposition, state);
                }
            }
        }

        /// <summary>
        /// Raise event for geo position changed
        /// </summary>
        /// <param name="position">Position</param>
        private void OnGeoPositionChanged(Geoposition position)
        {
            var handler = this.GeoPositionChanged;
            if (handler != null)
            {
                handler(this, new GeoPositionChangedEventArgs(position));
            }
        }

        public abstract void PublishLocation(Geoposition position);

        public abstract void PublishGeofence(Geoposition position, GeofenceState state);

        public abstract bool StartInternal();

        public abstract void StopInternal();
    }
}
