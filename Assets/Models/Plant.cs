using UnityEngine;

[System.Serializable]
public class Plant 
{
	public int 
	_id,
	_user,
	_type;

	public string 
	name,
	location_lat,
	location_lng,
	planting_date,
	status;

	public PlantRequest _request;
}