using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class WebService
{
	public static string route, action;
		
	#pragma warning disable 0219
	public static WWW Get()
	{
		string apiLink = ENV.API_URL + "/" + route + "/" + action;
		WWW www = new WWW (apiLink);

		Debug.Log("WebAPI - Get: " + apiLink);

		return www; 
	}

	#pragma warning disable 0219
	public static WWW Post(WWWForm form)
	{
		string apiLink = ENV.API_URL + "/" + route + "/" + action;

		WWW www = new WWW(apiLink, form);

		Debug.Log("WebAPI - Post: " + apiLink);

		return www;
	}
}