using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalImage : ModalGeneric 
{
	public CameraCaptureService camService;
	private string modalType = "Image";

	public void Start ()
	{
		camService.resetFields("seeding_pot");
	}

	public void Update ()
	{
		if (camService.photoBase64 != null)
			sendButton.interactable = true;
		else
			sendButton.interactable = false;
	}

	public void SavePicture ()
	{
		string photoBase64 = camService.photoBase64;
		MissionsService.UpdateMissionAnswer(modalType, photoBase64);

		Destroy();
	}
}
