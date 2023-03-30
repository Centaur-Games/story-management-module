using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    [SerializeField] private Dragable[] _dragableObjects;
    [SerializeField] private Dragable[] _dropableAreas;

    private Dragable _choosenDragable;
    private Vector3 _pastPosition;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var __item = CheckIfOnDragableObject();
            if (__item != null)
            {
                _choosenDragable = __item;
                _pastPosition = Input.mousePosition;
            }
            return;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (_choosenDragable == null)
                return;
            var move = Input.mousePosition - _pastPosition;
            _choosenDragable.rectTransform.position = _choosenDragable.rectTransform.position + move;
            _pastPosition = Input.mousePosition;
            return;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (_choosenDragable == null)
                return;
            var __dropable = CheckIfOnDropableArea(_choosenDragable.rectTransform.position, _choosenDragable.rectTransform.rect.size);
            if (__dropable != null)
            {
                if (__dropable.connectedItem == null)
                {
                    if (_choosenDragable.connectedItem != null)
                        _choosenDragable.connectedItem.connectedItem = null;
                    _choosenDragable.connectedItem = __dropable;
                    __dropable.connectedItem = _choosenDragable;
                }
            }
            _choosenDragable.rectTransform.position = _choosenDragable.backPosition;
            _choosenDragable = null;
        }
    }

    private Dragable CheckIfOnDragableObject()
    {
        foreach(var x in _dragableObjects)
        {
            if (x.CheckMouse())
            {
                return x;
            }
        }
        return null;
    }

    private Dragable CheckIfOnDropableArea()
    {
        foreach (var x in _dropableAreas)
        {
            if (x.CheckMouse())
            {
                return x;
            }
        }
        return null;
    }

    private Dragable CheckIfOnDropableArea(Vector2 position, Vector2 size)
    {
        foreach (var x in _dropableAreas)
        {
            if (x.CheckBox(position, size))
            {
                return x;
            }
        }
        return null;
    }
}
