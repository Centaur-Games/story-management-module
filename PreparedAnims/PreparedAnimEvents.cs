using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparedAnimEvents : MonoBehaviour
{
    public void pushState() {
        StoryManager.pushNextState(false);
    }

    public void prevState() {
        StoryManager.backStory(1);
    }
}
