using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WhereAreYou
{

    public class Program
    {
        public static void Main(string[] args)
        {
            RunAsync().Wait();
        }
        
        static async Task RunAsync()
        {
            Console.WriteLine("Enter a location to search for.");
            string locationInput = Console.ReadLine();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://maps.googleapis.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = await client.GetAsync("maps/api/geocode/json?address=" + locationInput);
                if (response.IsSuccessStatusCode)
                {
                    GoogleAddressResponse googleAddress = await response.Content.ReadAsAsync<GoogleAddressResponse>();
                    if (googleAddress.results != null && googleAddress.results.Length > 0)
                    {
                        int selectedItem;
                        if (googleAddress.results.Length > 1)
                        {
                            int i = 1;
                            foreach (result item in googleAddress.results)
                            {
                                Console.WriteLine("{0}. {1}", i.ToString(), item.formatted_address);
                                i++;
                            }
                            
                            Console.WriteLine("Which number are you looking for?");
                            string numberSelected = Console.ReadLine();
                            
                            while(!Utilities.IsValidChoice(numberSelected, googleAddress.results.Length))
                            {
                                Console.WriteLine("INVALID CHOICE! Which number are you looking for?");
                                numberSelected = Console.ReadLine();
                            }
                            selectedItem = int.Parse(numberSelected) - 1;
                        }
                        else
                        {
                            selectedItem = 0;
                        }
                        
                        Console.WriteLine("You selected {0} with latitude of {1} and longitude of {2}", googleAddress.results[selectedItem].formatted_address, googleAddress.results[selectedItem].geometry.location.lat, googleAddress.results[selectedItem].geometry.location.lng);
                    }
                    else
                    {
                        Console.WriteLine("No Results Found.");
                    }
                }
                else
                {
                    Console.WriteLine("Error! {0}", response.StatusCode);
                }
            }
        }
    }
    
    public class GoogleAddressResponse
    {
        public result[] results { get; set; }
        public string status { get; set; }
    }
    
    public class result
    {
        public address_component[] address_components { get; set; }
        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }
    }
    
    public class address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }
    
    public class geometry
    {
        public nesw bounds { get; set; }
        public latlong location { get; set; }
        public string location_type { get; set; }
        public nesw viewport { get; set; }
    }
    
    public class nesw
    {
        public latlong northeast { get; set; }
        public latlong southwest { get; set; } 
    }
    
    public class latlong
    {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }
    
    public class Utilities
    {
        public static bool IsValidChoice(string numberInput, int length)
        {
            int FinalNumber;
            int.TryParse(numberInput, out FinalNumber);
            if (FinalNumber > 0 && FinalNumber <= length)
                return true;
            else
                return false;
        }
    }
}