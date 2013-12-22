using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SkyLinq.Sample
{
    class HttpClientSample : ISample
    {
        public void Run()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:53473");
            //var mediaType = "application/json";
            //var mediaType = "application/xml";
            var mediaType = "application/csv";
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(mediaType));
            HttpResponseMessage response = client.GetAsync("api/log").Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
