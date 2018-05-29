using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModalGeneric : ScreenController 
{
	public Button sendButton;

	public void Destroy ()
	{
		PlaySound("back");
		Destroy(this.gameObject);
	}

	public string GetViewName()
	{
		Scene scene = SceneManager.GetActiveScene(); 
		return scene.name;
	}

	public void ReloadView()
	{
		SceneManager.LoadScene(GetViewName());
	}
}
