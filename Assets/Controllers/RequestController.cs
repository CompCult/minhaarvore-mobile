using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestController : ScreenController 
{
	public void Start ()
	{
		previousView = "Plants";
	}

	public void RequestPlant ()
	{
		StartCoroutine(_RequestPlant());
	}

	private IEnumerator _RequestPlant ()
	{
		AlertsService.makeLoadingAlert("Enviando");
		yield return null;
	}

}
