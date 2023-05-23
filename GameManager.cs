using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image MaximizeImage;

    public static bool galleryStateIsActive = false;
    public static GameManager instance;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GraphicRaycaster _raycaster;

    [HideInInspector] public static Canvas canvas => instance._canvas;
    [HideInInspector] public static EventSystem eventSystem => instance._eventSystem;
    [HideInInspector] public static GraphicRaycaster raycaster => instance._raycaster;

    void Awake() {
        if(instance != null) {
            Debug.LogWarning("GameManager birden çok kez başlatılmaya çalışıldı");
            return;
        }
        instance = this;
    }

    public static void SetGalleryStateActivate(bool isActive) {
        galleryStateIsActive = isActive;
    }

    public static void galleryStateContinue() {
        if(!galleryStateIsActive) return;

        galleryStateIsActive = false;
        StoryManager.pushNextState(true); 
    }

    public static void OpenMaximizeImage(Image image) {
        instance.MaximizeImage.transform.parent.gameObject.SetActive(true);
        instance.MaximizeImage.sprite = image.sprite;
    }

    public static void ResetLevel(int index) {
        SceneManager.LoadScene(index);
    }
}
