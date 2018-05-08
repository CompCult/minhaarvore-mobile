﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsRequestsController : ScreenController 
{
	public GameObject eventCard, noEventsCard;

	public void Start ()
	{
		previousView = "Events";
		eventCard.SetActive(false);

		StartCoroutine(_GetUserEvents());
	}

	private IEnumerator _GetUserEvents ()
	{
		AlertsService.makeLoadingAlert("Recebendo eventos");
		WWW eventsRequest = EventsService.GetUserEvents(UserService.user._id);

		while (!eventsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + eventsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + eventsRequest.text);
		AlertsService.removeLoadingAlert();

		if (eventsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			EventsService.UpdateUserEvents(eventsRequest.text);
			CreateEventRequestsCards();
		}
		else 
		{
			LoadPreviousView();
		}

		yield return null;
	}

	private void CreateEventRequestsCards ()
    {
     	Vector3 position = eventCard.transform.position;

     	if (EventsService.eventsRequests.Length > 0)
     		eventCard.SetActive(true);
     	else
     		noEventsCard.SetActive(true);

     	foreach (EventRequest evtReq in EventsService.eventsRequests)
        {
        	Event indexEvt = null;

        	foreach (Event evt in EventsService.events)
        		if (evt._id == evtReq._appointment)
        			indexEvt = evt;

        	if (indexEvt == null)
        		continue;

        	position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(eventCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	EventCard evtCard = card.GetComponent<EventCard>();
        	evtCard.UpdateEventWithStatus(indexEvt, evtReq);
        }

        eventCard.gameObject.SetActive(false);
        AlertsService.removeLoadingAlert();
    }

}
