using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace NavigationDrawerPopUpMenu2.Web
{
	public class OpenWeatherMap
	{

		private static void Request()
		{
			string url = "http://api.openweathermap.org/data/2.5/weather?q=Astana&units=metric&appid=[your_app_id]";

			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

			string response;

			using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
			{
				response = streamReader.ReadToEnd();
			}

			WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

		
		}

	}

}