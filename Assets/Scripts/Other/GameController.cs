using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI gameOverText;

    private GameField gameField;
    private BubblePool bubblePool;

    public bool gameOver { get; private set; }
    public bool endWaitFall { get; private set; }

    private AnimationManager animationManager;

    public void Init() {
        gameField = ServiceLocator.Get<GameField>();
        bubblePool = ServiceLocator.Get<BubblePool>();

        endWaitFall = true;
        animationManager = new AnimationManager();
    }

    public void ReloadInit() {
        StopAllCoroutines();
        endWaitFall = true;
        gameOver = false;
        gameOverText.gameObject.SetActive(false);
    }

    public void GameOver(bool isWin = false) {
        gameOver = true;
        gameOverText.gameObject.SetActive(true);
        gameOverText.SetText(isWin ? "Ты победил!" : "Ты проиграл!");
    }

    public void HandleBubblePlacement(Bubble bubble, Collision2D collision) {
        if (!endWaitFall) return;
        endWaitFall = false;
        bubble.RemoveRb();

        Vector3 closestPosition;
        if (bubble.isMaxStretched) {
            bubble.transform.position = collision.transform.position;
            closestPosition = collision.transform.position;
            bubblePool.ReturnBubble(collision.gameObject.GetComponent<Bubble>());

        }
        else {
            closestPosition = FindClosestGridPosition(bubble.transform.position);
            bubble.transform.position = closestPosition;
        }

        CheckNeighborsAsync(bubble);

        Collider2D[] hits = Physics2D.OverlapCircleAll(closestPosition, 1.1f);
        foreach (var hit in hits) {
            Bubble neighbor = hit.GetComponent<Bubble>();
            if (neighbor != null && bubble.gameObject.activeSelf) {
                animationManager.MoveAnimation(hit.transform, closestPosition);
            }
        }
    }

    private Vector3 FindClosestGridPosition(Vector3 position) {
        float gridSize = 1f;
        float halfGridSize = gridSize * 0.5f;

        int rowIndex = Mathf.RoundToInt(position.y);
        bool isEvenRow = Mathf.Abs(rowIndex) % 2 == 1;
        float rowOffset = isEvenRow ? 0f : halfGridSize;

        int colIndex = Mathf.RoundToInt((position.x - rowOffset) / gridSize);

        float x = colIndex * gridSize + rowOffset;
        float y = rowIndex;

        return new Vector3(x, y, position.z);
    }

    public async void CheckNeighborsAsync(Bubble bubble) {
        List<Bubble> neighbors = new List<Bubble>();
        FindNeighbors(bubble, bubble.colorCode, neighbors);

        if (neighbors.Count >= 3) {
            List<Transform> bubbleTransforms = neighbors.Select(n => n.transform).ToList();

            await animationManager.PopAnimationSequentialAsync(bubbleTransforms);

            EventBus.ScoreUpdated?.Invoke(neighbors.Count);
            StartCoroutine(CheckFloatingBalls());
        }
        else {
            endWaitFall = true;
        }
    }


    private IEnumerator CheckFloatingBalls() {

        yield return new WaitForEndOfFrame();
        Bubble[] allBubbles = FindObjectsOfType<Bubble>();
        List<Bubble> firstRowBubbles = gameField.GetFirstRowBubbles();
        List<Bubble> connectedBubbles = new List<Bubble>();
        foreach (Bubble bubble in firstRowBubbles) {
            if (bubble.gameObject.activeSelf) {
                FindConnectedBubbles(bubble, connectedBubbles);
            }
        }
        List<Bubble> fallingBubbles = new List<Bubble>();
        foreach (Bubble bubble in allBubbles) {
            if (bubble.name == "NextBubble") {
                continue;
            }
            if (!connectedBubbles.Contains(bubble)) {
                bubble.AddRb();
                bubble.GetCollider().isTrigger = true;
                fallingBubbles.Add(bubble);
                bubble.DOKill();
            }
        }

        yield return WaitUntilBubblesFall(fallingBubbles);
    }

    private IEnumerator WaitUntilBubblesFall(List<Bubble> fallingBubbles) {
        yield return new WaitWhile(() => fallingBubbles.Any(b => b.gameObject.activeSelf));
        endWaitFall = true;
    }


    void FindNeighbors(Bubble bubble, int color, List<Bubble> neighbors) {
        if (neighbors.Contains(bubble)) return;

        neighbors.Add(bubble);
        Collider2D[] hits = Physics2D.OverlapCircleAll(bubble.transform.position, 1.1f);
        foreach (var hit in hits) {
            Bubble neighbor = hit.GetComponent<Bubble>();
            if (neighbor != null && bubble.gameObject.activeSelf && neighbor.colorCode == color && !neighbors.Contains(neighbor)) {
                FindNeighbors(neighbor, color, neighbors);
            }
        }

    }

    private void FindConnectedBubbles(Bubble bubble, List<Bubble> connectedBubbles) {
        if (connectedBubbles.Contains(bubble)) return;

        connectedBubbles.Add(bubble);
        Collider2D[] hits = Physics2D.OverlapCircleAll(bubble.transform.position, 1.1f);
        foreach (var hit in hits) {
            Bubble neighbor = hit.GetComponent<Bubble>();
            if (neighbor != null && bubble.gameObject.activeSelf && !connectedBubbles.Contains(neighbor)) {
                FindConnectedBubbles(neighbor, connectedBubbles);
            }
        }

    }

}
