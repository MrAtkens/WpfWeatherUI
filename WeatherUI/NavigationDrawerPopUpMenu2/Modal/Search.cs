﻿using System.Collections.Generic;


namespace NavigationDrawerPopUpMenu2
{
	public class Region
	{
		public string ID { get; set; }
		public string LocalizedName { get; set; }
		public string EnglishName { get; set; }
	}

	public class Country
	{
		public string ID { get; set; }
		public string LocalizedName { get; set; }
		public string EnglishName { get; set; }
	}

	public class AdministrativeArea
	{
		public string ID { get; set; }
		public string LocalizedName { get; set; }
		public string EnglishName { get; set; }
		public int Level { get; set; }
		public string LocalizedType { get; set; }
		public string EnglishType { get; set; }
		public string CountryID { get; set; }
	}

	public class TimeZone
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public int GmtOffset { get; set; }
		public bool IsDaylightSaving { get; set; }
		public object NextOffsetChange { get; set; }
	}

	public class Metric
	{
		public int Value { get; set; }
		public string Unit { get; set; }
		public int UnitType { get; set; }
	}

	public class Imperial
	{
		public int Value { get; set; }
		public string Unit { get; set; }
		public int UnitType { get; set; }
	}

	public class Elevation
	{
		public Metric Metric { get; set; }
		public Imperial Imperial { get; set; }
	}

	public class GeoPosition
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public Elevation Elevation { get; set; }
	}

	public class SearchData
	{
		public int Version { get; set; }
		public string Key { get; set; }
		public string Type { get; set; }
		public int Rank { get; set; }
		public string LocalizedName { get; set; }
		public string EnglishName { get; set; }
		public string PrimaryPostalCode { get; set; }
		public Region Region { get; set; }
		public Country Country { get; set; }
		public AdministrativeArea AdministrativeArea { get; set; }
		public bool IsAlias { get; set; }
		public List<object> SupplementalAdminAreas { get; set; }
		public List<object> DataSets { get; set; }
	}
}
