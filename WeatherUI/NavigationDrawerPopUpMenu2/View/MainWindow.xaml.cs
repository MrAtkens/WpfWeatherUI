using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using NavigationDrawerPopUpMenu2.Modal;

namespace NavigationDrawerPopUpMenu2
{

	internal enum AccentState
	{
		ACCENT_DISABLED = 0,
		ACCENT_ENABLE_GRADIENT = 10,
		ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
		ACCENT_ENABLE_BLURBEHIND = 3,
		ACCENT_INVALID_STATE = 4
	}

	[StructLayout(LayoutKind.Sequential)]

	internal struct AccentPolicy
	{
		public AccentState accentState;
		public int accentFlags;
		public int GradientColor;
		public int AnimationId;
	}

	[StructLayout(LayoutKind.Sequential)]

	internal struct WindowCompositionAttributeData
	{
		public WindowCompositionAttribute Attribute;
		public IntPtr Data;
		public int SizeOfData;
	}

	internal enum WindowCompositionAttribute
	{
		WCA_ACCENT_POLICY = 19
	}
	public partial class MainWindow :Window
	{
		[DllImport("user32.dll")]

		internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);


		string fileInfo;
		string textBuffer;
	

		public MainWindow()
		{
			InitializeComponent();

		}

		internal void EnableBlur()
		{
			var windowHelper = new WindowInteropHelper(this);

			var accent = new AccentPolicy();
			accent.accentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

			var accentStructSize = Marshal.SizeOf(accent);

			var accentPtr = Marshal.AllocHGlobal(accentStructSize);
			Marshal.StructureToPtr(accent, accentPtr, false);

			var data = new WindowCompositionAttributeData();
			data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
			data.SizeOfData = accentStructSize;
			data.Data = accentPtr;

			SetWindowCompositionAttribute(windowHelper.Handle, ref data);

			Marshal.FreeHGlobal(accentPtr);

		}






		private void Warning(string text)
		{
			Header.Background = new SolidColorBrush(Color.FromRgb(238, 85, 87));
			HeaderText.Text = text;
		}



		private void DefaultHeader()
		{
			Header.Background = new SolidColorBrush(Color.FromRgb(0, 128, 255));
			HeaderText.Text = "Security Face";
		}


		private string AddWithDeleteSpaces(string material, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (material[i] == ' ')
				{
					material = material.Remove(i, 1);
					count--;
					i--;
				}
			}

			return material;
		}


		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			EnableBlur();
		}



		private void Maximaze_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
			{
				WindowState = WindowState.Normal;
			}
			else {
				WindowState = WindowState.Maximized;
			}
		}



		private void Minimaze_Click(object sender, RoutedEventArgs e)
		{
		    WindowState = WindowState.Minimized;
		}



		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}




		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void CloseLogin_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void LoginEnter_Click(object sender, RoutedEventArgs e)
		{

					Login.Visibility = Visibility.Collapsed;
					HeaderLogin.Visibility = Visibility.Collapsed;

					Header.Visibility = Visibility.Visible;
					GridSearch.Visibility = Visibility.Visible;
					MainGrid.Opacity = 0.8;

					AccountName.Text = LoginTextBox.Text;

			}
	

		private void LogOut_Click(object sender, RoutedEventArgs e)
		{
			Login.Visibility = Visibility.Visible;
			HeaderLogin.Visibility = Visibility.Visible;

			Header.Visibility = Visibility.Collapsed;
			GridSearch.Visibility = Visibility.Collapsed;

			DefaultHeader();

			LoginTextBox.Text = null;
			Password.Password = null;

		}


		private void SelectName_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void ButtonSearchClick(object sender, RoutedEventArgs e)
		{
			string region = "http://dataservice.accuweather.com/locations/v1/cities/search?apikey=RGkvbb5iMXpxpcboAjPfP3ATD2Cr5Rjj&q=";

			region += selectName.Text;

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(region);
			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

			List<SearchData> data = new List<SearchData>();

			using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
			{
				string JsonFormat = stream.ReadToEnd();
				data = JsonConvert.DeserializeObject<List<SearchData>>(JsonFormat);
			}

			string jsonWeather = $"http://dataservice.accuweather.com/forecasts/v1/hourly/12hour/ {data[0].Key}?apikey=RGkvbb5iMXpxpcboAjPfP3ATD2Cr5Rjj&metric=true";

			req = (HttpWebRequest)HttpWebRequest.Create(jsonWeather);
			resp = (HttpWebResponse)req.GetResponse();

			List<TenDay> SixDay = new List<TenDay>();

			using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
			{
				string JsonFormat = stream.ReadToEnd();
				SixDay = JsonConvert.DeserializeObject<List<TenDay>>(JsonFormat);
			}

			dateFirst.Content = SixDay[0].DateTime;
			dateSecond.Content = SixDay[2].DateTime;
			dateThirst.Content = SixDay[4].DateTime;
			dateFourth.Content = SixDay[6].DateTime;
			dateFifth.Content = SixDay[8].DateTime;
			dateSixth.Content = SixDay[10].DateTime;

			temputerFirst.Content = SixDay[0].Temperature.Value + SixDay[0].Temperature.Unit;
			temputerSecond.Content = SixDay[2].Temperature.Value + SixDay[2].Temperature.Unit;
			temputerThirst.Content = SixDay[4].Temperature.Value + SixDay[4].Temperature.Unit;
			temputerFourth.Content = SixDay[6].Temperature.Value + SixDay[6].Temperature.Unit;
			temputerFifth.Content = SixDay[8].Temperature.Value + SixDay[8].Temperature.Unit;
			temputerSixth.Content = SixDay[10].Temperature.Value + SixDay[10].Temperature.Unit;


			for (int i = 0; i < 11; i++)
			{
				if (i == 0 || i == 2 || i == 4 || i == 6 || i == 8 || i == 10)
				{
					switch (SixDay[i].WeatherIcon)
					{
						case 1:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/01-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/01-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/01-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/01-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/01-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/01-s.png") as ImageSource;
							break;

						case 2:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/02-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/02-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/02-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/02-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/02-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/02-s.png") as ImageSource;
							break;

						case 3:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/03-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/03-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/03-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/03-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/03-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/03-s.png") as ImageSource;
							break;

						case 4:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/04-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/04-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/04-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/04-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/04-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/04-s.png") as ImageSource;
							break;

						case 5:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/05-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/05-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/05-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/05-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/05-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/05-s.png") as ImageSource;
							break;

						case 6:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/06-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/06-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/06-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/06-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/06-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/06-s.png") as ImageSource;
							break;

						case 7:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/07-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/07-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/07-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/07-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/07-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/07-s.png") as ImageSource;
							break;

						case 8:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/08-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/08-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/08-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/08-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/08-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/08-s.png") as ImageSource;
							break;

						case 11:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/11-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/11-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/11-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/11-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/11-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/11-s.png") as ImageSource;
							break;

						case 12:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/12-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/12-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/12-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/12-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/12-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/12-s.png") as ImageSource;
							break;

						case 13:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/13-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/13-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/13-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/13-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/13-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/13-s.png") as ImageSource;
							break;

						case 14:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/14-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/14-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/14-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/14-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/14-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/14-s.png") as ImageSource;
							break;

						case 15:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/15-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/15-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/15-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/15-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/15-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/15-s.png") as ImageSource;
							break;

						case 16:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/16-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/16-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/16-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/16-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/16-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/16-s.png") as ImageSource;
							break;

						case 17:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/17-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/17-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/17-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/17-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/17-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/17-s.png") as ImageSource;
							break;

						case 18:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/18-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/18-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/18-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/18-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/18-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/18-s.png") as ImageSource;
							break;

						case 19:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/19-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/19-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/19-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/19-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/19-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/19-s.png") as ImageSource;
							break;

						case 20:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/20-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/20-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/20-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/20-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/20-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/20-s.png") as ImageSource;
							break;

						case 21:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/21-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/21-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/21-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/21-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/21-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/21-s.png") as ImageSource;
							break;

						case 22:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/22-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/22-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/22-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/22-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/22-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/22-s.png") as ImageSource;
							break;

						case 23:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/23-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/23-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/23-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/23-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/23-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/23-s.png") as ImageSource;
							break;

						case 24:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/24-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/24-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/24-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/24-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/24-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/24-s.png") as ImageSource;
							break;

						case 25:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/25-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/25-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/25-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/25-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/25-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/25-s.png") as ImageSource;
							break;

						case 26:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/26-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/26-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/26-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/26-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/26-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/26-s.png") as ImageSource;
							break;


						case 29:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/29-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/29-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/29-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/29-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/29-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/29-s.png") as ImageSource;
							break;

						case 30:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/30-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/30-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/30-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/30-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/30-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/30-s.png") as ImageSource;
							break;

						case 31:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/31-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/31-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/31-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/31-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/31-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/31-s.png") as ImageSource;
							break;

						case 32:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/32-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/32-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/32-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/32-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/32-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/32-s.png") as ImageSource;
							break;

						case 33:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/33-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/33-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/33-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/33-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/33-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/33-s.png") as ImageSource;
							break;

						case 34:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/34-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/34-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/34-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/34-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/34-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/34-s.png") as ImageSource;
							break;

						case 35:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/35-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/35-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/35-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/35-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/35-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/35-s.png") as ImageSource;
							break;

						case 36:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/36-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/36-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/36-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/36-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/36-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/36-s.png") as ImageSource;
							break;

						case 37:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/37-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/37-s.png") as ImageSource;
							else if (i == 4)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/37-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/37-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/37-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/37-s.png") as ImageSource;
							break;

						case 38:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/38-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/38-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/38-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/38-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/38-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/38-s.png") as ImageSource;
							break;

						case 39:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/39-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/39-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/39-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/39-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/39-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/39-s.png") as ImageSource;
							break;

						case 40:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/40-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/40-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/40-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/40-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/40-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/40-s.png") as ImageSource;
							break;

						case 41:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/41-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/41-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/41-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/41-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/41-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/41-s.png") as ImageSource;
							break;

						case 42:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/42-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/42-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/42-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/42-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/42-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/42-s.png") as ImageSource;
							break;

						case 43:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/43-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/43-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/43-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/43-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/43-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/43-s.png") as ImageSource;
							break;

						case 44:
							if (i == 0)
								iconFirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/44-s.png") as ImageSource;
							else if (i == 2)
								iconSecond.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/44-s.png") as ImageSource;
							else if (i == 4)
								iconThirst.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/44-s.png") as ImageSource;
							else if (i == 6)
								iconFourth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/44-s.png") as ImageSource;
							else if (i == 8)
								iconFifth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/44-s.png") as ImageSource;
							else if (i == 10)
								iconSixth.Source = (new ImageSourceConverter()).ConvertFromString("https://developer.accuweather.com/sites/default/files/44-s.png") as ImageSource;

							break;
					}
				}
			}

		}

	}

}

