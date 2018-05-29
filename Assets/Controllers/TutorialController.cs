using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : ScreenController 
{
	private string STATUS_DONE = "OK";

	public void CheckTutorial (string stepName)
	{
		string hash = GetHash(stepName);

		if (!PlayerPrefs.HasKey(hash))
		{
			OpenModal(stepName);
			PlayerPrefs.SetString(hash, STATUS_DONE);
		}
	}

	private string GetHash (string stepName)
	{
		int userId = UserService.user._id;
		string hash = "MinhaArvore:Tutorial:" + userId + ":" + stepName;
		
		return hash;
	}
}
