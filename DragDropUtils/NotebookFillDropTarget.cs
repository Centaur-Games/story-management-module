using UnityEngine;

[RequireComponent(typeof(DropTarget))]
public class NotebookFillDropTarget : MonoBehaviour, IDropTargetListener, IInputValidateable {
    [SerializeField] string answer;
    [SerializeField] Color neutralColor;
    [SerializeField] Color correctColor;
    [SerializeField] Color wrongColor;

    DropTarget _dropTarget;

    void Awake() {
        _dropTarget = GetComponent<DropTarget>();
    }

    public bool correct { get {
        var correct = lastDropped?.stringData == answer;

        if (lastDropped != null) {
            lastDropped.SetColor(correct ? correctColor : wrongColor);
        }

        return lastDropped?.stringData == answer;
    }}

    public bool locked { get => lastDropped?.locked ?? false; set {
        if (!value) {
            lastDropped.SetColor(neutralColor);
        } else {
            lastDropped.SetColor(correct ? correctColor : wrongColor);
        }

        lastDropped!.locked = value;
    }}

    NotebookFillDragable lastDropped;

    public void OnDrop(Dragable dragable) {
        if (lastDropped != null) {
            throw new DragableRejected();
        }

        var d = dragable.GetComponent<NotebookFillDragable>();

        if (d == null) {
            throw new DragableRejected();
        }

        lastDropped = d;

        dragable.SetWorldSpaceTargetPos(transform.position);
    }

    public void OnElementDragEnd(Dragable dragable, DropTarget droppedTarget) {
        if (droppedTarget != null) {
            lastDropped = null;
        }

        if (droppedTarget == _dropTarget) {
            dragable.SetWorldSpaceTargetPos(transform.position);
        }
    }

    public void OnElementDragStart(Dragable dragable) {
        lastDropped.SetColor(neutralColor);
    }

    public void OnElementDragCancel(Dragable dragable) {
        lastDropped = dragable.GetComponent<NotebookFillDragable>();
        lastDropped.SetColor(neutralColor);

        dragable.SetWorldSpaceTargetPos(transform.position);
    }

    public void OnDropCancel(Dragable dragable) {}
    public void OnHoverEnter(Dragable dragable) {}
    public void OnHoverExit(Dragable dragable) {}
    public void OnHoverStay(Dragable dragable) {}
}
