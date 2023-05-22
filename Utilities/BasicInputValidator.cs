using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BasicInputValidator : MonoBehaviour {
    [SerializeField] UnityEvent wrongCallbacks;
    [SerializeField] UnityEvent correctCallbacks;

    IEnumerable<GameObject> childOf(Transform obj) {
        for (int i = 0; i < obj.childCount; i++) {
            yield return obj.GetChild(i).gameObject;
        }
    }

    public void doAutoCheck() {
        doAutoCheck(transform);
    }

    public void doAutoCheck(Transform obj) {
        int depth = 0;
        bool compDone = false;
        doAutoCheck(transform, ref depth, ref compDone);
    }

    public void doAutoCheck(Transform obj, ref int depth, ref bool compDone) {
        depth++;

        foreach (var child in childOf(obj)) {
            if (child.activeInHierarchy && child.activeSelf) {
                var validateable = child.GetComponent<IInputValidateable>();

                if (validateable != null) {
                    if (!validateable.correct && !validateable.locked) {
                        wrongCallbacks.Invoke();
                        throw new System.Exception("wrong ans");
                    }

                    if (!validateable.locked) {
                        compDone |= true;
                        validateable.locked = true;
                    }
                }

                doAutoCheck(child.transform, ref depth, ref compDone);
            }
        }

        if (depth == 1 && compDone) {
            correctCallbacks.Invoke();
        }

        depth--;
    }
}
