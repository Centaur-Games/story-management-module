using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DropdownSettings : MonoBehaviour
{
    bool attributesIsOpen = false;
    string getButtonName => attributesIsOpen ? "Değişkenleri Gizle" : "Değişkenleri Göster";

    [ShowIf("attributesIsOpen")][SerializeField] private Transform _content;
    [ShowIf("attributesIsOpen")][SerializeField] private RectTransform _scrollview;
    [ShowIf("attributesIsOpen")][SerializeField] private GameObject _buttonPrefab;
    [ShowIf("attributesIsOpen")][SerializeField] private Button _mainButton;
    [ShowIf("attributesIsOpen")][SerializeField] public GameObject expandImage;

    // Odin
    [Button("$getButtonName")]
    void setWindowState() {
        attributesIsOpen = !attributesIsOpen;
    }
    //

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
