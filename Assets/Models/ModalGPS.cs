using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalGPS : ModalGeneric {

	public Text gpsStatus;
	public Button pickLocationButton;

	public void Start ()
	{
		pickLocationButton.interactable = false;
		StartCoroutine(_CheckGPS());
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
			gpsStatus.text = "Sintonizando...";
			mustCheckGPS = true;
		}
		else 
		{
			gpsStatus.text = "Rastreado";
			pickLocationButton.interactable = true;
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
}
