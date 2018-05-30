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

	public Place[] _places;

	public List<string> GetPlaceList ()
	{
		List<string> placeNames = new List<string>();
		foreach (Place place in _places)
			placeNames.Add(place.name);

		return placeNames;
	}
}
