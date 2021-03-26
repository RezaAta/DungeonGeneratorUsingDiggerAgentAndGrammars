using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : HumanoidAgent
{

    private void Awake()
    {
        base.statusUpdater = null;
    }
}
