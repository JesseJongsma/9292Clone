using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Org.Json;
using Android.Locations;
using Android.Content;
using Android.Widget;

namespace AppsProgrammeren_Eindopdracht2
{
    class PublicTransport
    {
        /*TODO: Er zijn op dit moment twee constanten: De API_KEY, die nodig is om de data op te mogen halen van de Google Maps Directions API,
                en ten tweede de URL, waarmee de data wordt opgehaald. Hierwordt de API_KEY aan toegevoegd. */
        private const string API_KEY = "AIzaSyAo6noQYIyE7EPtpBFQNffDGpTYnXT6tSY";
        private string URL;

        public PublicTransport(string coordinates, string destination) {
            URL = "https://maps.googleapis.com/maps/api/directions/json?" +
            "origin=" + coordinates +
            "&destination=" + destination +
            "&alternatives=true&mode=transit&key=";
        }

        /// <summary>
        /// Gets the public transport data.
        /// </summary>
        /// <returns>Public Transport data</returns>
        private string GetPublicTransportData()
        {
            string data = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + API_KEY);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            return data;
        }

        /// <summary>
        /// Gets the transit details.
        /// </summary>
        /// <returns>Transit details.</returns>
        public string[] GetTransitDetails()
        {
            List<string> transitSteps = new List<string>();
            JSONObject geoData = new JSONObject(GetPublicTransportData());

            // Check if the program found the location
            string status = geoData.GetString("status");
            if (status.ToString() != "OK")
            {
                string[] error = new string[1];
                error[0] = "No location was found";
                return error;
            }

            JSONArray routes = geoData.GetJSONArray("routes");
            for (int i = 0; i < routes.Length(); i++)
            {
                JSONArray legs = routes.GetJSONObject(i).GetJSONArray("legs");
                JSONArray steps = legs.GetJSONObject(0).GetJSONArray("steps");
                for (int j = 0; j < steps.Length(); j++)
                {
                    if (steps.GetJSONObject(j).GetString("travel_mode") == "TRANSIT")
                    {
                        JSONObject transitStep = steps.GetJSONObject(j);
                        JSONObject transitDetails = transitStep.GetJSONObject("transit_details");
                        string arrivalTime = transitDetails.GetJSONObject("arrival_time").GetString("text");
                        string arrivalStop = transitDetails.GetJSONObject("arrival_stop").GetString("name");
                        string departureTime = transitDetails.GetJSONObject("departure_time").GetString("text");
                        string departureStop = transitDetails.GetJSONObject("departure_stop").GetString("name");
                        transitSteps.Add("Departure: " + departureTime + " - " + departureStop + "\n" + 
                                         "Arrival: " + arrivalTime + " - " + arrivalStop + "\n");
                    }
                }
            }
            return transitSteps.ToArray();
        }
    }
}
