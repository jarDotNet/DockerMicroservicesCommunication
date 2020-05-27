using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClientService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace ClientService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IConfiguration configuration, ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return CallApiService().ToArray();
        }
        private IEnumerable<WeatherForecast> CallApiService()
        {
            IEnumerable<WeatherForecast> weatherData = new List<WeatherForecast>();
            try
            {
                var baseApiServiceAddress = _configuration["ApiServiceBaseAddress"];

                using var client = new HttpClient
                {
                    BaseAddress = new Uri(baseApiServiceAddress)
                };

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync("api/weatherforecast/weather").Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonTask = response.Content.ReadAsAsync<IEnumerable<WeatherForecast>>();
                    jsonTask.Wait();
                    weatherData = jsonTask.Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return weatherData;
        }
    }
}
