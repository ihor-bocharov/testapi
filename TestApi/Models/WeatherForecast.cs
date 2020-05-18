using System;
using System.ComponentModel.DataAnnotations;

namespace TestApi.Models
{
    public class WeatherForecast
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// TemperatureC
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// TemperatureF
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Summary
        /// </summary>
        [Required]
        public string Summary { get; set; }
    }
}
