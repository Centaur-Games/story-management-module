using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedObject : MonoBehaviour
{
    bool hasPendingAnim = false;
    Animator animator;

    void _pop(bool state, string outAnim, string entryAnim) {
        if (hasPendingAnim) {
            throw new System.Exception($"{gameObject.name} anim pending");
        }

        if(state == gameObject.activeInHierarchy){
            Debug.Log($"{gameObject.name} objesi zaten {(state ? "açık" : "kapalı")}.");
            return;
        }

        hasPendingAnim = true;

        if(!state) {
            if(animator == null || animator.runtimeAnimatorController == null) {
                gameObject.SetActive(false);
                hasPendingAnim = false;
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

    void closeObject() {
        hasPendingAnim = false;
        StoryManager.pendingAnims--;
        gameObject.SetActive(false);
    }

    void openObject() {
        hasPendingAnim = false;
        StoryManager.pendingAnims--;
    }

    public void go(Vector2 newPos) {
        targetPos = newPos;
        animating = true;
    }

    bool animating;
    Vector2 targetPos;
    [SerializeField] float lambda;
    [SerializeField] float epsilon;

    void Start() {
        animating = false;
        targetPos = transform.position;
    }

    void Update() {
        if (!animating) {
            return;
        }

        float exp = 1 - Mathf.Exp(-lambda * Time.deltaTime);
        transform.position = Vector2.Lerp(
            transform.position,
            targetPos,
            exp
        );

        if (exp > 1-epsilon) {
            animating = false;
        }
    }
}
