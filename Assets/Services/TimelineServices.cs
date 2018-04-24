using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public static class TimelineServices
{
	private static Post[] _posts;
	public static Post[] posts
	{
        get { return _posts; }
    }

    public static WWW NewPost (int userID, Texture2D image, string message)
	{
		WWWForm postForm = new WWWForm ();
		postForm.AddField ("_user", userID);
		postForm.AddField ("text", message);
		postForm.AddBinaryData("image", ImageConversion.EncodeToJPG(image), "photo.jpg", "image/jpeg");

		WebService.route = ENV.POSTS_ROUTE;
		WebService.action = "";

		return WebService.Post(postForm);
	}

	public static WWW GetTimelinePosts ()
	{
		WebService.route = ENV.POSTS_ROUTE;
		WebService.action = "";

		return WebService.Get();
	}

	public static void UpdateLocalPosts (Post[] posts)
	{
		_posts = posts;
	}

	public static void UpdateLocalPosts (string json)
	{
		_posts = UtilsService.GetJsonArray<Post>(json);
	}

}
