﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SkyLinq.Example
{
    internal class HttpClientExample : IExample
    {
        public void Run()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:53473");
            //var mediaType = "application/json";
            //var mediaType = "application/xml";
            var mediaType = "text/csv";
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(mediaType));
            HttpResponseMessage response = client.GetAsync("api/logapi/GetTopPages").Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
