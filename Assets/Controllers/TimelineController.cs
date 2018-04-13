using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineController : ScreenController 
{
	public InputField nameField, plantsField, leavesField;

	public void Start ()
	{
		previousView = "Home";
		GetTimelinePosts ();
	}

	public void GetMorePosts ()
	{
		StartCoroutine(_GetMorePosts());
	}

	private void GetTimelinePosts ()
	{
		AlertsService.makeLoadingAlert("Recebendo postagens");

		StartCoroutine(_GetTimelinePosts());
	}

	private IEnumerator _GetTimelinePosts ()
	{
		yield return null;
	}

	private IEnumerator _GetMorePosts ()
	{
		yield return null;
	}
}
