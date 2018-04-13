using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelinePostsController : ScreenController 
{
	public InputField messageField;

	public void MakePost ()
	{
		AlertsService.makeLoadingAlert("Publicando");

		StartCoroutine(_MakePost());
	}

	private IEnumerator _MakePost ()
	{
		yield return null;
	}

	private bool CheckFields ()
	{
		return messageField.text.Length >= 2;
	}
}
