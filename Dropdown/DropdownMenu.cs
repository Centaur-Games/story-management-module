using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DropdownMenu : MonoBehaviour
{
    bool isLocked = false;

    [SerializeField] public string correctAnswer;
    [SerializeField] private DropdownSettings _datas;
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color NonSelectColor;

    [SerializeField] private string[] _items;
    [SerializeField] private string _emptyText = "";

    [SerializeField] private float _spacing;

    [SerializeField] AnimatedObject[] affecteds;

    Button selectedButton;

    private RectTransform _rectTransform;
    private Vector2 _startSize;

    private int _choosen = -1;
    private bool _open = false;

    public UnityEvent OnDropdownChoosed;

    private void Start() {
        _rectTransform = GetComponent<RectTransform>();

        var __prefabTransform = _datas.buttonPrefab.GetComponent<RectTransform>();
        var __height = __prefabTransform.sizeDelta.y + _spacing;

        _datas.mainButton.onClick.AddListener(ButtonClick);
        var ___height = new Vector2(0, __height * _items.Length);

        _datas.contentTransform.sizeDelta = ___height;
        _datas.scrollViewTransform.sizeDelta = new Vector2(_datas.scrollViewTransform.sizeDelta.x, __height * _items.Length);

        var __nH = _datas.mainButtonTransform.rect.size + ___height;

        if (__nH.y < _rectTransform.sizeDelta.y) _startSize = __nH;
        else _startSize = _rectTransform.rect.size;

        for (int x = 0; x < _items.Length; x++) {
            var __item = Instantiate(_datas.buttonPrefab, _datas.content);

            var __itemTransform = __item.GetComponent<RectTransform>();
            __itemTransform.anchoredPosition = new Vector3(0, -__height * x);

            __item.GetComponentInChildren<TextMeshProUGUI>().text = _items[x];

            var __itemButton = __item.GetComponent<Button>();

            int y = x;

            
            __itemButton.onClick.AddListener(() => {
                if(isLocked) throw new System.Exception("Bu öğe kilitli");

                if(selectedButton != null) selectedButton.gameObject.GetComponent<Image>().color = NonSelectColor;
                selectedButton = __itemButton;
                selectedButton.gameObject.GetComponent<Image>().color = SelectedColor;

                _choosen = y;
                _datas.mainButtonText.text = _items[y];
                OnDropdownChoosed.Invoke();
                ButtonClick();
            });
        }

        _datas.mainButtonText.text = _emptyText;
        _rectTransform.sizeDelta = _datas.mainButtonTransform.rect.size;
    }

    public void ButtonClick() {
        // animation
        try {
            foreach(var e in  affecteds) e.playAnim(!_open ? "entry" : "out", () => {
                if(!_open) _rectTransform.sizeDelta = _datas.mainButtonTransform.rect.size;
            }, false);
        } catch {
            Debug.Log("Dropdown hızdan dolayı kısıtlandı");
            return;
        }

        _open = !_open;

        if (_open) _rectTransform.sizeDelta = _startSize;
    }

    public void closeAll(Button button) {
        _open = true;
        if(((Vector2)Input.mousePosition).CheckBox(_rectTransform.position, _rectTransform.sizeDelta, _rectTransform.pivot.x, _rectTransform.pivot.y)) {
            button.Select();
            return;
        }
        ButtonClick();
    }

    public string GetChoosenString() {
        if (_choosen == -1)
            return "";
        return _items[_choosen];
    }

    public void ResetValue() {
        if(isLocked) throw new System.Exception("Bu öğe kilitli");

        _choosen = -1;
        _datas.mainButtonText.text = _emptyText;
    }

    public int GetChoosenIndex() {
        return _choosen;
    }

    public void UnLock() {
        if(!isLocked) { Debug.LogWarning("Bu öğe zaten açık ama tekrardan açıyorum"); }
        isLocked = false;

        _datas.mainButton.interactable = true;
        _datas.expandImage.SetActive(true);

        _open = true;
        ButtonClick();
    }

    public void Lock() {
        if(isLocked) { Debug.LogWarning("Bu öğe zaten kilitli ama tekrardan kilitliyorum"); }
        isLocked = true;

        _datas.mainButton.interactable = false;
        _datas.expandImage.SetActive(false);

        _open = true;
        ButtonClick();
    }
}
