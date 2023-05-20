using UnityEngine;

struct LastOp {
    public bool state;
    public string outAnim;
    public string entryAnim;

    public override bool Equals(object obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => base.ToString();

    public static bool operator ==(LastOp? obj1, LastOp? obj2) {
        if (!obj1.HasValue && !obj2.HasValue) return true;
        if (!obj1.HasValue || !obj1.HasValue) return false;

        if (obj1?.state != obj2?.state) return false;
        if (obj1?.outAnim != obj2?.outAnim) return false;
        if (obj1?.entryAnim != obj2?.entryAnim) return false;

        return true;
    }

    public static bool operator !=(LastOp? obj1, LastOp? obj2) =>
        !(obj1 == obj2);
}

[RequireComponent(typeof(Animator))]
public class AnimatedObject : MonoBehaviour
{
    [SerializeField] bool AnimPending = true;
    Animator animator;

    LastOp? lastOp = null;

    System.Action After;

    bool mustBeClosed = false;

    void OnEnable() {
        if(!mustBeClosed) return;

        Debug.Log(
            "Daha önceden kapatılamayan obje tespit edildi! Kapatılıyor..."
        );

        mustBeClosed = false;
        gameObject.SetActive(false);
    }

    void OnDisable() {
        if (StoryManager.pendingAnims.Contains(gameObject)) {
            StoryManager.pendingAnims.Remove(gameObject);
        }
    }

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void _pop(bool state, string outAnim, string entryAnim) {
        Debug.LogWarning($"_pop:: obj: {gameObject.name}, target: {state}, {outAnim}, {entryAnim}\n{new System.Diagnostics.StackTrace()}");

        if (state == gameObject.activeSelf) {
            Debug.Log($"{gameObject.name} objesi zaten {(state ? "açık" : "kapalı")}.");
            mustBeClosed = false;
            gameObject.SetActive(state);
            return;
        }

        LastOp currOp = new LastOp {
            state=state,
            outAnim=outAnim,
            entryAnim=entryAnim
        };

        if (StoryManager.pendingAnims.Contains(gameObject) && AnimPending) {
            if (currOp == lastOp) {
                return;
            }

            throw new AnimationPendingException(
                $"Global Animation Pending while switching: {gameObject.name}, targetState: {state}"
            );
        }

        lastOp = currOp;

        if (AnimPending) {
            StoryManager.pendingAnims.Add(gameObject);
        }

        if (!state) {
            mustBeClosed = true;

            if (!gameObject.activeInHierarchy ||
                !gameObject.activeSelf ||
                animator == null ||
                animator.runtimeAnimatorController == null
            ) {
                if (AnimPending) {
                    StoryManager.pendingAnims.Remove(gameObject);
                }

                mustBeClosed = false;
                gameObject.SetActive(false);

                Debug.Log(
                    $"{gameObject.name} adlı animasyonlu obje aktif degilken kapatildi ya da animatoru yok"
                );

                return;
            }

            animator.Play(outAnim);
        } else {
            gameObject.SetActive(true);

            if (animator?.runtimeAnimatorController != null) {
                animator.Play(entryAnim);
            } else {
                if (AnimPending) {
                    StoryManager.pendingAnims.Remove(gameObject);
                }
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
        After = after;
    }

    void closeObject() {
        mustBeClosed = false;
        cleansePendingAnim();

        gameObject.SetActive(false);

        if(After != null) {
            After();
            After = null;
        }
    }

    void disableObject() { mustBeClosed = false; gameObject.SetActive(false); }

    void closeAnimation() => cleansePendingAnim();
    void openObject() => cleansePendingAnim();

    void cleansePendingAnim() {
        lastOp = null;
        if (!AnimPending) return;

        if (!StoryManager.pendingAnims.Contains(gameObject)) {
            throw new AnimationEndedAlreadyException(
                $"AnimatedObject already removed from pendingAnims list: {gameObject.name}"
            );
        }

        StoryManager.pendingAnims.Remove(gameObject);
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

    public void goWorldSpace(Vector2 vector) {
        var oldPos = transform.position;

        transform.position = vector;
        targetPos = rectTransform.anchoredPosition;

        transform.position = oldPos;

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

[System.Serializable]
public class AnimationPendingException : System.Exception
{
    public AnimationPendingException() { }
    public AnimationPendingException(string message) : base(message) { }
    public AnimationPendingException(string message, System.Exception inner) : base(message, inner) { }
    protected AnimationPendingException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[System.Serializable]
public class AnimationEndedAlreadyException : System.Exception
{
    public AnimationEndedAlreadyException() { }
    public AnimationEndedAlreadyException(string message) : base(message) { }
    public AnimationEndedAlreadyException(string message, System.Exception inner) : base(message, inner) { }
    protected AnimationEndedAlreadyException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}