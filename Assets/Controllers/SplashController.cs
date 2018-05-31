using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashController : ScreenController 
{
	public GameObject loadingHolder;
	public Button reloadButton;
	private string IN_MAINTENANCE = "true";

	public void Start ()
	{
		PlaySound("click_1");

		loadingHolder.SetActive(true);
		reloadButton.gameObject.SetActive(false);

		StartCoroutine(_CheckMaintenance());
	}

	private IEnumerator _CheckMaintenance ()
	{
		WWW checkRequest = SystemService.CheckMaintenance();
		float timeLoading = 0f;

		while (!checkRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		if (timeLoading < 3.0f)
		{
			float remainingTime = 3.0f - timeLoading;
			yield return new WaitForSeconds(remainingTime);
		}

		Debug.Log("Header: " + checkRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + checkRequest.text);

		if (checkRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			if (checkRequest.text == IN_MAINTENANCE)
			{
				loadingHolder.SetActive(false);
				AlertsService.makeAlert("EM MANUTENÇÃO", "O Minha Árvore está em manutenção no momento. Por favor, tente novamente mais tarde.", "Entendi");
				reloadButton.gameObject.SetActive(true);
			}
			else
				LoadView("Login");
		}
		else 
		{
			loadingHolder.SetActive(false);
			AlertsService.makeAlert("FALHA NA CONEXÃO", "Houve uma falha na conexão com o Minha Árvore. Por favor, tente novamente mais tarde.", "OK");
			reloadButton.gameObject.SetActive(true);
		}

		yield return null;
	}
}
