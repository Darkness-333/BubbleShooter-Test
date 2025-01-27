using System.Collections.Generic;
using UnityEngine;

public class BubblePool : MonoBehaviour {
    [SerializeField] private Bubble bubblePrefab;
    [SerializeField] private Transform bubblesParent;

    private Queue<Bubble> pool = new Queue<Bubble>();

    private Bubble CreateNewBubble() {
        Bubble bubble = Instantiate(bubblePrefab);
        bubble.transform.SetParent(bubblesParent);
        return bubble;
    }

    public Bubble GetBubble() {
        if (pool.Count > 0) {
            Bubble bubble = pool.Dequeue();
            if (bubble == null) {
                return CreateNewBubble();
            }
            bubble.gameObject.SetActive(true);
            return bubble;
        }
        else {
            return CreateNewBubble();
        }
    }

    public void ReturnBubble(Bubble bubble) {
        if (pool.Contains(bubble)) {
            return;
        }
        bubble.gameObject.SetActive(false);
        SetDefaultSettings(bubble);
        pool.Enqueue(bubble);
    }

    private void SetDefaultSettings(Bubble bubble) {
        bubble.transform.localScale = Vector3.one;
        bubble.transform.rotation = Quaternion.identity;

        bubble.RemoveRb();
        bubble.GetCollider().isTrigger = false;
        bubble.ResetCollision();
    }
}
