using UnityEngine;
using Sirenix.OdinInspector;

public class Dragable : MonoBehaviour {
    protected RectTransform rectTransform;

    [DetailedInfoBox("GameObject tag must be 'dragable'", "This script will not run when draggable tag is not given", InfoMessageType.Error, "tagControl")]
    [SerializeField] protected float returnSpeed = 1;
    [SerializeField] protected bool freeDragable = false;
    [SerializeField] public bool active = true;

    bool tagControl => gameObject.tag != "dragable";

    protected Vector3 startPos;
    protected Vector3 targetPos;
    protected DropTarget lastOwner;
    protected bool dragging;


    public DropTarget owner {
        get => lastOwner;
    }

    public bool draggingNow {
        get => dragging;
    }

    (GameObject go, DropTarget target) hovering = (null, null);

    protected virtual void Awake() {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        targetPos = rectTransform.anchoredPosition;
    }

    protected virtual void Start() {
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Update() {
        if (!dragging) {
            rectTransform.anchoredPosition = Vector3.Lerp(
                rectTransform.anchoredPosition,
                targetPos,
                1-Mathf.Exp(-returnSpeed * Time.deltaTime)
            );
        }
    }

    public virtual void OnDrag(GameObject target, DropTarget drop) {
        if (!dragging) {
            lastOwner?.callee.OnElementDragStart(this);
            dragging = true;
        }

        if (target == null) {
            if (hovering.go != null) {
                hovering.target.callee.OnHoverExit(this);
                hovering = (null, null);
            }
        } else {
            if (hovering.go != null && hovering.go != target) {
                hovering.target.callee.OnHoverExit(this);
                hovering = (null, null);
            } else if (hovering.go == target) {
                hovering.target.callee.OnHoverStay(this);
            } else {
                drop.callee.OnHoverEnter(this);
                hovering = (target, drop);
            }
        }

        transform.position = Input.mousePosition;
    }

    public virtual void OnDrop(DropTarget target) {
        if (hovering.go != null) {
            hovering.target.callee.OnHoverExit(this);
        }

        lastOwner?.callee.OnElementDragEnd(this, target);
        dragging = false;

        if (target == null) {
            onDropCancel();
        } else {
            var oldStart = startPos;
            var oldTarget = targetPos;
            var oldRectPos = rectTransform.position;

            try {
                startPos = rectTransform.anchoredPosition;
                targetPos = rectTransform.anchoredPosition;

                target.callee.OnDrop(this);
                lastOwner = target;
            } catch (DragableRejected) {
                startPos = oldStart;
                targetPos = oldTarget;
                rectTransform.position = oldRectPos;

                onDropCancel();
            }
        }

        hovering = (null, null);
    }

    public virtual void onDropCancel() {
        dragging = false;

        if (freeDragable) {
            targetPos = rectTransform.anchoredPosition;
            startPos = rectTransform.anchoredPosition;
            lastOwner = null;
        } else {
            targetPos = startPos;
        }

        lastOwner?.callee.OnElementDragCancel(this);
        hovering.target?.callee.OnDropCancel(this);
    }

    public void SetTargetPos(Vector3 pos) {
        targetPos = pos;
    }

    public void SetStartPos(Vector3 pos) {
        startPos = pos;
    }

    public void SetWorldSpaceTargetPos(Vector3 pos) {
        var oldPos = transform.position;

        transform.position = pos;
        targetPos = rectTransform.anchoredPosition;

        transform.position = oldPos;
    }

    public void SetWorldSpaceStartPos(Vector3 pos) {
        var oldPos = transform.position;

        transform.position = pos;
        startPos = rectTransform.anchoredPosition;

        transform.position = oldPos;
    }


    public void SetActivation(bool active)
    {
        this.active = active;
    }
}

[System.Serializable]
public class DragableRejected : System.Exception {
    public DragableRejected() { }
    public DragableRejected(string message) : base(message) { }
    public DragableRejected(string message, System.Exception inner) : base(message, inner) { }
    protected DragableRejected(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
