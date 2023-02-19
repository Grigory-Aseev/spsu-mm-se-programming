﻿using ISites;
using Requests;
using Tools;

namespace Sites
{
    public class OpenWeatherMap : ISite
    {
        readonly string url = "https://openweathermap.org/data/2.5/onecall?lat=59.8944&lon=30.2642&units=metric&appid=439d4b804bc8187953eb36d2a8c26a02";
        readonly string accept = "*/*";
        readonly string host = "openweathermap.org";
        readonly string referer = "https://openweathermap.org/";
        readonly List<List<string>> security = new List<List<string>>
        {
            new List<string> { "sec-ch-ua", "\"Not A;Brand\";v =\"99\", \"Chromium\";v=\"98\", \"Yandex\";v=\"22\"" },
            new List<string> { "sec-ch-ua-mobile", "?0" },
            new List<string> { "sec-ch-ua-platform", "\"Windows\"" },
            new List<string> { "Sec-Fetch-Dest", "empty" },
            new List<string> { "Sec-Fetch-Mode", "cors" },
            new List<string> { "Sec-Fetch-Site", "same-origin" }
        };
        public IGetRequest Request { get; private set; }
        readonly List<string> patternsForPasrsing = new List<string>
        {
            @"(?<=temp.:)-?\d+\.\d+",
            @"(?<=clouds.:)\d+",
            @"(?<=humidity.:)\d+",
            @"(?<=wind_speed.:)\d+",
            @"(?<=wind_deg.:)\d+",
            @"(?<=description.:.)(\w+.){1,3}(?=.,.icon)"
        };

        public OpenWeatherMap()
        {
            Request = new GetRequest(url, accept, host, referer);
            foreach (List<string> pair in security)
            {
                Request.AddToHeaders(pair.First(), pair.Last());
            }
        }

        public OpenWeatherMap(IGetRequest getRequest)
        {
            Request = getRequest;
            foreach (List<string> pair in security)
            {
                Request.AddToHeaders(pair.First(), pair.Last());
            }
        }

        public ISite WithGetRequest(IGetRequest getRequest)
        {
            return new OpenWeatherMap(getRequest);
        }

        public Weather.Weather GetWeather()
        {
            Request.Run();
            if (!Request.Connect)
            {
                return null;
            }

            Weather.Weather weather = new Parser(Request.Response).Parse(patternsForPasrsing);
            Weather.Weather weatherWithPara = new Weather.Weather(
                weather.TempC + "°C",
                weather.TempF + "°F",
                weather.Clouds + "%",
                weather.Humidity + "%",
                weather.WindSpeed + " m/s",
                weather.WindDegree != "No data" ? (Int32.Parse(weather.WindDegree) + 180) % 360 + "°": "No data",
                weather.FallOut);
            return weatherWithPara;
        }

        public void ShowWeather()
        {
            Painter painter = new Painter("Openweathermap");
            Weather.Weather weather = GetWeather();
            painter.DrawWeather(weather);
        }
    }
}
