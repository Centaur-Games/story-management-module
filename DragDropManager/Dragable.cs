using UnityEngine;

public class Dragable : MonoBehaviour {
    [SerializeField] bool freeDragable = false;

    Vector2 startPos;
    Vector2 targetPos;
    float lerpT = -1;

    (GameObject go, DropTarget target) hovering = (null, null);

    void Start() {
        startPos = transform.position;
    }

    void Update() {
        if (lerpT >= 0 && lerpT <= 1) {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime
            );

            lerpT+=Time.deltaTime;
        }
    }

    public void OnDrag(GameObject target, DropTarget drop) {
        if (target == null) {
            if (hovering.go != null) {
                hovering.target.callee.OnHoverExit(this);
                hovering = (null, null);
            }
        } else{
            if (hovering.go != null && hovering.go != target) {
                hovering.target.callee.OnHoverExit(this);
            } else if (hovering.go == target) {
                hovering.target.callee.OnHoverStay(this);
            }

            drop.callee.OnHoverEnter(this);
            hovering = (target, drop);
        }

        lerpT = -1;
        transform.position = Input.mousePosition;
    }

    public void OnDrop(DropTarget target) {
        hovering.target.callee.OnHoverExit(this);
        hovering = (null, null);

        if (target == null) {
            onDropCancel();
        }

        target.callee.OnDrop(this);
    }

    public void onDropCancel() {
        if (freeDragable) return;

        targetPos = startPos;
        lerpT = 0;
    }
}
