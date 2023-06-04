using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HelpButtonPage {
    public List<AnimatedObject> objectsToOpen;
}

public class HelpScreenController : MonoBehaviour {
    [SerializeField] List<HelpButtonPage> pages;

    HelpButtonPage? currentPage;

    public void setPage(int index) {
        if (currentPage != null) {
            foreach (var obj in currentPage.Value.objectsToOpen) {
                obj.SetActive(false);
            }
        }

        currentPage = pages[index];

        foreach (var obj in currentPage?.objectsToOpen) {
            obj.SetActive(true);
        }
    }
}