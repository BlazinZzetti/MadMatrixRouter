using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private GameObject playerCamera;

	// Use this for initialization
	void Start ()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(playerCamera != null)
        {
            transform.LookAt(playerCamera.transform);
            transform.Rotate(new Vector3Int(0, 180, 0));
        }
	}
}
