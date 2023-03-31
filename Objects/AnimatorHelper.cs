using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    void closeObject() {
        StoryManager.pendingAnims--;
        gameObject.SetActive(false);
    }
}
