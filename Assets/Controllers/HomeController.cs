using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : ScreenController 
{
	public Text nameField, plantsField, leavesField;

	public void Start ()
	{
		FillInputFields ();
	}

	public void FillInputFields ()
	{
		User user = UserService.user;

		if (user.name != null)
			nameField.text = user.name;

		if (user.points.ToString() != null)
			leavesField.text = user.points.ToString();

		if (PlantsService.plants != null)
			plantsField.text = PlantsService.plants.Length.ToString();
		else
			StartCoroutine(_RequestUserPlants());;
	}

	private IEnumerator _RequestUserPlants ()
	{
		WWW plantsRequest = PlantsService.GetUserPlants(UserService.user._id);

		while (!plantsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + plantsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + plantsRequest.text);

		if (plantsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			PlantsService.UpdateLocalPlants(plantsRequest.text);
			plantsField.text = PlantsService.plants.Length.ToString();
		}

		yield return null;
	}
}
