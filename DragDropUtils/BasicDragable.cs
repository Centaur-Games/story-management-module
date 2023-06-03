using UnityEngine;
using UnityEngine.Events;

public class BasicDragable : Dragable
{
    RectTransform rect;

    [Space]
    public float DragScale = 1;
    public float DropScale = 1;
    [Space]

    Vector3 initialPos;

    [SerializeField] UnityEvent onDragCallbacks;
    [SerializeField] UnityEvent onDropCallbacks;

    void Awake() {
        initialPos = GetComponent<RectTransform>().anchoredPosition;
    }

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
        if(rect == null) rect = GetComponent<RectTransform>();
        if(rect == null) return;
        rect.localScale = Vector3.one * scale;
    }
}
