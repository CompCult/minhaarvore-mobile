using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : ScreenController 
{
	public InputField nameField, emailField, passwordField;

	public void Start()
	{
		previousView = "Login";
	}

	public void Register()
	{
		AlertsService.makeLoadingAlert("Registrando");

		StartCoroutine(_Register());
	}

	private IEnumerator _Register()
	{
		User newUser = new User();

		newUser.name = nameField.text;
		newUser.email = emailField.text;
		newUser.password = passwordField.text;

		WWW registerForm = UserService.Register(newUser);

		while (!registerForm.isDone)
			yield return new WaitForSeconds(1f);

		Debug.Log("Header: " + registerForm.responseHeaders["STATUS"]);
		Debug.Log("Text: " + registerForm.text);

		if (registerForm.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UserService.UpdateLocalUser(registerForm.text);
			LoadView("Home");
		}
		else
		{
			if (registerForm.responseHeaders["STATUS"] == HTML.HTTP_400)
				AlertsService.makeAlert("E-mail em uso", "O endereço de e-mail inserido está em uso, tente um diferente.", "Entendi");
			else
				AlertsService.makeAlert("Falha na conexão", "Ocorreu um erro inesperado. Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	private bool CheckFields()
	{
		return UtilsService.CheckName(nameField.text) &&
			   UtilsService.CheckEmail(emailField.text) && 
			   UtilsService.CheckPassword(passwordField.text);
	}
}
