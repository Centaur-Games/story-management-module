using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    void closeObject() {
        StoryManager.pendingAnims--;
        gameObject.SetActive(false);
    }

    void openObject() {
        StoryManager.pendingAnims--;
    }
}
