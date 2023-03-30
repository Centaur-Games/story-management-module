using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class Dragable : MonoBehaviour
{
    private RectTransform _transform;
    public RectTransform rectTransform { get { return _transform; } }

    private Dragable _connectedItem;
    public Dragable connectedItem
    {
        get => _connectedItem;
        set
        {
            if (_onDrag != null)
            {
                _onDrag(value);
            }
            _connectedItem = value;
        }
    }

    private UnityAction<Dragable> _onDrag;

    public UnityAction<Dragable> onDrag
    {
        set
        {
            _onDrag = value;
        }
    }

    Vector3 _startPosition;
    public Vector3 backPosition
    {
        get
        {
            if (_connectedItem == null)
                return _startPosition;
            return _connectedItem.rectTransform.position;
        }
    }

    private void Start()
    {
        _transform = gameObject.GetComponent<RectTransform>();
        _startPosition = _transform.position;
    }

    public bool CheckMouse()
    {
        var __mousePosition = Input.mousePosition;
        var __transformPosition = _transform.position;
        var __width = _transform.rect.width / 2;
        var __height = _transform.rect.height / 2;
        return !(__transformPosition.x + __width < __mousePosition.x |
            __transformPosition.x - __width > __mousePosition.x |
            __transformPosition.y + __height < __mousePosition.y |
            __transformPosition.y - __height > __mousePosition.y);
    }

    public void GetBackToStart()
    {
        _connectedItem = null;
        _transform.position = _startPosition;
    }

    /// <summary>
    /// Bu objeyi bir dikdörtgen olarak hesaplayýp <paramref name="position"/> ve <paramref name="size"/> ile deðerleri verilen dikdörtgenle çarpýþtýðýný/[iç içe olduðunu] kontrol eder
    /// </summary>
    public bool CheckBox(Vector2 position, Vector2 size)
    {
        var __size = size / 2; // RectTransform boyutunu sað ve sola eþit olarak daðýtýyor. Mesela width deðeri 50 ise nesnenin boyutu saða doðru 25 sola doðru 25 olacaktýr
        /* 
         * Bir dikdörtgenin bütün noktalarýný hesaplayabilmek için 4 tane deðer yeterlidir
         * Koordinat sistemine dik olan bir dikdörtgenin koordinatlarýný hesaplayabilmek için 4 sayý yeterlidir
         * Örnek bir dikdörtgen: { (-1, -3), (-1, 2), (4, -3), (4, 2) }
         */
        float x11 = position.x - __size.x; 
        float x12 = position.x + __size.x;
        float y11 = position.y - __size.y;
        float y12 = position.y + __size.y;

        __size = new Vector2(_transform.rect.width / 2, _transform.rect.height / 2); // Bizim dikdörtgenimizin boyutunu hesaplama
        float x21 = _transform.position.x - __size.x;
        float x22 = _transform.position.x + __size.x;
        float y21 = _transform.position.y - __size.y;
        float y22 = _transform.position.y + __size.y;

        static bool CheckPoint(float px, float py, float x1, float x2, float y1, float y2) // Girilen (px, py) noktasýnýn { (x1, y1), (x1, y2), (x2, y1), (x2, y2) } dikdörtgeninin içinde olup olmadýðýný kontrol ediyor
        {
            return !(px < x1 | px > x2 | py < y1 | py > y2);
        }

        if (size.x * size.y > _transform.rect.width * _transform.rect.height) // hangi dikdörtgenin daha büyük olduðunu hesaplar
        {
            return CheckPoint(x21, y21, x11, x12, y11, y12) |
                CheckPoint(x22, y21, x11, x12, y11, y12) |
                CheckPoint(x21, y22, x11, x12, y11, y12) |
                CheckPoint(x22, y22, x11, x12, y11, y12);
        }
        return CheckPoint(x11, y11, x21, x22, y21, y22) |
            CheckPoint(x12, y11, x21, x22, y21, y22) |
            CheckPoint(x11, y12, x21, x22, y21, y22) |
            CheckPoint(x12, y12, x21, x22, y21, y22);
    }
}
