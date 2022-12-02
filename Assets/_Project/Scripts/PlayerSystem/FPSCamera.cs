using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From: https://gist.github.com/KarlRamstedt/407d50725c7b6abeaf43aee802fdd88e
public class FPSCamera : MonoBehaviour
{
	[Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
	[Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

	Vector2 rot = Vector2.zero;
	const string xAxis = "Mouse X";
	const string yAxis = "Mouse Y";

    private void OnEnable ()
    {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable ()
    {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.None;
	}

    void Update ()
	{
		rot.x += Input.GetAxis(xAxis) * sensitivity;
		rot.y += Input.GetAxis(yAxis) * sensitivity;
		rot.y = Mathf.Clamp(rot.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rot.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rot.y, Vector3.left);

		transform.localRotation = xQuat * yQuat;
	}
}

