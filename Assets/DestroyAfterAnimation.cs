using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    private Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        var animEvent = new AnimationEvent();
        animEvent.functionName = nameof(DestroyMe);
        animEvent.time = Animator.runtimeAnimatorController.animationClips[0].length-0.01f;
        Animator.runtimeAnimatorController.animationClips[0].AddEvent(animEvent);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
