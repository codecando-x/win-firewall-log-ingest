using MaxMind.Db;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewallLogIngestService
{
    internal class MaxmindGeoWrapper
    {
        private string ipAddress;//ipv4 or ipv6
        private string countryCode;
        private string city;
        private string latitude;
        private string longitude;
        private string postalCode;
        private string asn;

        private bool loaded = false;

        public MaxmindGeoWrapper(string ip) { 
            ipAddress = ip;
        }


        public void load()
        {
            using (var reader = new DatabaseReader(Properties.Settings.Default.MaxMindCityPath))
            {
                try
                {
                    var city = reader.City(ipAddress);

                    countryCode = city.Country.IsoCode;
                    this.city = city.City.Name;
                    postalCode = city.Postal.Code;
                    latitude = city.Location.Latitude.ToString(); 
                    longitude = city.Location.Longitude.ToString();

                    loaded = true;
                }
                catch (AddressNotFoundException addressNotFound) 
                {
                    Console.WriteLine(addressNotFound.Message);
                    loaded = false;
                }  
                
            }

        }

        public string getCity()
        {
            return city;
        }

        public bool isLoaded() { return loaded; }

        public string getCountryCode()
        {
            return countryCode;
        }

        public string getLatLng()
        {
            return latitude + ", " + longitude;
        }

        public string getAsn()
        {
            return asn;
        }
    }
}
