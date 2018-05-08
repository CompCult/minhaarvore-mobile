using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantsController : ScreenController 
{
	public GameObject plantCard, noPlantsCard;
	public GameObject statusApproved, statusPendent, statusRejected;

	public Text plantName, plantType, plantDate;

	public void Start ()
	{
		previousView = "Login";
		plantCard.SetActive(false);
		
		StartCoroutine(_GetPlantTypes());
	}

	private IEnumerator _GetPlantTypes ()
	{
		AlertsService.makeLoadingAlert("Recebendo tipos");
		WWW typesRequest = PlantsService.GetPlantTypes();

		while (!typesRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + typesRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + typesRequest.text);
		AlertsService.removeLoadingAlert();

		if (typesRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			PlantsService.UpdateLocalPlantTypes(typesRequest.text);
			yield return StartCoroutine(_GetPlants());
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
			LoadView("Home");
		}

		yield return null;
	}

	private IEnumerator _GetPlants ()
	{
		AlertsService.makeLoadingAlert("Recebendo suas plantas");
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
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
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
        	plantName.text = plant.name;

        	if (plant.planting_date != null)
        	{
        		statusApproved.SetActive(true);
        		plantDate.text = "Plantada em " + plant.planting_date;
        	}
        	else
        	{
        		statusPendent.SetActive(true);
        		plantDate.text = "Pendente";
        	}

        	foreach (PlantType type in PlantsService.types)
        	{
        		if (type._id == plant._type)
        		{
        			plantType.text = type.name;
        			break;
        		}
        	}

            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(plantCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);
        }

        plantCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

}
