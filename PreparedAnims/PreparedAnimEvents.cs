using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparedAnimEvents : MonoBehaviour
{
    Animator animator;
    void Awake() { getAnimator(); }

    public void forward() {
        getAnimator();
        animator.Play("forward");
    }
    public void revert() {
        getAnimator();
        animator.Play("revert");
    }

    public void pushState() {
        getAnimator();
        animator.Play("idle");
        StoryManager.pushNextState(false);
    }

    public void prevState() {
        getAnimator();
        animator.Play("idle");
        StoryManager.backStory(1);
    }

    void getAnimator() {
        if(animator != null) return;
        animator = GetComponent<Animator>();
    }
}
