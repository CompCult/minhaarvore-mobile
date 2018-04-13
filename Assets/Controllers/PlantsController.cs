using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantsController : ScreenController 
{
	public void Start ()
	{
		previousView = "Login";
		GetPlants ();
	}

	public void GetPlants ()
	{
		StartCoroutine(_GetPlants());
	}

	private IEnumerator _GetPlants ()
	{
		AlertsService.makeLoadingAlert("Recebendo");
		yield return null;
	}

}
