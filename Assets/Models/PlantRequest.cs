[System.Serializable]
public class PlantRequest
{
	public int 
	_id,
	_user,
	_type,
	sidewalk_size;

	public string
	tree_name,
	name,
	location_lat,
	location_lng,
	quantity,
	requester_name,
	place,
	status,
	photo,
	street,
	complement,
	number,
	neighborhood,
	city,
	state,
	zipcode,
	created_at,
	updated_at;

	public string[] places;

	public string GetAddress()
	{
		string address;

		if (location_lat != null && location_lng != null)
			address = location_lat + "," + location_lng;
		else
		 	address = street + " " + number + " " + city + " " + state;
		
		return address;
	}
}
