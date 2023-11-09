using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Module.Eclipses
{

    public enum CITY_CLASS {
		CITY = 1,
		REGION_CAPITAL = 2,
		COUNTRY_CAPITAL = 4
	}

	public partial class City {
		public string name;
		public int countryIndex;

		public int regionIndex = -1;
			

		public string province;
		public Vector3 unitySphereLocation;
		public int population;
		public CITY_CLASS cityClass;
		
		/// <summary>
		/// Reference to the city icon drawn over the globe.
		/// </summary>
		public GameObject gameObject;
		
		/// <summary>
		/// Returns if city is visible on the map based on minimum population filter.
		/// </summary>
		public bool isShown;

		public float latitude {
			get {
				return latlon.x;
			}
		}

		public float longitude {
			get {
				return latlon.y;
			}
		}

		Vector2 _latlon;

		public Vector2 latlon {
			get {
				if (latlonPending) {
					latlonPending = false;
					_latlon = Conversion.GetLatLonFromSpherePoint (unitySphereLocation);
				}
				return _latlon;
			}
			set {
				if (value != _latlon) {
					_latlon = value;
					unitySphereLocation = Conversion.GetSpherePointFromLatLon (_latlon);
				}
			}
		}

		bool latlonPending;

		public City (string name, string province, int countryIndex, int population, Vector3 location, CITY_CLASS cityClass) {
			this.name = name;
			this.province = province;
			this.countryIndex = countryIndex;
			this.population = population;
			this.unitySphereLocation = location;
			this.cityClass = cityClass;
			this.latlonPending = true;
		}

		public City Clone () {
			City c = new City (name, province, countryIndex, population, unitySphereLocation, cityClass);
			return c;
		}


	}
}