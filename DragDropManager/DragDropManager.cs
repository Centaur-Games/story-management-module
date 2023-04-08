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
            if (currDragable == null) getPointingDragable();

            if (currDragable == null) { // if still null after pointer raycast
                lastClickDropped = true; // KeyUp should drop and call no events.
            }

            getPointingDropTarget();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            if (lastClickDropped) {
                lastClickDropped = false;
                return;
            }

            if (currDragable != null) {
                currDragable.OnDrop(lastTarget);
                currDragable = null;
            }
        }
    }

    void getPointingDragable() {
        Raycast();

        foreach (var result in results) {
            if (result.gameObject.tag == "dragable") {
                var dragable = result.gameObject.GetComponent<Dragable>();
                currDragable = dragable;
                return;
            }
        }
    }

    void getPointingDropTarget() {
        Raycast();

        GameObject lastGameobject = null;

        foreach (var result in results) {
            if (result.gameObject.tag == "dropTarget") {
                var tmpTarget = result.gameObject.GetComponent<DropTarget>();

                if (currDragable == null) {
                    tmpTarget.callee.OnClickDown();
                }

                lastGameobject = result.gameObject;
                lastTarget = tmpTarget;
                break;
            }
        }

        currDragable.OnDrag(lastGameobject, lastTarget);
    }

    void Raycast() {
        var eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        results.Clear();
        raycaster.Raycast(eventData, results);
    }
}
