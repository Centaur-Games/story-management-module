using UnityEngine;

[RequireComponent(typeof(AnimatorHelper))]
[RequireComponent(typeof(Animator))]
public class AnimatedObject : MonoBehaviour
{
    Animator animator;

    public void SetActive(bool isActivate) {
        if(!isActivate) {
            if(animator == null || animator.runtimeAnimatorController == null) {
                gameObject.SetActive(false);
                return;
            }
            animator.Play("out");
        } else {
            gameObject.SetActive(true);
            if(animator == null)  animator = GetComponent<Animator>();
            if(animator.runtimeAnimatorController != null)  animator.Play("entry");
        }
    }

    public void SetActivePop(bool isActivate) {
        if(!isActivate) {
            if(animator == null || animator.runtimeAnimatorController == null) {
                gameObject.SetActive(false);
                return;
            }
            animator.Play("outPop");
        } else {
            gameObject.SetActive(true);
            if(animator == null)  animator = GetComponent<Animator>();
            if(animator.runtimeAnimatorController != null)  animator.Play("entryPop");
        }
    }
}
