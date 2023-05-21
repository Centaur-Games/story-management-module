using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this script non support to back stories
public class CheckMultipleState : MonoBehaviour
{
    [SerializeField] int StateCount = 0;
    [SerializeField] UnityEvent onComplete;

    int count = 0;

    public void onSucces() {
        count++;
        if(StateCount == count && onComplete != null) {
            onComplete.Invoke();
        }
    }
}
