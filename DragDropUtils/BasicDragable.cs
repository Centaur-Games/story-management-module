using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicDragable : Dragable
{
    RectTransform rect;

    [Space]
    public float DragScale = 1;
    public float DropScale = 1;
    [Space]

    [SerializeField] UnityEvent onDragCallbacks;
    [SerializeField] UnityEvent onDropCallbacks;

    protected override void Start()
    {
        base.Start();
        if(active) setRectScale(DropScale);
    }

    public override void OnDrag(GameObject target, DropTarget drop)
    {
        base.OnDrag(target, drop);
        onDragCallbacks.Invoke();
        setRectScale(DragScale);
    }

    public override void OnDrop(DropTarget target)
    {
        base.OnDrop(target);
        onDropCallbacks.Invoke();
        if(active) setRectScale(DropScale);
    }

    void setRectScale(float scale) {
        if(rect == null) rect = GetComponent<RectTransform>();
        if(rect == null) return;
        rect.localScale = Vector3.one * scale;
    }
}
