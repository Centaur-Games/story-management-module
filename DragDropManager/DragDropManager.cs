using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDropManager : MonoBehaviour {
    GraphicRaycaster raycaster;
    EventSystem eventSystem;

    Dragable currDragable;
    DropTarget lastTarget;
    bool lastClickDropped = false;

    List<RaycastResult> results = new List<RaycastResult>();

    void Start() {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (currDragable == null) {
                currDragable = getPointingDragable();

                if (currDragable == null) { // if still null after pointer raycast
                    lastClickDropped = true; // KeyUp should drop and call no events.
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (currDragable != null) {
                lastClickDropped = true;
                currDragable.onDropCancel();
                currDragable = null;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            if (currDragable != null && !lastClickDropped) {
                PollDropTarget();
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            if (lastClickDropped) {
                lastClickDropped = false;
                return;
            }

            if (currDragable != null) {
                currDragable.OnDrop(GetDropTarget());
                currDragable = null;
            }
        }
    }

    Dragable getPointingDragable() {
        Raycast();

        foreach (var result in results) {
            if (result.gameObject.tag != "dragable") continue;

            var dragable = result.gameObject.GetComponent<Dragable>();
            if (dragable.active) {
                return dragable;
            }
        }

        return null;
    }

    void PollDropTarget() {
        var tmpTarget = GetDropTarget();
        lastTarget = tmpTarget;

        currDragable?.OnDrag(lastTarget?.gameObject, lastTarget);
    }

    DropTarget GetDropTarget() {
        Raycast();

        foreach (var result in results) {
            if (result.gameObject.tag != "dropTarget") continue;

            var tmpRes = result.gameObject.GetComponent<DropTarget>();
            if (tmpRes.active) {
                return tmpRes;
            }
        }

        return null;
    }

    void Raycast() {
        var eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        results.Clear();
        raycaster.Raycast(eventData, results);
    }
}
