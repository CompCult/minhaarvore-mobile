using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class MissionsService
{
	private static Mission[] _missions;
	public static Mission[] missions { get { return _missions; } }

	private static Mission _mission;
	public static Mission mission { get { return _mission; } }

	private static MissionAnswer _missionAnswer;
	public static MissionAnswer missionAnswer { get { return _missionAnswer; } }

	public static WWW SearchMission (int userId, string secretCode)
	{
		WebService.route = ENV.MISSIONS_ROUTE;
		WebService.action = ENV.SEARCH_PRIVATE +
							"user_id=" + userId +
							"&mission_id=" + secretCode;

		return WebService.Get();
	}

	public static WWW SendResponse ()
	{
		WWWForm requestForm = new WWWForm ();
		//requestForm.AddField ("_quiz", _quiz);
		//requestForm.AddField ("_user", _user);
		//requestForm.AddField ("answer", answer);

		WebService.route = ENV.QUIZ_ANSWERS_ROUTE;
		WebService.action = "";

		return WebService.Post(requestForm);
	}

	public static WWW GetMissions (int userId)
	{
		WebService.route = ENV.MISSIONS_ROUTE;
		WebService.action = ENV.SEARCH_PUBLIC +
							"user_id=" + userId;

		return WebService.Get();
	}

	public static void UpdateMissions (string json)
	{
		_missions = UtilsService.GetJsonArray<Mission>(json);
	}

	public static void UpdateMission (string json)
	{
		_mission = JsonUtility.FromJson<Mission>(json);
	}

	public static void UpdateMission (Mission mission)
	{
		_mission = mission;
	}

}
