using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantsController : ScreenController 
{
	public GameObject plantCard, noPlantsCard;
	public GameObject statusApproved, statusPendent, statusRejected;

	public void Start ()
	{
		TutorialService.CheckTutorial("Plants");

		previousView = "Login";
		plantCard.SetActive(false);
		
		StartCoroutine(_GetPlantTypes());
	}

	private IEnumerator _GetPlantTypes ()
	{
		AlertsService.makeLoadingAlert("Recebendo plantas");
		WWW typesRequest = PlantsService.GetPlantTypes();

		while (!typesRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + typesRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + typesRequest.text);

		if (typesRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			PlantsService.UpdateLocalPlantTypes(typesRequest.text);

			foreach (PlantType plant in PlantsService.types)
				Debug.Log("Locais: " + plant._places[0].name);

			yield return StartCoroutine(_GetPlants());
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Home");
		}

		yield return null;
	}

	private IEnumerator _GetPlants ()
	{
		WWW plantsRequest = PlantsService.GetUserPlants(UserService.user._id);

		while (!plantsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + plantsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + plantsRequest.text);
		AlertsService.removeLoadingAlert();

		if (plantsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			PlantsService.UpdateLocalPlants(plantsRequest.text);
			CreatePlantsCard();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Home");
		}

		yield return null;
	}

	private void CreatePlantsCard ()
    {
     	Vector3 position = plantCard.transform.position;

     	if (PlantsService.plants.Length > 0)
     		plantCard.SetActive(true);
     	else
     		noPlantsCard.SetActive(true);

     	foreach (Plant plant in PlantsService.plants)
        {
        	Debug.Log("Request Status: " + plant._request.status);

            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(plantCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	PlantCard plantCardScript = card.GetComponent<PlantCard>();
        	plantCardScript.UpdatePlantCard(plant);
        }

        plantCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

}
