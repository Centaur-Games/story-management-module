using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour {
    [SerializeField] Story initialStory;
    public static StoryState? initialState;

    static Stack<StoryState> storyStack;
    public static StoryManager instance;

    public static List<GameObject> pendingAnims = new();

    public static string getPendingAnims() {
        string _out = "\n";

        pendingAnims.ForEach((e) => {
            _out += "  " + e.name;
            _out += "\n";
        });

        return _out;
    }

    void Awake() {
        pendingAnims.Clear();
    }

    void Start() {
        storyStack = new Stack<StoryState>();
        instance = this;

        if (initialState != null) {
            pushStory(initialState);
        } else {
            pushStory(initialStory.defaultState);
        }
    }

    public static void resetHistory() {
        if (pendingAnims.Count > 0) {
            throw new AnimationPendingException($"animation pending!\nObjects: {getPendingAnims()}");
        }

        var tmp = storyStack.Pop();
        storyStack.Clear();
        pushStory(tmp);
    }

    public static void resetStory() {
        if (pendingAnims.Count > 0) {
            throw new AnimationPendingException($"animation pending!\nObjects: {getPendingAnims()}");
        }

        var curr = storyStack.Peek();

        while(storyStack.Count > 0 && storyStack.Peek().owner == curr.owner) {
            storyStack.Pop();
        }

        pushStory(curr.owner.defaultState);
    }

    public static void pushNextState(bool ignorePendingAnims=false) {
        if (pendingAnims.Count > 0 && !ignorePendingAnims) {
            throw new AnimationPendingException($"animation pending!\nObjects: {getPendingAnims()}");
        }

        getCurrent().owner.switchToNextState(ignorePendingAnims);
    }

    public static void replaceStory(StoryState? storyState, bool ignorePendingAnims=false) {
        if (pendingAnims.Count > 0 && !ignorePendingAnims) {
            throw new AnimationPendingException($"animation pending!\nObjects: {getPendingAnims()}");
        }

        if (storyState == null) throw new System.Exception("push with null state");

        if (storyStack.Count > 0) {
            storyStack.Pop().owner.onPop(closeForced: true);
        }

        pushStory(storyState, ignorePendingAnims);
    }

    public static void pushStory(StoryState? storyState, bool ignorePendingAnims=false) {
        if (pendingAnims.Count > 0 && !ignorePendingAnims) {
            throw new AnimationPendingException($"animation pending!\nObjects: {getPendingAnims()}");
        }

        if (storyState == null) throw new System.Exception("push with null state");

        StoryState c = (StoryState)storyState;

        if (storyStack.Count > 0) {
            storyStack.Peek().owner.onPop();
        }

        storyStack.Push(c);

        try {
            c.owner.onPush(c);
        } catch (System.Exception e) {
            storyStack.Pop();
            getCurrent().owner.onPush(getCurrent(), forced: true);
            throw e;
        }
    }

    public static void backStory(int cnt) {
        if (pendingAnims.Count > 0) {
            throw new AnimationPendingException($"animation pending!\nObjects: {getPendingAnims()}");
        }

        popStory(cnt);
    }

    public static StoryState? popStory(int cnt, bool ignorePendingAnims=false) {
        if (pendingAnims.Count > 0 && !ignorePendingAnims) {
            throw new AnimationPendingException($"animation pending!\nObjects: {getPendingAnims()}");
        }

        Stack<StoryState> poppedStates = new();
        StoryState tmp;

        if (storyStack.Count < 2) return null;
        if (!storyStack.TryPeek(out tmp)) return null;

        for (var i = 0; i < cnt; i++) {
            try {
                tmp.owner.onPop(true);
            } catch (System.Exception e) {
                revertPop(poppedStates);
                throw e;
            }

            storyStack.Pop();
            poppedStates.Push(tmp);

            if (!storyStack.TryPeek(out tmp)) {
                return tmp;
            }

            try {
                tmp.owner.onPush(tmp, true);
            } catch (System.Exception e) {
                revertPop(poppedStates);
                throw e;
            }
        }

        return cnt == 0 ? tmp : null;
    }

    static void revertPop(Stack<StoryState> poppedStates) { // last resort
        StoryState tmp;
        if (!poppedStates.TryPop(out tmp)) {
            return;
        }

        while (true) {
            tmp.owner.onPush(tmp);
            if (!(poppedStates.TryPop(out tmp))) {
                return;
            }

            tmp.owner.onPop();
        }
    }

    public static StoryState getCurrent() {
        StoryState tmp;
        storyStack.TryPeek(out tmp);
        return tmp;
    }
}
