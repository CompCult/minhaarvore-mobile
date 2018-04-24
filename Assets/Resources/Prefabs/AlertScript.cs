using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertScript : MonoBehaviour 
{
	public void Destroy ()
	{
		Destroy(this.gameObject);
	}
}
