using UnityEngine;
using UnityEngine.Events;

public class BasicDropTarget : MonoBehaviour, IDropTargetListener, IInputValidateable {
    [SerializeField] Dragable targetDragable;
    [SerializeField] Dragable targetDragable2;
    [SerializeField] bool wouldAcceptDropped;
    [SerializeField] bool centerDropped;
    [SerializeField] bool RejectOnFail;

    [SerializeField] UnityEvent onSuccess;
    [SerializeField] UnityEvent onFail;
    [SerializeField] UnityEvent<Dragable> onHover;

    Dragable currDragable;
    bool _locked = false;

    bool IInputValidateable.correct => (currDragable == targetDragable || currDragable == targetDragable2) && currDragable != null;
    bool IInputValidateable.locked { get => _locked; set {
        _locked = value;
        currDragable?.SetActivation(!value);
    }}

    public void setTargetDragable(Dragable dragable) {
        targetDragable = dragable;
    }

    public void clearTargetDragable() {
        targetDragable = null;
    }

    public void reset() {
        if(currDragable is BasicDragable) {
            (currDragable as BasicDragable).reset();
            currDragable = null;
        }
    }

    void IDropTargetListener.OnDrop(Dragable dragable) {
        if (_locked || currDragable != null) {
            throw new DragableRejected();
        }

        if (dragable == targetDragable || dragable == targetDragable2) {
            onSuccess.Invoke();
        } else {
            onFail.Invoke();
            if(RejectOnFail) throw new DragableRejected();
        }

        if (wouldAcceptDropped) {
            if (centerDropped) {
                dragable.SetWorldSpaceTargetPos(transform.position);
                dragable.SetWorldSpaceStartPos(transform.position);
            }

            currDragable = dragable;

            return;
        }

        throw new DragableRejected();
    }

    void IDropTargetListener.OnHoverEnter(Dragable dragable) {
        onHover.Invoke(dragable);
    }

    void IDropTargetListener.OnElementDragStart(Dragable dragable) {
        currDragable = null;
    }

    void IDropTargetListener.OnElementDragCancel(Dragable dragable) {
        currDragable = dragable;
    }

    void IDropTargetListener.OnDropCancel(Dragable dragable) {}
    void IDropTargetListener.OnHoverExit(Dragable dragable) {}
    void IDropTargetListener.OnHoverStay(Dragable dragable) {}
    void IDropTargetListener.OnElementDragEnd(Dragable dragable, DropTarget droppedTarget) {}
}
