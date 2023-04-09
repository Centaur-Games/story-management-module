using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GenericFeedback : MonoBehaviour {
    static GenericFeedback instance;
    public static GenericFeedback genericFeedback { get => instance; }

    [SerializeField] Animator animator;

    [SerializeField] GameObject _feedbackMenu;

    [SerializeField] Image _feedbackBackground;

    [SerializeField] TextMeshProUGUI _feedbackTitle;
    [SerializeField] TextMeshProUGUI _feedbackContent;

    [SerializeField] Button _feedbackButton;
    [SerializeField] TextMeshProUGUI _feedbackButtonText;

    [SerializeField] Sprite _redBackground;
    [SerializeField] Sprite _greenBackground;

    // please don't write names wrong.
    [SerializeField] GameObject[] succesObjects;
    [SerializeField] GameObject[] failedObjects;

    void Awake() { instance = this; }

    /// <summary>
    /// Tek butonlu açılır kapanır menüyü Çalıştırır
    /// </summary>
    /// <param name="title">üst kısımdaki siyahla yazılı başlık alanı</param>
    /// <param name="content">Başlık ile buton arasındaki beyaz içerik alanı</param>
    /// <param name="isSuccess"> İşlemin başarılı(true) mı yoksa 
    /// başarısız(false) mı olduğunu belirler. Eğer true döndürürse arkaplan
    /// yeşil olur ve button'a "Devam" yazar eğer false döndürülürse arkaplan
    /// kırmızı olur ve button'a "Tekrar Dene" yazar</param>
    /// <param name="onClick">Button'a tıklayınca çağırılacak aksiyonu belirler</param>
    void _Show(string title, string content, bool isSuccess, UnityAction onClick) {
        _feedbackButton.onClick.AddListener(onClick);
        _feedbackTitle.text = title;
        _feedbackContent.text = content;
        _feedbackMenu.SetActive(true);

        animator.Play("entry");

        if (isSuccess) {
            _feedbackBackground.sprite = _greenBackground;
            _feedbackButtonText.text = "Devam";
        } else {
            _feedbackBackground.sprite = _redBackground;
            _feedbackButtonText.text = "Tekrar Dene";
        }

        foreach(var e in isSuccess ? succesObjects : failedObjects) {
            e.SetActive(true);
        }

        foreach(var e in !isSuccess ? succesObjects : failedObjects) {
            e.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Açılır menüyü kapatır
    /// </summary>
    void _Close() {
        _feedbackButton.onClick.RemoveAllListeners();

        animator.Play("out");
    }

    public static void Show(string title, string content, bool isSuccess, UnityAction act) {
        instance._Show(title, content, isSuccess, act);
    }

    public static void Close() {
        instance._Close();
    }

    void openObject() {}
    void closeObject() {
        gameObject.SetActive(false);
    }
}
