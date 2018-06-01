using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypesController : MonoBehaviour 
{
	public Button showTypePhoto;
	public Dropdown typeList, placeList;
	public GameObject sideWalkSizeObj;

	public void Start () 
	{
		FillPlantTypes();
	}
	
	public void UpdatePlaces () 
	{
		string newType = typeList.captionText.text;
		PlantType[] types = PlantsService.types;

		foreach (PlantType type in types)
			if (type.name == newType)
			{
				PlantsService.UpdateLocalPlantType(type);
				showTypePhoto.interactable = true;

				Debug.Log("type.GetPlaceList():" + type.GetPlaceList()[0]);

				UpdateList(placeList, type.GetPlaceList());
				break;
			}
	}

	public void CheckSidewalk ()
	{
		string currentPlace = placeList.captionText.text.ToLower();

		if (currentPlace.Contains("calçada"))
			sideWalkSizeObj.SetActive(true);
		else
			sideWalkSizeObj.SetActive(false);
	}

	private void FillPlantTypes ()
	{
		List<string> plantTypes = PlantsService.GetTypeNames(),
					 initialPlaces = PlantsService.types[0].GetPlaceList();

		Debug.Log("initialPlaces: " + initialPlaces[0]);
		
		UpdateList(typeList, plantTypes);
		UpdateList(placeList, initialPlaces);

		UpdatePlaces();
		CheckSidewalk();
	}

	private void UpdateList (Dropdown list, List<string> options)
	{
		list.ClearOptions();
		list.AddOptions(options);
		list.RefreshShownValue();

		CheckSidewalk();
	}
}
