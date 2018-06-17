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

        public void OnLocationChanged(Location location)
        {
            // Show toast of location
            lat = location.Latitude;
            lon = location.Longitude;
            _coordinates = lat.ToString() + ", " + lon.ToString();
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "Please enable GPS.", ToastLength.Short).Show();
        }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            if (status == Availability.TemporarilyUnavailable || status == Availability.OutOfService)
            {
                Toast.MakeText(this, "Please check your GPS settings.", ToastLength.Short).Show();
            }
        }

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
                // 1. Instantiate an AlertDialog.Builder with its constructor
                AlertDialog.Builder builder = new AlertDialog.Builder(this);

                // 2. Chain together various setter methods to set the dialog characteristics
                builder.SetMessage(e.ToString())
                       .SetTitle("Error");



                // 3. Get the AlertDialog from create()
                AlertDialog dialog = builder.Create();
                dialog.Show();
            }
        }
    }
}

