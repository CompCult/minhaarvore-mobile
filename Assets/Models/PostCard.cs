using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PostCard : MonoBehaviour
{
	#pragma warning disable 0108

	public Image image;
	public Text authorName, date, message, likes;

	public Post post;
	public GameObject loadingHolder;

	public void LikePost ()
	{
		if (UserService.user.points <= 0)
		{
			AlertsService.makeAlert("Sem folhas", "Você não tem folhas para dar. Plante mudas, participe de missões ou faça postagens para ganhar mais.", "OK");
			return;
		}

		bool UPDATE_USER_POINTS = true;
		StartCoroutine(_ChangePostPoints(1, UPDATE_USER_POINTS));
	}

	public void UpdatePost (Post post)
	{
		this.post = post;
		UpdateFields();
	}

	public void UpdateTextFields()
	{
		authorName.text = post.author_name;
		date.text = UtilsService.GetDate(post.created_at);
		message.text = post.text_msg;
		likes.text = post.points.ToString();
	}

	public void UpdateFields()
	{
		authorName.text = post.author_name;
		date.text = UtilsService.GetDate(post.created_at);
		message.text = post.text_msg;
		likes.text = post.points.ToString();

		StartCoroutine(UpdateImage());
	}

	private IEnumerator UpdateImage ()
    {
    	UnityWebRequest www = UnityWebRequestTexture.GetTexture(post.picture);
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

	private IEnumerator _ChangePostPoints (int newPoints, bool updateUser)
	{
		WWW likeRequest = TimelineService.UpdatePostPoints(post, newPoints);

		while (!likeRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + likeRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + likeRequest.text);

		if (likeRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			post.points += newPoints;

			if (updateUser)
				StartCoroutine(_ChangeUserPoints(-1 * newPoints));
		}

		UpdateTextFields();
	}

	private IEnumerator _ChangeUserPoints(int newPoints)
	{
		WWW pointsRequest = UserService.UpdatePoints(UserService.user, newPoints);

		while (!pointsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + pointsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + pointsRequest.text);

		if (pointsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UserService.user.points += newPoints;
		}
		else
		{
			bool DONT_UPDATE_USER_POINTS = false;
			StartCoroutine(_ChangePostPoints(-1, DONT_UPDATE_USER_POINTS));
		}
	}
}
