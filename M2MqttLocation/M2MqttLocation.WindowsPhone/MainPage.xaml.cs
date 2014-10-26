using M2MqttLocation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace M2MqttLocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IGeoService geoservice;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            if (this.geoservice == null)
                this.geoservice = new MqttGeoService("192.168.0.119");
                //this.geoservice = new HttpGeoService("192.168.0.119", 1880);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (this.geoservice != null)
                this.geoservice.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (this.geoservice != null)
                this.geoservice.Stop();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Geofence geofence;

            string id = this.tBoxName.Text;

            BasicGeoposition position;
            position.Latitude = Double.Parse(this.tBoxLatitude.Text);
            position.Longitude = Double.Parse(this.tBoxLongitude.Text);
            position.Altitude = 0.0;
            double radius = Double.Parse(this.tBoxRadius.Text);

            // the geofence is a circular region
            Geocircle geocircle = new Geocircle(position, radius);

            // want to listen for enter geofence, exit geofence and remove geofence events
            // you can select a subset of these event states
            MonitoredGeofenceStates mask = 0;

            mask |= MonitoredGeofenceStates.Entered;
            mask |= MonitoredGeofenceStates.Exited;
            mask |= MonitoredGeofenceStates.Removed;

            TimeSpan dwellTime = new TimeSpan(0);

            geofence = new Geofence(id, geocircle, mask, false, dwellTime);

            this.geoservice.AddGeofence(geofence);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.geoservice.RemoveGeofences();
        }
    }
}
