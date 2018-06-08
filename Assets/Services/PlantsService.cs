using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class PlantsService
{
	private static PlantType _type;
	public static PlantType type { get { return _type; } }

	private static PlantType[] _types;
	public static PlantType[] types { get { return _types; } }

	private static Plant[] _plants;
	public static Plant[] plants { get { return _plants; } }

	private static Plant _plant;
	public static Plant plant { get { return _plant; } }

	public static WWW RequestTree (int typeID, string photoBase64, string plantName, string requesterName, string requesterPhone, string placeName, string sidewalkSize, int quantity, 
								   string street, string number, string neighborhood, string city, string state, string complement, string zipcode, string locationType)
	{
		Debug.Log("photo: " + photoBase64.Substring(0, 32));
		Debug.Log("_user: " + UserService.user._id);
		Debug.Log("_type: " + typeID);
		Debug.Log("tree_name: " + plantName);
		Debug.Log("requester_name: " + requesterName);
		Debug.Log("requester_phone: " + requesterPhone);
		Debug.Log("place: " + placeName);
		Debug.Log("quantity: " + quantity);

		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("_user", UserService.user._id);
		requestForm.AddField ("_type", typeID);
		requestForm.AddField ("photo", photoBase64);
		requestForm.AddField ("tree_name", plantName);
		requestForm.AddField ("requester_name", requesterName);
		requestForm.AddField ("requester_phone", requesterPhone);
		requestForm.AddField ("quantity", quantity);
		requestForm.AddField ("place", placeName);

		if (locationType == "GPS")
		{
			Debug.Log("location_lat: " + GPSService.location[0].ToString());
			Debug.Log("location_lng: " + GPSService.location[1].ToString());

			requestForm.AddField ("location_lat", GPSService.location[0].ToString());
			requestForm.AddField ("location_lng", GPSService.location[1].ToString());
		}
		else
		{
			Debug.Log("street: " + street);
			Debug.Log("number: " + number);
			Debug.Log("neighborhood: " + neighborhood);
			Debug.Log("city: " + city);
			Debug.Log("state: " + state);
			Debug.Log("complement: " + complement);
			Debug.Log("zipcode: " + zipcode);

			requestForm.AddField ("street", street);
			requestForm.AddField ("number", number);
			requestForm.AddField ("neighborhood", neighborhood);
			requestForm.AddField ("city", city);
			requestForm.AddField ("state", state);
			requestForm.AddField ("complement", complement);
			requestForm.AddField ("zipcode", zipcode);
		}

		if (placeName == "Calçada")
		{
			Debug.Log("sidewalk_size: " + sidewalkSize);
			requestForm.AddField ("sidewalk_size", sidewalkSize);
		}

		WebService.route = ENV.PLANTS_REQUEST_ROUTE;
		WebService.action = "";

		return WebService.Post(requestForm);
	}

	public static WWW GetPlantTypes ()
	{
		WebService.route = ENV.PLANTS_TYPES_ROUTE;
		WebService.action = "";

		return WebService.Get();
	}

	public static WWW GetUserPlants (int userID)
	{
		WebService.route = ENV.PLANTS_ROUTE;
		WebService.action = ENV.QUERY_ACTION +
							"_user=" + userID;

		return WebService.Get();
	}

	public static void UpdateLocalPlant (Plant plant)
	{
		_plant = plant;
	}

	public static void UpdateLocalPlants (string json)
	{
		_plants = UtilsService.GetJsonArray<Plant>(json);
	}

	public static void UpdateLocalPlantTypes (string json)
	{
		_types = UtilsService.GetJsonArray<PlantType>(json);
	}

	public static void UpdateLocalPlantType (PlantType type)
	{
		_type = type;
	}

	public static List<string> GetTypeNames ()
	{
		List<string> typeNames = new List<string>();
		foreach (PlantType type in _types)
		{
			Debug.Log("Added: " + type.name);
			typeNames.Add(type.name);
		}

		return typeNames;
	}
}
