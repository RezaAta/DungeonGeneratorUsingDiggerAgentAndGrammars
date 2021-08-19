using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBody : MonoBehaviour
{
    public Material material;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public MeshCollider agentCollider;
    public Animator animator;
    public RuntimeAnimatorController runtimeAnimatorController;
    public HumanoidAgent agent;

    // Start is called before the first frame update
    void Awake()
    {
        if (this.agentCollider == null)
        {
            this.agentCollider = this.gameObject.AddComponent<MeshCollider>();
            this.agentCollider.sharedMesh = this.skinnedMeshRenderer.sharedMesh;
        }

        //if (this.animator == null)
        //{
        //    this.animator = this.gameObject.AddComponent<Animator>();
        //    this.animator.runtimeAnimatorController = this.runtimeAnimatorController;
        //}
        //else
        //    this.animator.runtimeAnimatorController = this.runtimeAnimatorController;


        this.material = this.gameObject.GetComponent<Material>();
        this.skinnedMeshRenderer = this.gameObject.GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
