using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : ScreenController 
{
	public void Start ()
	{
		previousView = "Home";
	}

	public void SaveChanges ()
	{
		StartCoroutine(_SaveChanges());
	}

	private IEnumerator _SaveChanges ()
	{
		AlertsService.makeLoadingAlert("Salvando");
		yield return null;
	}

}
