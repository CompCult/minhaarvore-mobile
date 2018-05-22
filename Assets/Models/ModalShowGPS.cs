using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ModalShowGPS : ModalGeneric 
{
	public GameObject loadingHolder;
	public MissionAnswer currentAnswer;
	public Image mapImage;

	public void Start ()
	{
		currentAnswer = MissionsService.missionAnswer;
		StartCoroutine(_UpdateImage());
	}

	private IEnumerator _UpdateImage ()
    {
    	string latlong = currentAnswer.location_lat + "," + currentAnswer.location_lng;
    	string url = ENV.GOOGLE_MAPS_COORD_URL.Replace("PLACE", latlong);
    	
    	UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		www.SetRequestHeader("Accept", "image/*");
		
		var async = www.SendWebRequest();
		while (!async.isDone)
		    yield return null;

		if (!www.isNetworkError)
		{
		    yield return new WaitForEndOfFrame();

		    byte[] results = www.downloadHandler.data;
		    Texture2D texture = new Texture2D(100, 100);

		    texture.LoadImage(results);
		    Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

		    mapImage.sprite = sprite;
		    Destroy(loadingHolder);
		}
    }

}
