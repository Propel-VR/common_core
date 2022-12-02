using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorZone : MonoBehaviour
{
	[SerializeField] DoorScript door;
	[SerializeField] bool enableOpen = true;
	[SerializeField] bool enableClosing = true;

	void OnTriggerEnter (Collider other)
	{
		if(enableOpen) door.SimulateOnTriggerEnter(other);
	}

	void OnTriggerExit (Collider other)
	{
		if(enableClosing) door.SimulateOnTriggerExit(other);
	}
}
