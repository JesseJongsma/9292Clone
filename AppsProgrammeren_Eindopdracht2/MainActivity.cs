using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Runtime;
using Android.Content;
using Android.Util;
using System;

namespace AppsProgrammeren_Eindopdracht2
{
    [Activity(Label = "AppsProgrammeren_Eindopdracht2", MainLauncher = true)]
    public class MainActivity : Activity, ILocationListener
    {
        private PublicTransport _publicTransport;
        private LocationManager _locationManager;
        private double lat, lon;
        private string _coordinates = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            _locationManager = (LocationManager)GetSystemService(Context.LocationService);
            _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);

            Button button = FindViewById<Button>(Resource.Id.buttonSearch);

            //Assign The Event To Button
            button.Click += delegate
            {
                //Call Your Method When User Clicks The Button
                clickButtonSearch();
            };
        }

        /// <summary>
        /// This method keeps the location up-to-date in the program
        /// </summary>
        /// <param name="location">The current location of the GPS</param>
        public void OnLocationChanged(Location location)
        {
            // Show toast of location
            lat = location.Latitude;
            lon = location.Longitude;
            _coordinates = lat.ToString() + ", " + lon.ToString();
        }

        /// <summary>
        /// This method is called while GPS is disabled
        /// </summary>
        /// <param name="provider"></param>
        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "Please enable GPS.", ToastLength.Short).Show();
        }

        /// <summary>
        /// This method is called while GPS is enabled 
        /// </summary>
        /// <param name="provider"></param>
        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            if (status == Availability.TemporarilyUnavailable || status == Availability.OutOfService)
            {
                Toast.MakeText(this, "Please check your GPS settings.", ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Retrieve and display the routes when the user presses the submit button
        /// </summary>
        public void clickButtonSearch()
        {
            string locationText = FindViewById<EditText>(Resource.Id.location).Text;
            ListView listView = (ListView)FindViewById(Resource.Id.listView1);
            try
            {
                _publicTransport = new PublicTransport(_coordinates, locationText);
                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _publicTransport.GetTransitDetails());
                listView.Adapter = adapter;
            }
            catch (Exception e)
            {
                // Show a dialog with the error
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage(e.ToString())
                       .SetTitle("Error");
                AlertDialog dialog = builder.Create();
                dialog.Show();
            }
        }
    }
}

