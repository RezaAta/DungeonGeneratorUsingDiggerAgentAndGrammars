using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct Bar
{
    public Bar(double min = 0, double max = 100, double current = 100) 
    {
        this.min = min;
        this.max = max;
        this.current = current;
    }

    public double max;
    public double min;
    public double current;
}

public class AgentAttributes : MonoBehaviour
{
    //public List<System.Action<in List<GameObject>,out null>> setOfPossibleActions;
    public GameObject currentLocation;
    
    public Bar health = new Bar();
    public double currentHealth;

    public int power = 1;
    public int level = 1;
    public float movementSpeed = 4f;
    public double agility = 1; //base speed of casting spells and attacking
    public List<GameObject> items = new List<GameObject>();
    public double winRatio = -1;

    public string AIType = "basic";//basic,advanced,manual

    // Start is called before the first frame update
    void Start()
    {
        //this.health = new Bar();
        //this.health.max = 100;
        //this.health.min = 0;
        //this.health.current = 100;

        //this.currentHealth = this.health.current;
    }

    public void decreaseHealth()
    {
        health.current = health.current - health.current / 10;

        currentHealth = health.current;
    }
}
