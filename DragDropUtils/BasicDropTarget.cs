using UnityEngine;
using UnityEngine.Events;

public class BasicDropTarget : MonoBehaviour, IDropTargetListener {
    [SerializeField] Dragable targetDragable;
    [SerializeField] UnityEvent onSuccess;
    [SerializeField] UnityEvent onFail;
    [SerializeField] UnityEvent<Dragable> onHover;

    public void setTargetDragable(Dragable dragable) {
        targetDragable = dragable;
    }

    public void clearTargetDragable() {
        targetDragable = null;
    }

    void IDropTargetListener.OnDrop(Dragable dragable) {
        if (dragable == targetDragable) {
            onSuccess.Invoke();
        } else {
            onFail.Invoke();
        }

        throw new DragableRejected();
    }

    void IDropTargetListener.OnHoverEnter(Dragable dragable) {
        onHover.Invoke(dragable);
    }

    void IDropTargetListener.OnDropCancel(Dragable dragable) {}
    void IDropTargetListener.OnElementDragCancel(Dragable dragable) {}
    void IDropTargetListener.OnElementDragEnd(Dragable dragable, DropTarget droppedTarget) {}
    void IDropTargetListener.OnElementDragStart(Dragable dragable) {}
    void IDropTargetListener.OnHoverExit(Dragable dragable) {}
    void IDropTargetListener.OnHoverStay(Dragable dragable) {}
}
