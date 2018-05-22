using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalGeneric : MonoBehaviour 
{
	public void Destroy ()
	{
		Destroy(this.gameObject);
	}
}
