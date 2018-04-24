using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : ScreenController 
{
	public InputField emailField, passwordField;

	public void Start ()
	{
		CheckAuthenticatedUser();
	}

	private void CheckAuthenticatedUser ()
	{
		if (PlayerPrefs.HasKey("MinhaArvore:Conectar"))
		{
			emailField.text = PlayerPrefs.GetString("MinhaArvore:Email");
			passwordField.text = PlayerPrefs.GetString("MinhaArvore:Senha");

			Authenticate();
		}
	}

	private void SaveUser ()
	{
		PlayerPrefs.SetString("MinhaArvore:Conectar", "Sim");
		PlayerPrefs.SetString("MinhaArvore:Email", emailField.text);
		PlayerPrefs.SetString("MinhaArvore:Senha", passwordField.text);
	}

	public void Authenticate ()
	{
		AlertsService.makeLoadingAlert("Autenticando");

		StartCoroutine(_Authenticate());
	}

	private IEnumerator _Authenticate ()
	{
		WWW loginRequest = UserService.Login(emailField.text, passwordField.text);

		while (!loginRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + loginRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + loginRequest.text);
		AlertsService.removeLoadingAlert();

		if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			SaveUser();
			UserService.UpdateLocalUser(loginRequest.text);
			
			LoadView("Home");
		}
		else 
		{
			if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_400)
				AlertsService.makeAlert("Senha incorreta", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else if (loginRequest.responseHeaders["STATUS"] == HTML.HTTP_404)
				AlertsService.makeAlert("Usuário não encontrado", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else
				AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private bool CheckFields ()
	{
		return UtilsService.CheckEmail(emailField.text) && 
			   UtilsService.CheckPassword(passwordField.text);
	}
}
