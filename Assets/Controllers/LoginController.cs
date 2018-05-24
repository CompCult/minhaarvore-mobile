using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : ScreenController 
{
	public InputField emailField, passwordField;
	public Button loginButton, registerButton;

	private string IN_MAINTENANCE = "true";

	public void Start ()
	{
		loginButton.interactable = false;
		registerButton.interactable = false;

		StartCoroutine(_CheckMaintenance());
	}

	private void CheckAuthenticatedUser ()
	{
		if (PlayerPrefs.HasKey("MinhaArvore:Email"))
			emailField.text = PlayerPrefs.GetString("MinhaArvore:Email");
	}

	private void SaveUser ()
	{
		PlayerPrefs.SetString("MinhaArvore:Email", emailField.text);
	}

	public void Authenticate ()
	{
		AlertsService.makeLoadingAlert("Autenticando");
		StartCoroutine(_Authenticate());
	}

	private IEnumerator _CheckMaintenance ()
	{
		WWW checkRequest = SystemService.CheckMaintenance();

		while (!checkRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + checkRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + checkRequest.text);

		if (checkRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			if (checkRequest.text == IN_MAINTENANCE)
			{
				AlertsService.makeAlert("EM MANUTENÇÃO", "O Minha Árvore está em manutenção no momento. Por favor, tente novamente mais tarde.", "Entendi");
			}
			else
			{
				loginButton.interactable = true;
				registerButton.interactable = true;
				CheckAuthenticatedUser();
			}
		}
		else 
		{
			AlertsService.makeAlert("FALHA NA CONEXÃO", "Houve uma falha na conexão com o Minha Árvore. Por favor, tente novamente mais tarde.", "OK");
		}

		yield return null;
	}

	private IEnumerator _Authenticate ()
	{
		WWW loginRequest = UserService.Login(emailField.text, passwordField.text);

		while (!loginRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + loginRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + loginRequest.text);

		if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			SaveUser();
			UserService.UpdateLocalUser(loginRequest.text);
			yield return StartCoroutine(_GetUserPhoto());
		}
		else 
		{
			AlertsService.removeLoadingAlert();

			if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_400)
				AlertsService.makeAlert("Senha incorreta", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_404 || loginRequest.responseHeaders["STATUS"] == HTML.HTTP_401)
				AlertsService.makeAlert("Usuário não encontrado", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else
				AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private IEnumerator _GetUserPhoto ()
	{
		string photoUrl = UserService.user.picture;
		Texture2D texture;

		if (photoUrl == null || photoUrl.Length < 1)
		{
			texture = UtilsService.GetDefaultProfilePhoto();
		}
		else
		{
			var www = new WWW(photoUrl);
			yield return www;

			texture = UtilsService.ResizeTexture(www.texture, "Average", 0.25f);
		}

		if (texture != null)
			UserService.user.profilePicture = texture;

		UserService.user.profilePicture = texture;
		LoadView("Home");
	}

	private bool CheckFields ()
	{
		return UtilsService.CheckEmail(emailField.text) && 
			   UtilsService.CheckPassword(passwordField.text);
	}
}
