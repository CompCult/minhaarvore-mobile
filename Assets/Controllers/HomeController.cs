using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : ScreenController 
{
	public InputField nameField, plantsField, leavesField;

	public void Start ()
	{
		previousView = "Login";
		FillInputFields ();
	}

	public void FillInputFields ()
	{
		// TODO
	}
}
