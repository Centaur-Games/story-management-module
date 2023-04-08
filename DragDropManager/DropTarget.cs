using UnityEngine;

public interface IDropTargetListener {
    /// <summary>Called once when a Dragable object holding mouse pointer enters the target</summary>
    public void OnHoverEnter(Dragable dragable);

    /// <summary>Called once when a Draggable object leaved the drop targets area
    /// (belonging(dragging in and out a belonging object) or not) and when a draggable is dropped</summary>
    public void OnHoverExit(Dragable dragable);

    /// <summary>Called every update when a draggable travels inside</summary>
    public void OnHoverStay(Dragable dragable);

    /// <summary>Called once when a draggable(belonging or not) is dropped onto the area,
    /// you can throw an exception if you want to reject this object
    /// (belonging objects rejection does not cause the element to be orphaned
    /// if listener is applied correctly).</summary>
    public void OnDrop(Dragable dragable); // you shall throw if you reject

    /// <summary>Called when an entered object has cancelled or rejected by.</summary>
    public void OnDropCancel(Dragable dragable);

    /// <summary>Called when a belonging has been started to drag</summary>
    public void OnElementDragStart(Dragable dragable);
    /// <summary>Called when a belonging object has been dropped</summary>
    public void OnElementDragEnd(Dragable dragable, DropTarget droppedTarget);
    /// <summary>Called when a belonging object has been cancelled</summary>
    public void OnElementDragCancel(Dragable dragable);
}

public class DropTarget : MonoBehaviour {
    [SerializeField] MonoBehaviour toBeCalled;
    public bool active;

    void Start() {
        if (!(toBeCalled is IDropTargetListener)) {
            throw new System.Exception(
                $"{gameObject.name} has a non IDropTargetListener callee");
        }
    }

    public IDropTargetListener callee {
        get => (IDropTargetListener)toBeCalled;
    }
}
