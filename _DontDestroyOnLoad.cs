using UnityEngine;

public class _DontDestroyOnLoad : MonoBehaviour {
    void Start() {
        DontDestroyOnLoad(gameObject);
    }
}
