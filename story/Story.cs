using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// StoryState storylerin duruma göre açık tuttuğu objeler ve diyalogları,
/// sonraki aktivite ve hikayeyi belirleyen bir veri tipidir.
/// </summary>
[System.Serializable]
public struct StoryState {
    /// <summary>Bu durumun bağlı olduğu story</summary>
    [HideInInspector] public Story owner;
    /// <summary>Bu durum pushlandığında yüklenmesi beklenen image</summary>
    public Sprite bgSprite;

    /// <summary>Eklemeleli olarak açılacak objeler, state poplandığında kapatılır.</summary>
    public AnimatedObject[] iActiveObjects;

    /// <summary>Zorla açılacak objeler, state poplandığında kapatılmaz.</summary>
    public AnimatedObject[] iForcedActiveObjects;

    /// <summary>Zorla kapatılacak objeler</summary>
    public AnimatedObject[] iForcedClosedObjects;

    /// <summary>State poplandığında galeriye eklenecek fotograf/fotograflar</summary>
    public Image[] pushToGalleryAfter;

    /// <summary>Storylerin switchToNextState'i tarafından kullanılan, history
    /// değişikliklerinde iç sayacın bozulmaması için kullanılan sayaç</summary>
    public int? stateCounter;

    /// <summary> storymanager tarafidan yapilan state degisikliklerinde cagirilacak dinleyiciler</summary>
    public StoryPushListeners listeners;

    /// <summary>switchToNextState cağırıldığında eğer nextActivity yok ise pushlanacak olan story</summary>
    public Story nextStory;

    //// <summary>the restoring state for the pushed nextActivity to return to</summary>
    public PusherState pusherState;
}

[System.Serializable]
public struct StoryPushListeners {
    //// <summary>callbacks to be called on push</summary>
    public UnityEvent<StoryState?> onPush;
    //// <summary>callbacks to be called on pop</summary>
    public UnityEvent<StoryState?> onPop;

    //// <summary>callbacks to be called on push</summary>
    public UnityEvent<StoryState?> onForcedPush;
    //// <summary>callbacks to be called on pop</summary>
    public UnityEvent<StoryState?> onForcedPop;
}

public class PusherState {
    public StoryState state;
}

public class Story : MonoBehaviour {
    /// <summary>Bu storyinin backgroundu</summary>
    [SerializeField] Image bgImage;

    /// <summary>Bu storynin içerdiği durumlar.</summary>
    [SerializeField] StoryState[] states;

    /// <summary>Bu objenin herhangi bir state verilmeden pushlanması durumunda kullanılacak olan
    /// states dizisinin ilk elamanını dönden varsayılan durum</summary>
    public StoryState defaultState {
        get {
            var tmp = states[0];
            tmp.owner = this;
            return tmp;
        }
    }

    /// <summary>Bu objeye en son push çağrısında verilen statedir.</summary>
    StoryState? currState;

    void Awake() {
        for (var i=0; i<states.Length; i++) {
            states[i].owner = this;
            states[i].stateCounter = i;
        }

        currState = null;
    }

    /// <summary>
    /// StoryManager tarafından bu story objesinin durumlarından birisinin geçmişe eklenmesi durumunda çağırılır.
    /// </summary>
    /// <param name="state">StoryManager tarafından gönderilen state</param>
    public void onPush(StoryState? state, bool forced=false) {
        openForcedActives(state ?? defaultState, forced);
        closeForceCloseds(state ?? defaultState, forced);
        openActives(state ?? defaultState, forced);
        setBackgroundImage(state ?? defaultState);

        currState = state ?? defaultState;

        if (forced) {
            currState?.listeners.onForcedPush.Invoke(currState);
        } else {
            currState?.listeners.onPush.Invoke(currState);
        }
    }

    /// <summary>
    /// StoryManager tarafından bu story objesinin statelerinden birinin geçmişten çıkarılması durumunda çağırılır.
    /// </summary>
    /// <param name="closeForced">Normal ileriye doğru sayfa değişimi olmadığında
    /// zorla açık tutulan objelerin kapatılıp kapatılmayacağını belirlemek 
    /// için StoryManager tarafından kullanılır</param>
    public void onPop(bool closeForced=false) {
        if (currState == null) {
            throw new System.Exception("popped already");
        }

        closeActives((StoryState)currState, closeForced);

        if (closeForced) {
            closeForcedActives((StoryState)currState, true);
        }

        if (closeForced) {
            currState?.listeners.onForcedPop.Invoke(currState);
        } else {
            currState?.listeners.onPop.Invoke(currState);
        }

        currState = null;
    }

    /// <summary>
    /// Bağlı olduğu story objesinin sonraki storysinin ya da aktivitesini
    /// StoryManager'e pushlar
    /// </summary>
    public void switchToNextState(bool ignorePendingAnims=false) {
        if (StoryManager.pendingAnims > 0 && !ignorePendingAnims) {
            throw new System.Exception("animation pending.");
        }

        if (currState == null) {
            throw new System.Exception("not pushed");
        }

        StoryState c = (StoryState)currState;

        var tmp = states[(int)c.stateCounter+1];

        if (tmp.nextStory != null) {
            var state = tmp.nextStory.defaultState;
            state.pusherState = new PusherState{state=tmp};
            StoryManager.pushStory(state, ignorePendingAnims);
        }

        else {
            tmp.owner = this;
            tmp.pusherState = new PusherState{state=c};

            StoryManager.pushStory(tmp, ignorePendingAnims);
        }
    }

    #region util

    void setBackgroundImage(StoryState state) {
        if (bgImage == null) {
            return;
        }

        if(state.bgSprite != null) {
            bgImage.sprite = state.bgSprite;
        }
    }

    void openActives(StoryState state, bool forced=false) {
        if (forced) {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActivePop(true);
            }
        } else {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActive(true);
            }
        }
    }

    void closeActives(StoryState state, bool forced=false) {
        if (forced) {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActivePop(false);
            }
        } else {
            foreach(var obj in state.iActiveObjects) {
                obj.SetActive(false);
            }
        }
    }

    void openForcedActives(StoryState state, bool forced=false) {
        if (forced) {
            foreach(var obj in state.iForcedActiveObjects) {
                obj.SetActivePop(true);
            }
        } else {
            foreach(var obj in state.iForcedActiveObjects) {
                obj.SetActive(true);
            }
        }
    }

    void closeForcedActives(StoryState state, bool forced=false) {
        if (forced) {
            foreach(var obj in state.iForcedActiveObjects) {
                obj.SetActivePop(false);
            }
        } else {
            foreach(var obj in state.iForcedActiveObjects) {
                obj.SetActive(false);
            }
        }
    }

    void closeForceCloseds(StoryState state, bool forced=false) {
        if (forced) {
            foreach (var obj in state.iForcedClosedObjects) {
                obj.SetActivePop(false);
            }
        } else {
            foreach(var obj in state.iForcedClosedObjects) {
                obj.SetActive(false);
            }
        }
    }

    #endregion
}