using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySequantial : MonoBehaviour
{
    [SerializeField] List<StorySequantialElement> elements = new List<StorySequantialElement>();

    bool locked = false;

    public void start() {
        if(locked) throw new Exception("Sequantial story is locked");
        locked = true;

        StartCoroutine(show(elements, false));
    }

    public void back() {
        Debug.Log("Geri basıldı");
        if(locked) throw new Exception("Sequantial story is locked");
        locked  = true;

        List<StorySequantialElement> _elements = new List<StorySequantialElement>();
        for(int i = elements.Count-1; i >= 0; i--) {
            _elements.Add(elements[i]);
        }
        StartCoroutine(show(_elements, true));
    }

    IEnumerator show(List<StorySequantialElement> _elements, bool isReverse) {
        foreach(var e in _elements) {
            yield return new WaitForSecondsRealtime(e.XSecondsAfter);

            if(isReverse) StoryManager.popStory(1, true);
            else StoryManager.pushNextState(true);

            yield return new WaitForSecondsRealtime(e.duration);
        }
        // if(isReverse) StoryManager.popStory(1, true);
        locked = false;
    }
}

[Serializable]
public class StorySequantialElement {
    public float XSecondsAfter;
    public float duration;
}
