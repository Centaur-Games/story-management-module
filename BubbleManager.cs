using System;
using UnityEngine;

public class BubbleManager : MonoBehaviour {
    [SerializeField] RectTransform _textRectTransform;
    [SerializeField] RectTransform _rectTransform;

    [SerializeField] float _rotationSpeed;
    [SerializeField] float _positionSpeed;

    [SerializeField] BubbleHorizontal _horizontal;
    public void SetHorizantalLeft() { this.horizontal = BubbleHorizontal.Left; }
    public void SetHorizantalRight() { this.horizontal = BubbleHorizontal.Right; }

    [SerializeField] BubbleVertical _vertical;
    public void SetVerticalBottom() { this.vertical = BubbleVertical.Bottom; }
    public void SetVerticalTop() { this.vertical = BubbleVertical.Top; }

    float rotationTime;
    Vector3 rotationStartAngles;
    Vector3 rotationTargetAngles;

    float positionTime;
    Vector3 positionStart;
    Vector3 positionTarget;

    bool changingRot = false;
    bool changingPos = false;

    public Vector3 positionDestination {
        get => positionTarget;
        set {
            positionStart = _rectTransform.localPosition;
            positionTarget = value;
            positionTime = 0;
            changingPos = true;
        }
    }

    public BubbleHorizontal horizontal {
        get => _horizontal;
        set {
            _horizontal = value;

            if (value == BubbleHorizontal.Left) {
                rotationTargetAngles.y = 180;
            } else {
                rotationTargetAngles.y = 0;
            }

            rotationStartAngles = _rectTransform.localRotation.eulerAngles;
            rotationTime = 0;

            changingRot = true;
        }
    }

    public BubbleVertical vertical {
        get => _vertical;
        set {
            _vertical = value;

            if (value == BubbleVertical.Bottom) {
                rotationTargetAngles.x = 180;
            } else {
                rotationTargetAngles.x = 0;
            }

            rotationStartAngles = _rectTransform.localRotation.eulerAngles;
            rotationTime = 0;

            changingRot = true;
        }
    }

    public void SetPosition(string vector) {
        var x = vector.Split("|");
        SetPosition(new Vector3(float.Parse(x[0]), float.Parse(x[1])));
    }

    public void SetPosition(Vector3 vector) {
        positionDestination = vector;
    }

    void Start() {
        float __x = 180;
        float __y = 180;

        if (_vertical == BubbleVertical.Top) {
            __x = 0;
        } if (_horizontal == BubbleHorizontal.Right) {
            __y = 0;
        }

        _rectTransform.localRotation = _textRectTransform.localRotation =
            Quaternion.Euler(__x, __y, 0);
    }

    void Update() {
        if (changingRot) ChangeRotation();
        if (changingPos) ChangePosition();
    }

    void ChangeRotation() {
        if (rotationTime > 1) {
            changingRot = false;
            return;
        }

        if (rotationTime > .5f) {
            _textRectTransform.localRotation = Quaternion.Euler(rotationTargetAngles);
        }

        _rectTransform.localRotation = Quaternion.Euler(
            Vector3.Lerp(
                rotationStartAngles,
                rotationTargetAngles,
                rotationTime
            )
        );

        rotationTime += Time.deltaTime * _rotationSpeed;
    }

    void ChangePosition() {
        if (positionTime > 1) {
            changingPos = false;
            return;
        }

        _rectTransform.localPosition = Vector3.Lerp(
            positionStart,
            positionTarget,
            positionTime
        );

        positionTime += Time.deltaTime * _positionSpeed;
    }
}

[Serializable]
[System.ComponentModel.DefaultValue(BubbleVertical.Bottom)]
public enum BubbleVertical : byte {
    Bottom = 1,
    Top = 3,
}

[Serializable]
[System.ComponentModel.DefaultValue(BubbleHorizontal.Left)]
public enum BubbleHorizontal : byte {
    Left = 1,
    Right = 3
}
