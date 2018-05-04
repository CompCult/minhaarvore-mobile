using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestController : ScreenController 
{
	public GameObject detailsCardObj, addressCardObj;

	// Details
	public GameObject sideWalkSizeObj;
	public Dropdown plantTypesDropdown, placeDropdown;
	public InputField plantName, requesterName, quantityField, sideWalkSize;

	// Address
	public GameObject manualAddressObj, gpsImageObj;
	public Text gpsStatus;
	public InputField streetField, numberField, neighborhoodField, cityField, complementField, zipField;
	public Dropdown stateDropdown;
	public Toggle termsToggle;

	// Extras
	public CameraCaptureService camService;
	private string CHECK_OK = "OK",
				   CURRENT_MENU = "DETAILS",
				   CURRENT_GET_LOCATION = "MANUAL";

	public void Start ()
	{
		previousView = "Plants";
		sideWalkSizeObj.SetActive(false);

		camService.resetPreview();

		FillPlantTypesDropdown();
		GPSService.StartGPS();
	}

	public void ToggleGetLocation()
	{
		if (CURRENT_GET_LOCATION == "MANUAL")
		{
			CURRENT_GET_LOCATION = "GPS";
			gpsImageObj.SetActive(true);
			manualAddressObj.SetActive(false);

			StartCoroutine(_CheckGPS());
		}
		else
		{
			CURRENT_GET_LOCATION = "MANUAL";
			gpsImageObj.SetActive(false);
			manualAddressObj.SetActive(true);
		}
	}

	private IEnumerator _CheckGPS()
	{
		bool mustCheckGPS = false;
		GPSService.StartGPS();

		if (!GPSService.IsActive())
		{
			gpsStatus.text = "Aguardando ativação do GPS no celular";
			mustCheckGPS = true;
		}
		else if (GPSService.location[0] == 0.0 || GPSService.location[1] == 0.0)
		{
			GPSService.ReceivePlayerLocation();
			gpsStatus.text = "Obtendo localização...";
			mustCheckGPS = true;
		}
		else 
		{
			gpsStatus.text = "Localização obtida";
		}

		if (mustCheckGPS)
		{
			yield return new WaitForSeconds(2);
			yield return StartCoroutine(_CheckGPS());
		}
		else
		{
			yield return null;
		}
	}

	public void ToggleMenu()
	{
		if (CURRENT_MENU == "DETAILS")
		{
			CURRENT_MENU = "ADDRESS";
			detailsCardObj.SetActive(false);
			addressCardObj.SetActive(true);
		}
		else
		{
			CURRENT_MENU = "DETAILS";
			detailsCardObj.SetActive(true);
			addressCardObj.SetActive(false);
		}
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

		string photoBase64 = camService.photoBase64;

		int quantity = int.Parse(quantityField.text),
			typeID = -1;

		string plant = plantName.text,
			   requester = requesterName.text,
			   place = placeDropdown.captionText.text,
			   sidewalkSize = sideWalkSize.text,
			   street = streetField.text,
			   number = numberField.text,
			   neighborhood = neighborhoodField.text,
			   city = cityField.text,
			   state = stateDropdown.captionText.text,
			   complement = complementField.text,
			   zipcode = zipField.text;

		foreach (PlantType type in PlantsService.types)
			if (type.name.Equals(plantTypesDropdown.captionText.text))
				typeID = type._id;

		WWW requestForm = PlantsService.RequestTree(typeID, photoBase64, plant, requester, place, sidewalkSize, quantity,
													street, number, neighborhood, city, state, complement, zipcode, CURRENT_GET_LOCATION);

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

	public void UpdatePlace ()
	{
		if (placeDropdown.captionText.text == "Calçada")
			sideWalkSizeObj.SetActive(true);
		else
			sideWalkSizeObj.SetActive(false);
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

		GPSService.ReceivePlayerLocation();

		if  (CURRENT_GET_LOCATION == "GPS" &&
			 (GPSService.location[0] == 0.0 || 
			 GPSService.location[1] == 0.0 ||
			 !GPSService.IsActive())
			)
			message = "Ainda não obtivemos sua geolocalização. Verifique se seu GPS está ligado.";

		if (camService.photoBase64 == null)
			message = "Capture ou selecione a foto do local que deseja realizar o plantio.";

		if (placeDropdown.captionText.text == "Calçada" &&
			sideWalkSize.text.Length < 2)
			message = "Escreva o tamanho de sua calçada em centímetros.";

		if (plantName.text.Length < 2)
			message = "O nome da muda deve possuir, pelo menos, dois caracteres.";
		
		if (requesterName.text.Length < 3)
			message = "O nome do solicitante deve possuir, pelo menos, três caracteres.";
		
		if (!termsToggle.isOn)
			message = "Você deve concordar com o termo de ciência de pedidos no Minha Árvore.";
		
		if (quantityField.text.Length < 1 || int.Parse(quantityField.text) < 1)
			message = "Você deve pedir, pelo menos, uma planta no campo de quantidade.";

		if (CURRENT_GET_LOCATION == "MANUAL")
		{
			if (streetField.text.Length < 5 ||
				numberField.text.Length < 1 ||
				neighborhoodField.text.Length < 3 ||
				cityField.text.Length < 2 ||
				zipField.text.Length < 8)
			message = "Preencha seu endereço por completo.";
		}


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
