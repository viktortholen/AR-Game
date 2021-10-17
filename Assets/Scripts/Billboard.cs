using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
 
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;

        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
       
    }
}