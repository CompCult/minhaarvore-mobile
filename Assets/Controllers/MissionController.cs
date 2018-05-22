using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : ScreenController 
{
	public GameObject imageButton, audioButton, videoButton, textButton, geolocationButton;
	public Button sendButton;
	public InputField missionName;
	public Text missionDescription;
	public Dropdown senderTypeDropdown;

	private Mission currentMission;

	public void Start ()
	{
		previousView = "Missions";
		currentMission = MissionsService.mission;
		sendButton.interactable = false;

		missionName.text = currentMission.name;
		missionDescription.text = currentMission.description;

		ResetButtons ();
		UpdateMissionInfo ();
	}

	public void OpenModal (string modalName)
	{
		string modalPath = "Prefabs/Modal" + modalName;

        GameObject modalPrefab = (GameObject) Resources.Load(modalPath),
                   modalInstance = (GameObject) GameObject.Instantiate(modalPrefab, Vector3.zero, Quaternion.identity);
        
        modalInstance.transform.SetParent (GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        modalInstance.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
	}

	public void SendResponse ()
	{
		Mission currentMission = MissionsService.mission;
		MissionAnswer currentAnswer = MissionsService.missionAnswer;
		
		string message = null;

		if (currentAnswer != null) 
		{
			if (currentMission.has_image && currentAnswer.image == null)
				message = "Você precisa registrar uma foto antes de enviar essa resposta.";

			if (currentMission.has_audio && currentAnswer.audio == null)
				message = "Você precisa capturar um áudio antes de enviar essa resposta.";

			if (currentMission.has_video && currentAnswer.video == null)
				message = "Você precisa registrar um vídeo antes de enviar essa resposta.";

			if (currentMission.has_text && currentAnswer.text_msg == null)
				message = "Você precisa escrever um texto antes de enviar essa resposta.";

			if (currentMission.has_geolocation && (currentAnswer.location_lat == null || currentAnswer.location_lng == null))
				message = "Você precisa registrar sua geolocalização antes de enviar essa resposta.";
		}
		else
		{
			message = "Você precisa enviar os dados solicitados pela missão antes de enviar uma resposta.";
		}

		if (message == null)
			StartCoroutine(_SendResponse());
		else
			AlertsService.makeAlert("Aviso", message, "Entendi");
	}

	private IEnumerator _SendResponse ()
	{
		AlertsService.makeLoadingAlert("Enviando resposta");

		int currentUserId = UserService.user._id,
			missionId = MissionsService.mission._id,
			groupId = GetSelectedGroupId(senderTypeDropdown.captionText.text);

		WWW responseRequest = MissionsService.SendResponse(currentUserId, missionId, groupId);

		while (!responseRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + responseRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + responseRequest.text);
		AlertsService.removeLoadingAlert();

		if (responseRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			Mission currentMission = MissionsService.mission;

			if (currentMission.end_message != null && currentMission.end_message.Length > 0)
				OpenModal("Final");
			else
			{
				AlertsService.makeAlert("Resposta enviada", "Boa! Sua resposta foi enviada com sucesso. Você será redirecionado(a) para as missões.", "");
				yield return new WaitForSeconds(5f);
				LoadView("Missions");
			}
		}
		else 
		{
			if (responseRequest.responseHeaders["STATUS"] == HTML.HTTP_400)
				AlertsService.makeAlert("Senha incorreta", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else if (responseRequest.responseHeaders["STATUS"] == HTML.HTTP_404 || responseRequest.responseHeaders["STATUS"] == HTML.HTTP_401)
				AlertsService.makeAlert("Usuário não encontrado", "Por favor, verifique se inseriu corretamente o e-mail e senha.", "OK");
			else
				AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
		}

		yield return null;
	}

	#pragma warning disable 0472
	private void UpdateMissionInfo ()
	{
		if (currentMission.is_grupal != null && currentMission.is_grupal)
			StartCoroutine(_GetGroups());
		else
			sendButton.interactable = true;

		if (currentMission.has_image != null && currentMission.has_image)
			imageButton.SetActive(true);

		if (currentMission.has_audio != null && currentMission.has_audio)
			audioButton.SetActive(true);

		if (currentMission.has_video != null && currentMission.has_video)
			videoButton.SetActive(true);

		if (currentMission.has_text != null && currentMission.has_text)
			textButton.SetActive(true);

		if (currentMission.has_geolocation != null && currentMission.has_geolocation)
			geolocationButton.SetActive(true);
	}

	private IEnumerator _GetGroups ()
	{
		User currentUser = UserService.user;
		WWW groupsRequest = GroupsService.GetUserGroups(currentUser._id);

		while (!groupsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + groupsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + groupsRequest.text);

		if (groupsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			GroupsService.UpdateCurrentGroups(groupsRequest.text);

			if (GroupsService.groups.Length < 1)
			{
				senderTypeDropdown.gameObject.SetActive(false);
				AlertsService.makeAlert("Sem grupos", "Essa é uma missão em grupo e você não está em nenhum grupo. Participe de um grupo para poder responder essa missão.", "");
				yield return new WaitForSeconds(5f);
				LoadView("Missions");
			}
			else
				sendButton.interactable = true;

			FillGroupsDropdown();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Essa é uma missão em grupo e não pudemos listar seus grupos.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Missions");
		}

		yield return null;
	}

	private int GetSelectedGroupId (string groupName)
	{
		foreach (Group group in GroupsService.groups)
			if (group.name == groupName)
				return group._id;

		return 0;
	}

	private void FillGroupsDropdown ()
	{
		senderTypeDropdown.ClearOptions();
	    senderTypeDropdown.AddOptions(GroupsService.GetGroupNames());
	    senderTypeDropdown.RefreshShownValue();
	}

	private void ResetButtons ()
	{
		imageButton.SetActive(false);
		audioButton.SetActive(false);
		videoButton.SetActive(false);
		geolocationButton.SetActive(false);
		textButton.SetActive(false);
	}

}
