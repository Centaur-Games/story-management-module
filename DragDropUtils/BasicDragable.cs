using UnityEngine;
using UnityEngine.Events;

public class BasicDragable : Dragable
{
    [Space]
    public float DragScale = 1;
    public float DropScale = 1;
    [Space]

    Vector3 initialPos;

    [SerializeField] UnityEvent onDragCallbacks;
    [SerializeField] UnityEvent onDropCallbacks;

    public void reset() {
        startPos = initialPos;
        targetPos = initialPos;

        active = true;
        setRectScale(DropScale);
    }

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
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if(rectTransform == null) return;
        rectTransform.localScale = Vector3.one * scale;
    }
}
