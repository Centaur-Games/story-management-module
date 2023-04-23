using UnityEngine;

public class NullDropTarget : MonoBehaviour, IDropTargetListener {
    public void OnDrop(Dragable dragable) {}
    public void OnDropCancel(Dragable dragable) {}
    public void OnElementDragCancel(Dragable dragable) {}
    public void OnElementDragEnd(Dragable dragable, DropTarget droppedTarget) {}
    public void OnElementDragStart(Dragable dragable) {}
    public void OnHoverEnter(Dragable dragable) {}
    public void OnHoverExit(Dragable dragable) {}
    public void OnHoverStay(Dragable dragable) {}
}
