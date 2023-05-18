using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PourTarget : MonoBehaviour, IDropTargetListener {
    [SerializeField] float reserveFillAmount;
    [SerializeField] Image reserve;
    [SerializeField] List<Pourable> acceptedPourables;

    [SerializeField] UnityEvent invokeOnFill;
    [SerializeField] float fillEpsilon = 0.05f;

    public bool CheckPoured() {
        return (reserve.fillAmount+fillEpsilon) >= reserveFillAmount;
    }

    void IDropTargetListener.OnDrop(Dragable dragable) {
        throw new DragableRejected();
    }

    void IDropTargetListener.OnDropCancel(Dragable dragable) {
        if (CheckPoured()) {
            invokeOnFill.Invoke();
        }

        if (dragable is Pourable && acceptedPourables.Contains((Pourable)dragable)) {
            ((Pourable)dragable).StopPour();
        }
    }

    void IDropTargetListener.OnHoverStay(Dragable dragable) {
        if (dragable is Pourable && acceptedPourables.Contains((Pourable)dragable)) {
            var amount = ((Pourable)dragable).pourReserveTick();
            reserve.fillAmount += amount * reserveFillAmount;
        }
    }

    void IDropTargetListener.OnHoverEnter(Dragable dragable) {
        if (dragable is Pourable && acceptedPourables.Contains((Pourable)dragable)) {
            ((Pourable)dragable).StartPour();
        }
    }

    void IDropTargetListener.OnHoverExit(Dragable dragable) {
        if (dragable is Pourable && acceptedPourables.Contains((Pourable)dragable)) {
            ((Pourable)dragable).StopPour();
        }
    }

    public void resetFilledAmount() {
        reserve.fillAmount = 0;
    }

    void IDropTargetListener.OnElementDragCancel(Dragable dragable) {}
    void IDropTargetListener.OnElementDragEnd
        (Dragable dragable, DropTarget droppedTarget) {}
    void IDropTargetListener.OnElementDragStart(Dragable dragable) {}
}
