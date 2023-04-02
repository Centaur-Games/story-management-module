using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    [SerializeField] private RectTransform _textRectTransform;
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _positionSpeed;

    [SerializeField] private BubbleHorizontal _horizontal;
    [SerializeField] private BubbleVertical _vertical;

    private float _rotationT;
    private Vector3 _rotationStart;
    private Vector3 _rotationEnd;

    private float _positionT;
    private Vector3 _positionStart;
    private Vector3 _positionEnd;

    public BubbleHorizontal horizontal
    {
        get => _horizontal;
        set
        {
            _horizontal = value;
            if (value == BubbleHorizontal.Left)
                _rotationEnd.y = 180;
            else
                _rotationEnd.y = 0;
            _rotationStart = _rectTransform.localRotation.eulerAngles;
            _rotationT = 0;
            _chancingRotation = true;
        }
    }
    public BubbleVertical vertical
    {
        get => _vertical;
        set
        {
            _vertical = value;
            if (value == BubbleVertical.Bottom)
                _rotationEnd.x = 180;
            else
                _rotationEnd.x = 0;
            _rotationStart = _rectTransform.localRotation.eulerAngles;
            _rotationT = 0;
            _chancingRotation = true;
        }
    }

    public Vector3 positionDestination
    {
        get => _positionEnd;
        set
        {
            _positionStart = _rectTransform.localPosition;
            _positionEnd = value;
            _positionT = 0;
            _chancingPosition = true;
        }
    }

    private bool _chancingRotation = false;
    private bool _chancingPosition = false;

    void Start()
    {
        float __x = 180;
        float __y = 180;
        if (_vertical == BubbleVertical.Top)
            __x = 0;
        if (_horizontal == BubbleHorizontal.Right)
            __y = 0;
        _rectTransform.localRotation = Quaternion.Euler(__x, __y, 0);
        _textRectTransform.localRotation = Quaternion.Euler(__x, __y, 0);
    }

    void Update()
    {
        if (_chancingRotation)
            ChangeRotation();
        if (_chancingPosition)
            ChangePosition();
    }

    private void ChangeRotation()
    {
        if (_rotationT > 1)
        {
            _chancingRotation = false;
            return;
        }
        if (_rotationT > .5f)
            _textRectTransform.localRotation = Quaternion.Euler(_rotationEnd);
        _rectTransform.localRotation = Quaternion.Euler(Vector3.Lerp(_rotationStart, _rotationEnd, _rotationT));
        _rotationT += Time.deltaTime * _rotationSpeed;
    }

    private void ChangePosition()
    {
        if (_positionT > 1)
        {
            _chancingPosition = false;
            return;
        }
        _rectTransform.localPosition = Vector3.Lerp(_positionStart, _positionEnd, _positionT);
        _positionT += Time.deltaTime * _positionSpeed;
    }

    public void ChangeVertical(BubbleVertical vertical)
    {
        this.vertical = vertical;
    }

    public void ChangeHorizontal(BubbleHorizontal horizontal)
    {
        this.horizontal = horizontal;
    }

    public void SetX(float x)
    {
        positionDestination = new Vector3(x, positionDestination.y);
    }

    public void SetY(float y)
    {
        positionDestination = new Vector3(positionDestination.x, y);
    }
}

[System.ComponentModel.DefaultValue(BubbleVertical.Bottom)]
public enum BubbleVertical : byte
{
    Bottom = 1,
    Top = 3,
}

[System.ComponentModel.DefaultValue(BubbleHorizontal.Left)]
public enum BubbleHorizontal : byte
{
    Left = 1,
    Right = 3
}