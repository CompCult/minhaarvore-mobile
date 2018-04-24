using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineController : ScreenController 
{
	// Timeline Post Card
	public GameObject postCard, noPostsCard;
	public Image img;
	public Text authorName, date, message, likes;

	// New post
	public InputField newMessage;
	public Texture2D image;

	public void Start ()
	{
		previousView = "Home";
		//StartCoroutine(_GetTimelinePosts());
	}

	public void AlertNoContent ()
	{
		AlertsService.makeAlert("Ambiente fechado", "Você está em um ambiente de testes. Por enquanto, não autorizamos postagens na linha do tempo neste ambiente.", "Entendi");
	}

	private IEnumerator _GetTimelinePosts ()
	{
		AlertsService.makeLoadingAlert("Recebendo postagens");
		WWW postsRequest = TimelineServices.GetTimelinePosts();

		while (!postsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + postsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + postsRequest.text);
		AlertsService.removeLoadingAlert();

		if (postsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			TimelineServices.UpdateLocalPosts(postsRequest.text);
			CreatePostsCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
			LoadView("Home");
		}

		yield return null;
	}

	private void CreatePostsCards ()
    {
     	Vector3 position = postCard.transform.position;

     	if (PlantsService.plants.Length > 0)
     	{
     		postCard.SetActive(true);
     		noPostsCard.SetActive(false);
     	}
     	else
     	{
     		postCard.SetActive(false);
     		noPostsCard.SetActive(true);
     	}

     	foreach (Post post in TimelineServices.posts)
        {
        	date.text = post.created_at;
        	likes.text = post.points.ToString();
        	message.text = post.text;

            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(postCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);
        }

        postCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }
}
