using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DropdownMenu : MonoBehaviour
{
    [SerializeField] private DropdownSettings _datas;

    [SerializeField] private string[] _items;
    [SerializeField] private string _emptyText = "";

    [SerializeField] private float _spacing;

    private RectTransform _rectTransform;
    private Vector2 _startSize;

    private int _choosen = -1;
    private bool _open = false;

    public UnityEvent OnDropdownChoosed;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        var __content = _datas.content;
        var __button = _datas.mainButton;
        var __prefab = _datas.buttonPrefab;
        var __prefabTransform = __prefab.GetComponent<RectTransform>();
        var __height = __prefabTransform.sizeDelta.y + _spacing;

        __button.onClick.AddListener(ButtonClick);
        var ___height = new Vector2(0, __height * _items.Length);
        _datas.contentTransform.sizeDelta = ___height;
        var __nH = _datas.mainButtonTransform.rect.size + ___height;
        if (__nH.y < _rectTransform.sizeDelta.y)
            _startSize = __nH;
        else
            _startSize = _rectTransform.rect.size;

        for (int x = 0; x < _items.Length; x++)
        {
            var __item = Instantiate(__prefab, __content);
            
            var __itemTransform = __item.GetComponent<RectTransform>();
            __itemTransform.anchoredPosition = new Vector3(0, -__height * x);

            __item.GetComponentInChildren<TextMeshProUGUI>().text = _items[x];

            var __itemButton = __item.GetComponent<Button>();

            int y = x;
            __itemButton.onClick.AddListener(() =>
            {
                _choosen = y;
                _datas.mainButtonText.text = _items[y];
                OnDropdownChoosed.Invoke();
                ButtonClick();
            });
        }

        _datas.mainButtonText.text = _emptyText;
        _rectTransform.sizeDelta = _datas.mainButtonTransform.rect.size;
    }

    private void ButtonClick()
    {
        _open = !_open;
        if (_open)
        {
            _rectTransform.sizeDelta = _startSize;
            return;
        }
        _rectTransform.sizeDelta = _datas.mainButtonTransform.rect.size;
    }

    public string GetChoosenString()
    {
        if (_choosen == -1)
            return "";
        return _items[_choosen];
    }

    public void ResetValue()
    {
        _choosen = -1;
        _datas.mainButtonText.text = _emptyText;
    }

    public int GetChoosenIndex()
    {
        return _choosen;
    }
}
