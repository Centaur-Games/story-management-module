using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GenericFeedback : MonoBehaviour
{
    private static GenericFeedback _genericFeedback;
    public static GenericFeedback genericFeedback
    {
        get => _genericFeedback;
    }

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject _feedbackMenu;
    [SerializeField] private Image _feedbackBackground;
    [SerializeField] private TextMeshProUGUI _feedbackTitle;
    [SerializeField] private TextMeshProUGUI _feedbackContent;
    [SerializeField] private Button _feedbackButton;
    [SerializeField] private TextMeshProUGUI _feedbackButtonText;

    [SerializeField] private Sprite _redBackground;
    [SerializeField] private Sprite _greenBackground;

    private void Awake() { _genericFeedback = this; }

    /// <summary>
    /// Tek butonlu a��l�r kapan�r men�y� �al��t�r�r<br /><br />
    /// <paramref name="title"/> �st k�s�mdaki siyahla yaz�l� ba�l�k alan�<br />
    /// <paramref name="content"/> �st k�s�mdaki siyahla yaz�l� ba�l�k alan�<br />
    /// <paramref name="isSuccess"/> ��in ba�ar�l�(true) m� yoksa ba�ar�s�z(false) m� oldu�unu belirler. e�er true d�nd�r�l�rse arkaplan ye�il olur ve button'a "Devam" yazar e�er false d�nd�r�l�rse arkaplan k�rm�z� olur ve button'a "Tekrar Dene" yazar<br />
    /// <paramref name="onClick"/> Button'a t�klay�nca �al��acak etkinli�i belirler
    /// </summary>
    /// <param name="title">�st k�s�mdaki siyahla yaz�l� ba�l�k alan�</param>
    /// <param name="content">Ba�l�k ile buton aras�ndaki beyaz i�erik alan�</param>
    /// <param name="isSuccess"> ��in ba�ar�l�(true) m� yoksa ba�ar�s�z(false) m� oldu�unu belirler. e�er true d�nd�r�l�rse arkaplan ye�il olur ve button'a "Devam" yazar e�er false d�nd�r�l�rse arkaplan k�rm�z� olur ve button'a "Tekrar Dene" yazar</param>
    /// <param name="onClick">Button'a t�klay�nca �al��acak etkinli�i belirler</param>
    public static void Show(string title, string content, bool isSuccess, UnityAction onClick)
    {
        var gf = _genericFeedback;
        gf._feedbackButton.onClick.AddListener(onClick);
        gf._feedbackTitle.text = title;
        gf._feedbackContent.text = content;
        gf._feedbackMenu.SetActive(true);
        genericFeedback.animator.Play("entry");
        if (isSuccess)
        {
            gf._feedbackBackground.sprite = gf._greenBackground;
            gf._feedbackButtonText.text = "Devam";
            return;
        }
        gf._feedbackBackground.sprite = gf._redBackground;
        gf._feedbackButtonText.text = "Tekrar Dene";
    }

    /// <summary>
    /// A��lm�� men�y� kapat�r
    /// </summary>
    public static void Close()
    {
        var gf = _genericFeedback;
        gf._feedbackButton.onClick.RemoveAllListeners();
        genericFeedback.animator.Play("out");
    }
}
