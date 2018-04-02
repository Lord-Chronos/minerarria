using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float cameraSpeed;

	// Update is called once per frame
	void Update () {
        float hzAxis = Input.GetAxis("Horizontal");
        gameObject.transform.Translate(new Vector3(hzAxis * Time.deltaTime * cameraSpeed, 0, 0));
	}
}
