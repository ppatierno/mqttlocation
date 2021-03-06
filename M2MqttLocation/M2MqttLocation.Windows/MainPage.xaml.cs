﻿using M2MqttLocation.Services;
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
        MqttGeoService geoservice;

        public MainPage()
        {
            this.InitializeComponent();

            if (this.geoservice == null)
                this.geoservice = new MqttGeoService("localhost");
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
