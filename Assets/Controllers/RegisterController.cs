using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : ScreenController 
{
	public InputField nameField, emailField, passwordField;

	public void Start ()
	{
		previousView = "Login";
	}

	public void Register ()
	{
		AlertsService.makeLoadingAlert("Registrando");

		StartCoroutine(_Register());
	}

	private IEnumerator _Register ()
	{
		LoadView("Home");
		yield return null;
	}

	private bool CheckFields()
	{
		return UtilsService.CheckName(nameField.text) &&
			   UtilsService.CheckEmail(emailField.text) && 
			   UtilsService.CheckPassword(passwordField.text);
	}
}
