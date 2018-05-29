using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypesController : MonoBehaviour 
{
	public Dropdown typeList, placeList;

	public void Start () 
	{
		UpdatePlaces();
	}
	
	public void UpdatePlaces () 
	{
		string newType = placeList.captionText.text;
		PlantType[] types = PlantsService.types;

		foreach (PlantType type in types)
			if (type.name == newType)
			{
				UpdatePlaceList(type.GetPlaceList());
				break;
			}

	}

	private void UpdatePlaceList(List<string> places)
	{
		placeList.AddOptions(places);
	}
}
