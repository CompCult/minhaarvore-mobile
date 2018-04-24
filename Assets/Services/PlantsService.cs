using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class PlantsService
{
	private static PlantType[] _types;
	public static PlantType[] types { get { return _types; } }

	private static Plant[] _plants;
	public static Plant[] plants { get { return _plants; } }

	public static WWW RequestTree (int typeID, byte[] photoArray, string plantName, string requesterName, string placeName, string sidewalkSize, int quantity)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("_user", UserService.user._id);
		requestForm.AddField ("_type", typeID);
		requestForm.AddBinaryData ("photo", photoArray, "Photo.png", "image/png");
		requestForm.AddField ("tree_name", plantName);
		requestForm.AddField ("requester_name", requesterName);
		requestForm.AddField ("place", placeName);
		requestForm.AddField ("quantity", quantity);
		requestForm.AddField ("location_lat", GPSService.location[0].ToString());
		requestForm.AddField ("location_lng", GPSService.location[1].ToString());
		if (sidewalkSize != null)
			requestForm.AddField ("sidewalk_size", sidewalkSize);

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
							"user=" + userID;

		return WebService.Get();
	}

	public static void UpdateLocalPlants (string json)
	{
		_plants = UtilsService.GetJsonArray<Plant>(json);
	}

	public static void UpdateLocalPlantTypes (string json)
	{
		_types = UtilsService.GetJsonArray<PlantType>(json);
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
