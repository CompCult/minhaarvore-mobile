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
		MissionsService.SendResponse();
	}

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

	private void FillGroupsDropdown ()
	{
		senderTypeDropdown.ClearOptions();
	    senderTypeDropdown.AddOptions(GroupsService.GetGroupNames());
	    senderTypeDropdown.RefreshShownValue();
	}

}
