using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(45,45,0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + new Vector3(-8, 12, -8);
        
    }
}
