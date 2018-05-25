using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlantCard : MonoBehaviour
{
	#pragma warning disable 0108

	public GameObject statusApproved, statusPending, statusRejected;
	public Text name, type, date;
	public RawImage photo;
	public Plant plant;

	private string photoUrl;
	private string STATUS_APPROVED = "Aprovado",
				   STATUS_REJECTED = "Rejeitado",
				   STATUS_PENDING = "Pendente",
				   UNKNOWN_TYPE = "Tipo desconhecido";

	public void UpdateModalInfo ()
	{
		PlantsService.UpdateLocalPlant(plant);
	}

	public void UpdatePlantCard (Plant plant)
	{
		this.plant = plant;
		name.text = plant.name;

		if (plant._request != null && plant._request.status != null)
		{
			if (plant._request.status == STATUS_APPROVED)
			{
				statusApproved.SetActive(true);
				date.text = STATUS_APPROVED;
			}
			else if (plant._request.status == STATUS_REJECTED)
			{
				statusRejected.SetActive(true);
				date.text = STATUS_REJECTED;
			}
			else
			{
				statusPending.SetActive(true);
				date.text = STATUS_PENDING;
			}
		}
		else
		{
			statusPending.SetActive(true);
			date.text = STATUS_PENDING;
		}

		if (plant.planting_date != null)
    		date.text = "Plantada em " + plant.planting_date;

    	type.text = UNKNOWN_TYPE;
    	foreach (PlantType type in PlantsService.types)
    	{
    		if (type._id == plant._type)
    		{
    			this.type.text = type.name;
    			this.photoUrl = type.photo;
    			break;
    		}
    	}

    	StartCoroutine(_UpdatePlantPhoto());
	}

	private IEnumerator _UpdatePlantPhoto ()
	{
		Texture2D texture;

		if (photoUrl == null || photoUrl.Length < 1)
		{
			texture = UtilsService.GetDefaultProfilePhoto();
		}
		else
		{
			var www = new WWW(photoUrl);
			yield return www;

			texture = UtilsService.ResizeTexture(www.texture, "Average", 0.25f);
		}

		if (texture == null)
			texture = UtilsService.GetDefaultProfilePhoto();

		photo.texture = texture;
	}
}
