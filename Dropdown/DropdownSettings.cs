using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSettings : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Button _mainButton;
    
    private TextMeshProUGUI _mainButtonText;
    private RectTransform _mainButtonTransform;
    private RectTransform _contentTransform;

    public Transform content { get => _content; }
    public RectTransform contentTransform { get => _contentTransform; }
    public GameObject buttonPrefab { get => _buttonPrefab; }
    public Button mainButton { get => _mainButton; }
    public TextMeshProUGUI mainButtonText { get => _mainButtonText; }
    public RectTransform mainButtonTransform { get => _mainButtonTransform; }

    private void Start()
    {
        _mainButtonText = _mainButton.GetComponentInChildren<TextMeshProUGUI>();
        _mainButtonTransform = _mainButton.GetComponent<RectTransform>();
        _contentTransform = _content.GetComponent<RectTransform>();
    }
}
