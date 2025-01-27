using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour {
    [SerializeField] private Bubble bubblePrefab;
    [SerializeField] private Transform bubblesParent;

    private BubbleColorsSO bubbleColors;
    private GameController gameController;
    private BubblePool bubblePool;

    private float bubbleSpacing = 1f;
    private Vector3 startOffset = new Vector3(-9, 9, 0);

    private List<Bubble> firstRowBubbles = new();
    private TextAsset levelFile;
    private int rows;

    public void Init() {
        bubbleColors = ServiceLocator.Get<BubbleColorsSO>();
        gameController = ServiceLocator.Get<GameController>();
        bubblePool = ServiceLocator.Get<BubblePool>();

        ClearField();
        GenerateField();
    }

    private void ClearField() {
        firstRowBubbles.Clear();
        Bubble[] bubbles = FindObjectsOfType<Bubble>();
        foreach (Bubble bubble in bubbles) {
            if (bubble.name == "NextBubble") continue;
            bubblePool.ReturnBubble(bubble);
        }
    }

    public void SetLevelFile(TextAsset levelFile) {
        this.levelFile = levelFile;
    }

    private void GenerateField() {
        Color[] colors = bubbleColors.GetColors();

        string[] lines = levelFile.text.Split('\n');
        rows = lines.Length;

        for (int y = 0; y < rows; y++) {
            float rowOffset = (y % 2 == 1) ? 0.5f * bubbleSpacing : 0f;
            string line = lines[y].Trim();

            for (int x = 0; x < line.Length; x++) {
                int colorCode = line[x] - '0';
                Vector3 position = new Vector3(x * bubbleSpacing, -y * bubbleSpacing, 0) + startOffset + Vector3.right * rowOffset;
                Bubble bubble = bubblePool.GetBubble();
                bubble.transform.position = position;
                bubble.SetColor(colorCode, colors[colorCode]);

                if (y == 0) {
                    firstRowBubbles.Add(bubble);
                }

            }
        }
    }

    public List<Bubble> GetFirstRowBubbles() {
        return firstRowBubbles;
    }

    public void CheckBubblesInFirstRow() {
        int activeBubbles = 0;
        foreach (Bubble bubble in firstRowBubbles) {
            if (bubble.gameObject.activeSelf) activeBubbles++;
        }

        if (activeBubbles <= firstRowBubbles.Count * 0.3f) {
            gameController.GameOver(true);
        }
    }

}
