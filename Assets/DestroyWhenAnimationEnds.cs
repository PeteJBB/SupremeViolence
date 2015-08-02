using UnityEngine;
using System.Collections;

public class DestroyWhenAnimationEnds : MonoBehaviour 
{
    private Animator anim;

	// Use this for initialization
	void Start () 
    {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if(state.normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
	}
}
