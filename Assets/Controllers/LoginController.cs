using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : ScreenController 
{
	public InputField emailField, passwordField;

	public void Start()
	{
		CheckSavedEmail();
	}

	private void CheckSavedEmail()
	{
		if (PlayerPrefs.HasKey("MinhaArvore:Email"))
		{
			emailField.text = PlayerPrefs.GetString("MinhaArvore:Email");
		}
	}

	private void SaveEmail()
	{
		PlayerPrefs.SetString("MinhaArvore:Email", emailField.text);
	}

	public void Authenticate ()
	{
		AlertsService.makeLoadingAlert("Autenticando");

		StartCoroutine(_Authenticate());
	}

	private IEnumerator _Authenticate ()
	{
		yield return null;
	}

	private bool CheckFields()
	{
		return UtilsService.CheckEmail(emailField.text) && UtilsService.CheckPassword(passwordField.text);
	}
}
