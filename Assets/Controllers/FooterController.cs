using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterController : ScreenController  
{
	public GameObject[] buttons;
	public GameObject buttonsMenu;

	private Color greenColor = new Color(0.07450981f, 0.7568628f, 0.3333333f),
				  greyColor = new Color(0.4470589f, 0.4470589f, 0.4470589f);

	public void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	public void OnLevelWasLoaded()
	{
		Debug.Log("Loaded: " + GetViewName());

		if (GetViewName() == "Login" || GetViewName() == "Register")
			buttonsMenu.SetActive(false);
		else
			buttonsMenu.SetActive(true);
	}

	public void LoadViewAndMark(string viewName)
	{
		LoadView(viewName);

		foreach (GameObject button in buttons)
		{
			if (button.name == viewName)
				MarkButton(button, greenColor);
			else
				MarkButton(button, greyColor);
		}
	}

	private void MarkButton(GameObject button, Color color)
	{
		Image icon = button.GetComponentsInChildren<Image>()[1];
		Text label = button.GetComponentsInChildren<Text>()[0];

		icon.color = color;
		label.color = color;
	}
}
