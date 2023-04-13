using UnityEngine;

public class Dragable : MonoBehaviour {
    RectTransform rectTransform;

    [SerializeField] bool freeDragable = false;
    [SerializeField] float returnSpeed = 1;

    [SerializeField] public bool active = true;

    Vector3 startPos;
    Vector3 targetPos;
    DropTarget lastOwner;
    bool dragging;

    (GameObject go, DropTarget target) hovering = (null, null);

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        targetPos = rectTransform.anchoredPosition;
    }

    void Update() {
        if (!dragging) {
            rectTransform.anchoredPosition = Vector3.Lerp(
                rectTransform.anchoredPosition,
                targetPos,
                1-Mathf.Exp(-returnSpeed * Time.deltaTime)
            );
        }
    }

    public void OnDrag(GameObject target, DropTarget drop) {
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

    public void OnDrop(DropTarget target) {
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
                startPos = Input.mousePosition;
                targetPos = Input.mousePosition;
                rectTransform.position = Input.mousePosition;

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

    public void onDropCancel() {
        dragging = false;

        if (freeDragable) {
            rectTransform.position = Input.mousePosition;
            targetPos = Input.mousePosition;
            startPos = Input.mousePosition;
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
