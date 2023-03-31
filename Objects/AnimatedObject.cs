using UnityEngine;

[RequireComponent(typeof(AnimatorHelper))]
[RequireComponent(typeof(Animator))]
public class AnimatedObject : MonoBehaviour
{
    Animator animator;

    void _pop(bool state, string outAnim, string entryAnim) {
        if(!state) {
            if(animator == null || animator.runtimeAnimatorController == null) {
                gameObject.SetActive(false);
                return;
            }

            StoryManager.pendingAnims++;
            animator.Play(outAnim);
        } else {
            gameObject.SetActive(true);

            if(animator == null) {
                animator = GetComponent<Animator>();
            }

            if(animator.runtimeAnimatorController != null) {
                StoryManager.pendingAnims++;
                animator.Play(entryAnim);
            }
        }
    }

    public void SetActive(bool isActive) {
        _pop(isActive, "out", "entry");
    }

    public void SetActivePop(bool isActive) {
        _pop(isActive, "outPop", "entryPop");
    }
}
