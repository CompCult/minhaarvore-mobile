using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ModalShowImage : ModalGeneric 
{
	public GameObject loadingHolder;
	public Image image;

	private string location_lat, location_lng,
				   PLANTS_VIEW = "Plants",
				   SUBMISSIONS_VIEW = "Submissions";

	public void Start ()
	{
		string url, location;

		if (GetViewName() == PLANTS_VIEW)
		{
			Plant currentPlant = PlantsService.plant;
			location = currentPlant._request.GetAddress();

			url = ENV.GOOGLE_MAPS_COORD_URL.Replace("PLACE", location);
		}
		else if (GetViewName() == SUBMISSIONS_VIEW)
		{
			MissionAnswer currentAnswer = MissionsService.missionAnswer;
			location_lat = (currentAnswer.location_lat != null ? currentAnswer.location_lat : "0");
			location_lng = (currentAnswer.location_lng != null ? currentAnswer.location_lng : "0");
			location = location_lat + "," + location_lng;

			url = ENV.GOOGLE_MAPS_COORD_URL.Replace("PLACE", location);
		}
		else
		{
			PlantType plantType = PlantsService.type;
			url = plantType.photo;
		}

		if (url != null)
			StartCoroutine(_ShowImage(url));
	}

	private IEnumerator _ShowImage (string url)
    {
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

		    image.sprite = sprite;
		    Destroy(loadingHolder);
		}
    }
}
