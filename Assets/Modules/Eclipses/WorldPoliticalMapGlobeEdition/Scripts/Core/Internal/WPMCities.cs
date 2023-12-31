﻿// World Political Map - Globe Edition for Unity - Main Script
// Created by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WPM


using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Module.Eclipses
{

    public partial class WorldMapGlobe : MonoBehaviour {

		const float CITY_HIT_PRECISION = 0.001f;
		// 0.00085f;

		#region Internal variables

		List<City> _cities;

		// resources
		Material citiesNormalMat, citiesRegionCapitalMat, citiesCountryCapitalMat;
		GameObject citiesLayer, citySpot, citySpotCapitalRegion, citySpotCapitalCountry;

		#endregion

		// internal cache
		City[] visibleCities;
		
		/// <summary>
		/// City look up dictionary. Used internally for fast searching of city objects.
		/// </summary>
		Dictionary<City, int> _cityLookup;
		int lastCityLookupCount = -1;

		Dictionary<City, int>cityLookup {
			get {
				if (_cityLookup != null && cities.Count == lastCityLookupCount)
					return _cityLookup;
				if (_cityLookup == null) {
					_cityLookup = new Dictionary<City,int> ();
				} else {
					_cityLookup.Clear ();
				}
				if (cities != null) {
					int cityCount = cities.Count;
					for (int k = 0; k < cityCount; k++)
						_cityLookup [cities [k]] = k;
				}
				lastCityLookupCount = _cityLookup.Count;
				return _cityLookup;
			}
		}


		#region System initialization

		void ReadCitiesPackedString () {
			string cityCatalogFileName = geodataFolderName + "/cities10";
			TextAsset ta = Resources.Load<TextAsset> (cityCatalogFileName);
			string s = ta.text;
			ReadCitiesPackedString (s);
		}

		/// <summary>
		/// Reads the cities data from a packed string.
		/// </summary>
		void ReadCitiesPackedString (string s) {

			if (_countries == null) {
				Init ();
				if (_cities != null)
					return;
			}

			string[] cityList = s.Split (new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			int cityCount = cityList.Length;
			cities = new List<City> (cityCount);
			int citiesCount = 0;
			for (int k = 0; k < cityCount; k++) {
				string[] cityInfo = cityList [k].Split (new char[] { '$' });
				string country = cityInfo [2];
				int countryIndex = GetCountryIndex (country);
				if (countryIndex >= 0) {
					string name = cityInfo [0];
					string province = cityInfo [1];
					int population = int.Parse (cityInfo [3]);
					float x = float.Parse (cityInfo [4], Misc.InvariantCulture) / MAP_PRECISION;
					float y = float.Parse (cityInfo [5], Misc.InvariantCulture) / MAP_PRECISION;
					float z = float.Parse (cityInfo [6], Misc.InvariantCulture) / MAP_PRECISION;
					CITY_CLASS cityClass = (CITY_CLASS)int.Parse (cityInfo [7]);
					City city = new City (name, province, countryIndex, population, new Vector3 (x, y, z), cityClass);
					_cities.Add (city);
					if (cityClass == CITY_CLASS.COUNTRY_CAPITAL) {
						_countries [countryIndex].cityCapitalIndex = citiesCount;
					}
					citiesCount++;
				}
			}
		}

		#endregion

		#region Drawing stuff

		/// <summary>
		/// Redraws the cities. This is automatically called by Redraw(). Used internally by the Map Editor. You should not need to call this method directly.
		/// </summary>
		public void DrawCities () {

			if (!_showCities || !gameObject.activeInHierarchy || cities == null)
				return;

			// Create cities layer
			Transform t = transform.Find ("Cities");
			if (t != null)
				DestroyImmediate (t.gameObject);
			citiesLayer = new GameObject ("Cities");
			citiesLayer.transform.SetParent (transform, false);
			if (_earthInvertedMode)
				citiesLayer.transform.localScale *= 0.99f;

			// Create cityclass parents
			GameObject countryCapitals = new GameObject ("Country Capitals");
			countryCapitals.hideFlags = HideFlags.DontSave;
			countryCapitals.transform.SetParent (citiesLayer.transform, false);
			GameObject regionCapitals = new GameObject ("Region Capitals");
			regionCapitals.hideFlags = HideFlags.DontSave;
			regionCapitals.transform.SetParent (citiesLayer.transform, false);
			GameObject normalCities = new GameObject ("Normal Cities");
			normalCities.hideFlags = HideFlags.DontSave;
			normalCities.transform.SetParent (citiesLayer.transform, false);
			bool combineMeshesActive = _combineCityMeshes && Application.isPlaying;
			float scale = CityScaler.GetScale (this);
			Vector3 cityScale = new Vector3 (scale, scale, 1f);

			// Draw city marks
			_numCitiesDrawn = 0;
			int minPopulation = _minPopulation * 1000;
			int visibleCount = 0;

			// flip localscale.x to prevent transform issues
			if (_earthInvertedMode)
				transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

			int cityCount = cities.Count;
			for (int k = 0; k < cityCount; k++) {
				City city = cities [k];
				Country country = countries [city.countryIndex];
				city.isShown = !country.hidden && ((((int)city.cityClass & _cityClassAlwaysShow) != 0) || (minPopulation == 0 || city.population >= minPopulation));
				if (city.isShown) {
					GameObject cityObj, cityParent;
					switch (city.cityClass) {
					case CITY_CLASS.COUNTRY_CAPITAL: 
						cityObj = Instantiate (citySpotCapitalCountry); 
						if (!combineMeshesActive) {
							cityObj.GetComponent<Renderer> ().sharedMaterial = citiesCountryCapitalMat;
						}
						cityParent = countryCapitals;
						break;
					case CITY_CLASS.REGION_CAPITAL: 
						cityObj = Instantiate (citySpotCapitalRegion); 
						if (!combineMeshesActive) {
							cityObj.GetComponent<Renderer> ().sharedMaterial = citiesRegionCapitalMat;
						}
						cityParent = regionCapitals;
						break;
					default:
						cityObj = Instantiate (citySpot); 
						if (!combineMeshesActive) {
							cityObj.GetComponent<Renderer> ().sharedMaterial = citiesNormalMat;
						}
						cityParent = normalCities;
						break;
					}
					cityObj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
					cityObj.transform.SetParent (cityParent.transform, false);
					cityObj.transform.localPosition = city.unitySphereLocation;
					cityObj.transform.localScale = cityScale;
					if (_earthInvertedMode) {
						cityObj.transform.LookAt (transform.TransformPoint (city.unitySphereLocation * 2f));
					} else {
						cityObj.transform.LookAt (transform.position);
					}
					city.gameObject = cityObj;
					_numCitiesDrawn++;
					visibleCount++;
				} else {
					city.gameObject = null;
				}
			}

			if (_earthInvertedMode)
				transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

			// Cache visible cities (this is faster than iterate through the entire collection)
			if (visibleCities == null || visibleCities.Length != visibleCount)
				visibleCities = new City[visibleCount];

			for (int k = 0; k < cityCount; k++) {
				City city = cities [k];
				if (city.isShown)
					visibleCities [--visibleCount] = city;
			}

			// Toggle cities layer visibility according to settings
			citiesLayer.SetActive (_showCities);

			CityScaler cityScaler = citiesLayer.GetComponent<CityScaler> () ?? citiesLayer.AddComponent<CityScaler> ();
			cityScaler.map = this;
			cityScaler.ScaleCities ();

			if (combineMeshesActive) {
				DestroyImmediate (cityScaler);
				CombineMeshes (normalCities, citiesNormalMat);
				CombineMeshes (regionCapitals, citiesRegionCapitalMat);
				CombineMeshes (countryCapitals, citiesCountryCapitalMat);
			}
		}

		void CombineMeshes (GameObject obj, Material mat) {
			obj.transform.SetParent (null);
			obj.transform.localScale = Vector3.one;
			obj.transform.localRotation = Misc.QuaternionZero; //Quaternion.Euler(0,0,0);
			obj.transform.position = Vector3.zero;
			MeshFilter[] meshFilters = obj.transform.GetComponentsInChildren<MeshFilter> ();
			CombineInstance[] combine = new CombineInstance[meshFilters.Length];
			int i = 0;
			while (i < meshFilters.Length) {
				combine [i].mesh = meshFilters [i].sharedMesh;
				combine [i].transform = meshFilters [i].transform.localToWorldMatrix;
				meshFilters [i].gameObject.SetActive (false);
				i++;
			}
			MeshFilter mf = obj.AddComponent<MeshFilter> ();
			mf.mesh = new Mesh ();
			mf.sharedMesh.hideFlags = HideFlags.DontSave;
			mf.sharedMesh.CombineMeshes (combine);
			Renderer renderer = obj.AddComponent<MeshRenderer> ();
			renderer.receiveShadows = false;
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.sharedMaterial = mat;
			obj.transform.SetParent (citiesLayer.transform, false);
		}

		void HighlightCity (int cityIndex) {
			if (cityIndex == _cityHighlightedIndex)
				return;
			_cityHighlightedIndex = cityIndex;
			_cityHighlighted = cities [cityIndex];

			// Raise event
			if (OnCityEnter != null)
				OnCityEnter (_cityHighlightedIndex);
		}

		void HideCityHighlight () {
			if (_cityHighlightedIndex < 0)
				return;

			// Raise event
			if (OnCityExit != null)
				OnCityExit (_cityHighlightedIndex);
			_cityHighlighted = null;
			_cityHighlightedIndex = -1;
		}

		#endregion

		#region Internal Cities API

		/// <summary>
		/// Returns any city near the point specified in sphere coordinates. Must be closer than CITY_HIT_PRECISION constant, usually when pointer clicks over a city icon.
		/// Optimized method for mouse hitting.
		/// </summary>
		/// <returns>The city near point.</returns>
		/// <param name="localPoint">Local point in sphere coordinates.</param>
		public int GetCityNearPointFast (Vector3 localPoint) {
			if (visibleCities == null)
				return -1;
			float hitPrecision = CITY_HIT_PRECISION * _cityIconSize * 5.0f;
			float hitPrecisionSqr = hitPrecision * hitPrecision;
			Vector2 latlon = Conversion.GetLatLonFromSpherePoint (localPoint);
			for (int c = 0; c < countries.Length; c++) {
				Country country = countries [c];
				if (country.regionsRect2D.Contains (latlon)) {
					for (int t = 0; t < visibleCities.Length; t++) {
						City city = visibleCities [t];
						if (city.countryIndex != c)
							continue;
						float dist = FastVector.SqrDistance (ref city.unitySphereLocation, ref localPoint); // (city.unitySphereLocation - localPoint).sqrMagnitude;
						if (dist < hitPrecisionSqr) {
							return GetCityIndex (city, false);
						}
					}
				}
			}
			return -1;
		}

		bool GetCityUnderMouse (int countryIndex, Vector3 localPoint, out int cityIndex) {
			cityIndex = -1;
			if (visibleCities == null)
				return false;
			float hitPrecision = CITY_HIT_PRECISION * _cityIconSize * 5.0f;
			float hitPrecisionSqr = hitPrecision * hitPrecision;
			for (int c = 0; c < visibleCities.Length; c++) {
				City city = visibleCities [c];
				if (city.countryIndex == countryIndex && city.isShown) {
//																				if ((city.unitySphereLocation - localPoint).sqrMagnitude < hitPrecisionSqr) {
					if (FastVector.SqrDistance (ref city.unitySphereLocation, ref localPoint) < hitPrecisionSqr) {
						cityIndex = GetCityIndex (city, false);
						return true;
					}
				}
			}
			return false;
		}


		/// <summary>
		/// Returns cities belonging to a provided country.
		/// </summary>
		public List<City>GetCities (int countryIndex) {
			List<City> results = new List<City> (20);
			int cityCount = cities.Count;
			for (int c = 0; c < cityCount; c++) {
				if (cities [c].countryIndex == countryIndex)
					results.Add (cities [c]);
			}
			return results;
		}

		/// <summary>
		/// Returns cities enclosed by a region.
		/// </summary>
		public List<City>GetCities (Region region) {
			List<City> results = new List<City> (20);
			int cityCount = cities.Count;
			for (int c = 0; c < cityCount; c++) {
				if (region.Contains (_cities [c].latlon)) {
					results.Add (cities [c]);
				}
			}
			return results;
		}


		/// <summary>
		/// Updates the city scale.
		/// </summary>
		public void ScaleCities () {
			if (citiesLayer != null) {
				CityScaler scaler = citiesLayer.GetComponent<CityScaler> ();
				if (scaler != null) {
					scaler.ScaleCities ();
				} else {
					DrawCities ();
				}
			}
		}


		int GetCityCountryRegionIndex (City city) {
			if (city.regionIndex < 0) {
				int countryIndex = city.countryIndex;
				if (countryIndex < 0 || countryIndex > countries.Length)
					return -1;

				Country country = countries [countryIndex];
				if (country.regions == null)
					return -1;
				int regionCount = country.regions.Count;

				float minDist = float.MaxValue;
				for (int cr = 0; cr < regionCount; cr++) {
					Region region = country.regions [cr];
					if (region == null || region.spherePoints == null)
						continue;
					for (int p = 0; p < region.spherePoints.Length; p++) {
						float dist = (region.spherePoints [p] - city.unitySphereLocation).sqrMagnitude;
						if (dist < minDist) {
							minDist = dist;
							city.regionIndex = cr;
						}
					}
				}
			}
			return city.regionIndex;
		}



		#endregion
	}

}