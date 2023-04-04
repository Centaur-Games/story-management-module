using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public struct BubbleVertAlignment {
    public BubbleVertical vert;
    public BubbleHorizontal hor;
}

[System.Serializable]
public struct DialogData {
    public DialogManager d_mngr;
    public BubbleManager b_mngr;
    public int index;
    public Vector2 pos;
    public BubbleVertAlignment alignment;
}

/// <summary>
/// StoryState storylerin duruma göre açık tuttuğu objeler ve diyalogları,
/// sonraki aktivite ve hikayeyi belirleyen bir veri tipidir.
/// </summary>
[System.Serializable]
public struct StoryState {
    /// <summary>Bu durumun bağlı olduğu story</summary>
    [HideInInspector] public Story owner;

    [ShowIf("canVisible")]
    /// <summary>Bu durum pushlandığında yüklenmesi beklenen image</summary>
    public Sprite bgSprite;

    [ShowIf("canVisible")]
    /// <summary>Eklemeleli olarak açılacak objeler, state poplandığında kapatılır.</summary>
    public AnimatedObject[] iActiveObjects;

    [ShowIf("canVisible")]
    /// <summary>the dialog calls</summary>
    public DialogData[] iActiveDialogs;

    [ShowIf("canVisible")]
    /// <summary>Zorla açılacak objeler, state poplandığında kapatılmaz.</summary>
    public AnimatedObject[] iForcedActiveObjects;

    [ShowIf("canVisible")]
    /// <summary>Zorla kapatılacak objeler</summary>
    public AnimatedObject[] iForcedClosedObjects;

    [ShowIf("canVisible")]
    /// <summary>State poplandığında galeriye eklenecek fotograf/fotograflar</summary>
    public Image[] pushToGalleryAfter;

    /// <summary>Storylerin switchToNextState'i tarafından kullanılan, history
    /// değişikliklerinde iç sayacın bozulmaması için kullanılan sayaç</summary>
    public int? stateCounter;

    [ShowIf("canVisible")]
    /// <summary> storymanager tarafidan yapilan state degisikliklerinde cagirilacak dinleyiciler</summary>
    public StoryPushListeners listeners;

    /// <summary>switchToNextState cağırıldığında eğer nextActivity yok ise pushlanacak olan story</summary>
    public Story nextStory;

    //// <summary>the restoring state for the pushed nextActivity to return to</summary>
    public PusherState pusherState;

    bool canVisible() {
        return nextStory == null;
    }
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
        openAndSetDialogs(state ?? defaultState, forced);
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
        closeDialogs((StoryState)currState, closeForced);

        if (closeForced) {
            closeForcedActives((StoryState)currState, true);
            openForceCloseds((StoryState)currState, true);
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

    void openForceCloseds(StoryState state, bool forced=true) {
        if (forced) {
            foreach(var obj in state.iForcedClosedObjects) {
                obj.SetActivePop(true);
            }
        } else {
            foreach(var obj in state.iForcedClosedObjects) {
                obj.SetActive(true);
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

    void openAndSetDialogs(StoryState state, bool forced=false) {
        foreach (var dialog in state.iActiveDialogs) {
            dialog.d_mngr.open(dialog.index);

            if (dialog.b_mngr.horizontal != dialog.alignment.hor) {
                Debug.Log($"hor set to {dialog.alignment.hor} from {dialog.b_mngr.horizontal}");
                dialog.b_mngr.horizontal = dialog.alignment.hor;
            }

            if (dialog.b_mngr.vertical != dialog.alignment.vert) {
                Debug.Log($"vert set to {dialog.alignment.vert} from {dialog.b_mngr.vertical}");
                dialog.b_mngr.vertical = dialog.alignment.vert;
            }

            if (dialog.b_mngr.positionDestination != (Vector3)dialog.pos) {
                dialog.b_mngr.SetPosition(dialog.pos);
            }
        }
    }

    void closeDialogs(StoryState state, bool forced=false) {
        foreach (var dialog in state.iActiveDialogs) {
            // TODO: dialog.d_mngr.close(dialog.index);
        }
    }

    #endregion
}