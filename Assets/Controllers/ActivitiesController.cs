using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivitiesController : ScreenController 
{
	public void Start ()
	{
		TutorialService.CheckTutorial("Activities");
		previousView = "Home";
	}
}
