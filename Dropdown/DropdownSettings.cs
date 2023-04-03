using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSettings : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private RectTransform _scrollview;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Button _mainButton;
    [SerializeField] public GameObject expandImage;
    
    private TextMeshProUGUI _mainButtonText;
    private RectTransform _mainButtonTransform;
    private RectTransform _contentTransform;

    public Transform content { get => _content; }
    public RectTransform contentTransform { get => _contentTransform; }
    public RectTransform scrollViewTransform { get => _scrollview; }
    public GameObject buttonPrefab { get => _buttonPrefab; }
    public Button mainButton { get => _mainButton; }
    public TextMeshProUGUI mainButtonText { get => _mainButtonText; }
    public RectTransform mainButtonTransform { get => _mainButtonTransform; }

    private void Awake()
    {
        _mainButtonText = _mainButton.GetComponentInChildren<TextMeshProUGUI>();
        _mainButtonTransform = _mainButton.GetComponent<RectTransform>();
        _contentTransform = _content.GetComponent<RectTransform>();
    }
}
