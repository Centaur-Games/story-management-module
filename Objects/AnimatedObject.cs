using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedObject : MonoBehaviour
{
    bool hasPendingAnim = false;
    Animator animator;

    System.Action After;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void _pop(bool state, string outAnim, string entryAnim) {
        if(!state && !gameObject.activeInHierarchy) {
            gameObject.SetActive(false);
            Debug.Log($"{gameObject.name} adlı animasyonlu obje zor kullanılarak kapatıldı");
            return;
        }

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

    public void playAnim(string anim, System.Action after, bool activator = true) {
        if(activator) gameObject.SetActive(true);
        animator.Play(anim);
    }

    void closeObject() {
        hasPendingAnim = false;
        StoryManager.pendingAnims--;
        gameObject.SetActive(false);

        if(After != null) After();
    }

    void disableObject() { gameObject.SetActive(false); }

    void closeAnimation() {
        hasPendingAnim = false;
        StoryManager.pendingAnims--;
    }

    void openObject() {
        hasPendingAnim = false;
        StoryManager.pendingAnims--;
    }

    [System.Obsolete("Bunun yerine go(Vector2) kullan")]
    public void go(string vector) {
        var x = vector.Split("|");
        go(new Vector2(float.Parse(x[0]), float.Parse(x[1])));
    }

    public void go(Vector2 vector) {
        targetPos = vector;
        animating = true;
    }

    bool animating = false;
    Vector2 targetPos;
    RectTransform rectTransform;
    [SerializeField] [Range(1,10)] float speed = 5;
    [SerializeField] float epsilon = 0.1f;

    void Start() {
        animating = false;

        rectTransform = GetComponent<RectTransform>();
        targetPos = rectTransform.anchoredPosition;
    }

    void Update() {
        if (!animating) return;
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();

        float exp = 1 - Mathf.Exp(-speed * Time.deltaTime);
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPos,
            exp
        );

        if (exp > 1-epsilon) {
            animating = false;
        }
    }
}
