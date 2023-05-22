using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicDragable : Dragable
{
    [SerializeField] UnityEvent onDragCallbacks;
    [SerializeField] UnityEvent onDropCallbacks;

    public override void OnDrag(GameObject target, DropTarget drop)
    {
        base.OnDrag(target, drop);
        onDragCallbacks.Invoke();
    }

    public override void OnDrop(DropTarget target)
    {
        base.OnDrop(target);
        onDropCallbacks.Invoke();
    }
}
