using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypesController : MonoBehaviour 
{
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
				UpdateList(placeList, type.GetPlaceList());
				break;
			}
	}

	public void CheckSidewalk ()
	{
		if (placeList.captionText.text == "Calçada")
			sideWalkSizeObj.SetActive(true);
		else
			sideWalkSizeObj.SetActive(false);
	}

	private void FillPlantTypes ()
	{
		List<string> plantTypes = PlantsService.GetTypeNames(),
					 initialPlaces = PlantsService.types[0].GetPlaceList();
		
		UpdateList(typeList, plantTypes);
		UpdateList(placeList, initialPlaces);
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
