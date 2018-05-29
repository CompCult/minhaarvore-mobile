using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlantType 
{
	public int 
	_id,
	ammount_available;

	public string 
	name,
	description,
	photo;

	public string[] places;

	public List<string> GetPlaceList()
	{
		var list = new List<string>(places);
		return list;
	}
}
