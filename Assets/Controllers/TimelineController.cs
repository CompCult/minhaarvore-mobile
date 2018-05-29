using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimelineController : ScreenController 
{
	public GameObject postCard, noPostsCard;

	public void Start ()
	{
		TutorialService.CheckTutorial("Timeline");

		previousView = "Home";

		postCard.SetActive(false);
		noPostsCard.SetActive(false);

		StartCoroutine(_GetTimelinePosts());
	}

	private IEnumerator _GetTimelinePosts ()
	{
		AlertsService.makeLoadingAlert("Recebendo postagens");
		WWW postsRequest = TimelineService.GetTimelinePosts();

		while (!postsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + postsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + postsRequest.text);

		if (postsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			TimelineService.UpdateLocalPosts(postsRequest.text);
			CreatePostsCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Home");
		}

		yield return null;
	}

	private void CreatePostsCards ()
    {
     	Vector3 position = postCard.transform.position;

     	if (TimelineService.posts.Length > 0)
     	{
     		postCard.SetActive(true);
     		noPostsCard.SetActive(false);
     	}
     	else
     	{
     		postCard.SetActive(false);
     		noPostsCard.SetActive(true);
     	}

     	foreach (Post post in TimelineService.posts)
        {
            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(postCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	PostCard postCardScript = card.GetComponent<PostCard>();
        	postCardScript.UpdatePost(post);
        }

        postCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }
}
