using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalGeneric : MonoBehaviour 
{
	public Button sendButton;

	public void Destroy ()
	{
		Destroy(this.gameObject);
	}
}
