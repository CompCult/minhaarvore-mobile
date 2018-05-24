﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PostCard : MonoBehaviour
{
	#pragma warning disable 0108

	public RawImage profilePic, imagePost;
	public Text authorName, date, message, likes;

	public Post post;
	public GameObject loadingHolder, likeButton;

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
		User currentUser = UserService.user;

		if (currentUser._id == post._user)
			likeButton.SetActive(false);
		else
			likeButton.SetActive(true);

		authorName.text = post.author_name;
		date.text = UtilsService.GetDate(post.created_at);
		message.text = post.text_msg;
		likes.text = post.points.ToString();

		StartCoroutine(_GetAuthorPhoto());
		StartCoroutine(_GetPostImage());
	}

	private IEnumerator _GetAuthorPhoto ()
	{
		string photoUrl = post.author_photo;
		Texture2D texture;

		if (photoUrl == null || photoUrl.Length < 1)
		{
			texture = UtilsService.GetDefaultPhoto();
		}
		else
		{
			Debug.Log ("current author photo url is " + photoUrl);
			var www = new WWW(photoUrl);
			yield return www;

			texture = UtilsService.ResizeTexture(www.texture, "Average", 0.25f);
		}

		if (texture == null)
			texture = UtilsService.GetDefaultPhoto();

		profilePic.texture = texture;
	}

	private IEnumerator _GetPostImage ()
    {
    	string photoUrl = post.picture;
    	Texture2D texture;

		if (photoUrl == null || photoUrl.Length < 1)
			yield break;

		Debug.Log ("current post photo url is " + photoUrl);
		var www = new WWW(photoUrl);
		yield return www;

		if (www.responseHeaders["STATUS"] != HTML.HTTP_200)
			yield break;

		texture = UtilsService.ResizeTexture(www.texture, "Average", 0.25f);

		if (texture == null)
		{
			Debug.LogError("Failed to load texture url: " + photoUrl);
			yield break;
		}

		Destroy(loadingHolder);
		imagePost.texture = texture;
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
