using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float cameraSpeed;

    private Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update () {
        float hzAxis = Input.GetAxis("Horizontal");
        //gameObject.transform.Translate(new Vector3(hzAxis * Time.deltaTime * cameraSpeed, 0, 0)); // Deprecated code
        gameObject.transform.position = new Vector3(player.position.x, 0, -10);
	}
}
