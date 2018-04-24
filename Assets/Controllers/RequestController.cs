using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestController : ScreenController 
{
	public GameObject sideWalkSizeObj;
	public Dropdown plantTypesDropdown;
	public InputField plantName, requesterName, quantityField, sideWalkSize;
	public Toggle[] places;
	public Toggle termsToggle;

	public CameraCaptureService camService;

	private Toggle markedPlace;
	private string CHECK_OK = "OK";

	public void Start ()
	{
		previousView = "Plants";
		sideWalkSizeObj.SetActive(false);

		FillPlantTypesDropdown();
		GPSService.StartGPS();
	}

	public void RequestTree ()
	{
		AlertsService.makeLoadingAlert("Solicitando");

		StartCoroutine(_RequestTree());
	}

	private IEnumerator _RequestTree ()
	{
		string status = CheckFields();
		
		if (status != CHECK_OK)
		{
			AlertsService.removeLoadingAlert();
			AlertsService.makeAlert("Campos inválidos", status, "OK");
			yield break;
		}

		byte[] photoArray = camService.photoArray;

		int quantity = int.Parse(quantityField.text),
			typeID = -1;

		string plant = plantName.text,
			   requester = requesterName.text,
			   place = markedPlace.GetComponentInChildren<Text>().text,
			   sidewalkSize = null;

		foreach (PlantType type in PlantsService.types)
			if (type.name.Equals(plantTypesDropdown.captionText.text))
				typeID = type._id;

		if (place == "Calçada")
			sidewalkSize = sideWalkSize.text;

		WWW requestForm = PlantsService.RequestTree(typeID, photoArray, plant, requester, place, sidewalkSize, quantity);

		while (!requestForm.isDone)
			yield return new WaitForSeconds(1f);

		AlertsService.removeLoadingAlert();
		Debug.Log("Header: " + requestForm.responseHeaders["STATUS"]);
		Debug.Log("Text: " + requestForm.text);

		if (requestForm.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			yield return StartCoroutine(_Return());
		}
		else
		{
			AlertsService.makeAlert("Falha na conexão", "Ocorreu um problema ao enviar seu pedido. Tente novamente.", "Entendi");
		}

		yield return null;
	}

	public void UpdatePlace (Toggle newPlace)
	{
		if (newPlace == markedPlace && newPlace.isOn == false)
		{
			newPlace.isOn = true;
			return;
		}

		string name = newPlace.GetComponentInChildren<Text>().text;

		if (name == "Calçada" && newPlace.isOn == true)
			sideWalkSizeObj.SetActive(true);
		else 
			sideWalkSizeObj.SetActive(false);

		foreach (Toggle place in places)
		{
			if (place != newPlace)
				place.isOn = false;
			else 
				markedPlace = place;
		}
	}

	private void FillPlantTypesDropdown ()
	{
		if (plantTypesDropdown != null)
		{
			plantTypesDropdown.ClearOptions();
	        plantTypesDropdown.AddOptions(PlantsService.GetTypeNames());
	        plantTypesDropdown.RefreshShownValue();
    	}
	}

	private string CheckFields ()
	{
		string message = CHECK_OK;
		bool placeSelected = false;

		GPSService.ReceivePlayerLocation();

		if (!GPSService.IsActive())
			message = "Ative o GPS de seu celular antes de enviar uma solicitação.";

		if (GPSService.location[0] == 0.0 || 
			GPSService.location[1] == 0.0)
			message = "Ainda não obtivemos sua geolocalização. Aguarde alguns instantes e tente novamente.";

		if (camService.photoArray == null)
			message = "Capture ou selecione a foto do local que deseja realizar o plantio.";

		if (markedPlace != null)
		{
			string place = markedPlace.GetComponentInChildren<Text>().text;
			if (place == "Calçada" && sideWalkSize.text.Length < 2)
				message = "Escreva o tamanho de sua calçada em centímetros.";
		}

		if (plantName.text.Length < 2)
			message = "O nome da muda deve possuir, pelo menos, dois caracteres.";
		
		if (requesterName.text.Length < 3)
			message = "O nome do solicitante deve possuir, pelo menos, três caracteres.";
		
		if (!termsToggle.isOn)
			message = "Você deve concordar com o termo de ciência de pedidos no Minha Árvore.";
		
		if (quantityField.text.Length < 1 || int.Parse(quantityField.text) < 1)
			message = "Você deve pedir, pelo menos, uma planta no campo de quantidade.";
		
		foreach (Toggle place in places)
			if (place.isOn)
				placeSelected = true;

		if (!placeSelected)
			message = "Selecione o local de plantio de sua nova muda.";

		return message;
	}

	private IEnumerator _Return ()
	{
		AlertsService.makeAlert("Solicitação realizada", "Em breve entraremos em contato caso seu pedido seja aprovado.", "");
		yield return new WaitForSeconds(5f);
		LoadView("Plants");
		yield return null;
	}

}
