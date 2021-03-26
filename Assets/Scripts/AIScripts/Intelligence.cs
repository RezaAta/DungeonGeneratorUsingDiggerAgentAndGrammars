using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intelligence : MonoBehaviour
{
    public HumanoidAgent agent;

    
    public void Awake()
    {
        if (this.agent == null)
            this.agent = this.gameObject.GetComponent<HumanoidAgent>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
