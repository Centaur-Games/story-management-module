using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IDropTargetListener {
    public void OnHoverEnter(Dragable dragable);
    public void OnHoverExit(Dragable dragable);
    public void OnHoverStay(Dragable dragable);
    public void OnDrop(Dragable dragable);
    public void OnDropCancel(Dragable dragable);
    public void OnClickDown();
}

public class DropTarget : MonoBehaviour {
    GraphicRaycaster raycaster;
    EventSystem eventSystem;

    [SerializeField] MonoBehaviour toBeCalled;

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
