using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour {
    [SerializeField] Story initialStory;

    static Stack<StoryState> storyStack;
    public static StoryManager instance;

    public static int pendingAnims = 0;

    void Awake() {
        pendingAnims = 0;

        if (instance != null) {
            throw new System.Exception("StoryManager is initialized for second time.");
        }

        storyStack = new Stack<StoryState>();
        instance = this;

        pushStory(initialStory.defaultState);
    }

    public static void resetHistory() {
        if (pendingAnims > 0) {
            throw new System.Exception("animation pending.");
        }

        var tmp = storyStack.Pop();
        storyStack.Clear();
        pushStory(tmp);
    }

    public static void resetStory() {
        if (pendingAnims > 0) {
            throw new System.Exception("animation pending.");
        }

        var curr = storyStack.Peek();

        while(storyStack.Count > 0 && storyStack.Peek().owner == curr.owner) {
            storyStack.Pop();
        }

        pushStory(curr.owner.defaultState);
    }

    public static void pushNextState() {
        if (pendingAnims > 0) {
            throw new System.Exception("animation pending.");
        }

        getCurrent().owner.switchToNextState();
    }

    public static void replaceStory(StoryState? storyState) {
        if (pendingAnims > 0) {
            throw new System.Exception("animation pending.");
        }

        if (storyState == null) throw new System.Exception("push with null state");

        if (storyStack.Count > 0) {
            storyStack.Pop().owner.onPop(closeForced: true);
        }

        pushStory(storyState);
    }

    public static void pushStory(StoryState? storyState) {
        if (pendingAnims > 0) {
            throw new System.Exception("animation pending.");
        }

        if (storyState == null) throw new System.Exception("push with null state");

        StoryState c = (StoryState)storyState;

        if (storyStack.Count > 0) {
            storyStack.Peek().owner.onPop();
        }

        storyStack.Push(c);
        c.owner.onPush(c);
    }

    public static void backStory(int cnt) {
        if (pendingAnims > 0) {
            throw new System.Exception("animation pending.");
        }

        popStory(cnt);
    }

    static StoryState? popStory(int cnt) {
        if (pendingAnims > 0) {
            throw new System.Exception("animation pending.");
        }

        StoryState tmp;

        if(storyStack.Count <= 1) return null;
        if(!storyStack.TryPop(out tmp)) return null;

        for (var i = 0; i < cnt; i++) {
            tmp.owner.onPop(true);
            if(!storyStack.TryPeek(out tmp)) {
                return tmp;
            }
            tmp.owner.onPush(tmp, true);
        }

        return cnt == 0 ? tmp : null;
    }

    public static StoryState getCurrent() {
        StoryState tmp;
        storyStack.TryPeek(out tmp);
        return tmp;
    }
}