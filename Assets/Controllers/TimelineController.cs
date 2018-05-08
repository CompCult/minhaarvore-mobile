using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimelineController : ScreenController 
{
	// Timeline Post Card
	public GameObject newPostCard, postCard, noPostsCard;

	// New post
	public Image userPhoto;
	public Text userName;
	public InputField newMessageField;

	// Other
	public CameraCaptureService camService;
	public GameObject addButton, capturePhotoMenu;

	private string STATUS_OK = "OK";

	public void Start ()
	{
		previousView = "Home";
		userName.text = UserService.user.name;

		postCard.SetActive(false);
		newPostCard.SetActive(false);
		addButton.SetActive(true);

		camService.resetPreview("");
		StartCoroutine(_GetTimelinePosts());
	}

	public void ToggleNewPost()
	{
		if (newPostCard.activeSelf)
		{
			if (camService.photoBase64 != null)
				camService.resetPreview("");
			else
			{
				newPostCard.SetActive(false);
				addButton.SetActive(true);

				StopCoroutine(_CheckPhotoCaptured());
			}
		}
		else
		{
			StartCoroutine(_CheckPhotoCaptured());

			newPostCard.SetActive(true);
			addButton.SetActive(false);
		}
	}

	public void SendNewPost()
	{
		AlertsService.makeLoadingAlert("Enviando");

		string status = CheckFields();
		if (status != STATUS_OK)
		{
			AlertsService.removeLoadingAlert();
			AlertsService.makeAlert("Campos inválidos", status, "OK");
			return;
		}

		StartCoroutine(_SendNewPost());
	}

	private IEnumerator _CheckPhotoCaptured ()
	{
		if (camService.photoBase64 != null)
			capturePhotoMenu.SetActive(false);
		else
			capturePhotoMenu.SetActive(true);

		yield return new WaitForSeconds(1);
		yield return StartCoroutine(_CheckPhotoCaptured());
	}

	private IEnumerator _SendNewPost ()
	{
		int userId = UserService.user._id;
		string imageBase64 = camService.photoBase64,
			   message = newMessageField.text;

		WWW postForm = TimelineService.NewPost(userId, imageBase64, message);

		while (!postForm.isDone)
			yield return new WaitForSeconds(1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + postForm.responseHeaders["STATUS"]);
		Debug.Log("Text: " + postForm.text);

		if (postForm.responseHeaders["STATUS"] == HTML.HTTP_200)
			ReloadView();
		else
			AlertsService.makeAlert("Falha na conexão", "Ocorreu um problema ao enviar sua publicação. Tente novamente.", "Entendi");

		yield return null;
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
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
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

    private string CheckFields()
    {
    	string message = STATUS_OK;

    	if (camService.photoBase64 == null)
			message = "Você não selecionou uma foto para sua postagem.";

		if (newMessageField.text.Length < 2)
			message = "Escreva uma mensagem para sua publicação.";

		return message;
    }

}
