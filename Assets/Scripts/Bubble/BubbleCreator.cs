using UnityEngine;

public class BubbleCreator {
    private Bubble nextBubble;

    private BubblesCounter bubblesCounter;
    private GameField gameField;
    private GameController gameController;
    private BubblePool bubblePool;

    private int currentColorCode;
    private int nextColorCode;
    private Color[] colors;

    private Vector3 bubbleStartPosition = new Vector3(0, -6, 0);

    public BubbleCreator(Bubble nextBubble) {
        this.nextBubble = nextBubble;

        bubblesCounter = ServiceLocator.Get<BubblesCounter>();
        gameField = ServiceLocator.Get<GameField>();
        gameController = ServiceLocator.Get<GameController>();
        bubblePool = ServiceLocator.Get<BubblePool>();
        BubbleColorsSO bubbleColors = ServiceLocator.Get<BubbleColorsSO>();

        colors = bubbleColors.GetColors();

        InitColors();

    }

    public void ReloadInit() {
        InitColors();
    }

    private void InitColors() {
        currentColorCode = GetRandomColorCode();
        nextColorCode = GetRandomColorCode();
    }

    private int GetRandomColorCode() {
        return Random.Range(0, colors.Length);
    }

    public Bubble PrepareNextBubble() {
        gameField.CheckBubblesInFirstRow();

        if (gameController.gameOver)
            return null;

        if (bubblesCounter.HaveFreeBubbles()) {
            bubblesCounter.GetBubble();
        }
        else {
            gameController.GameOver();
            return null;
        }

        Bubble currentBubble = bubblePool.GetBubble();

        Rigidbody2D rb = currentBubble.AddRb();

        rb.isKinematic = true;
        currentBubble.transform.position = bubbleStartPosition;

        currentColorCode = nextColorCode;
        currentBubble.SetColor(currentColorCode, colors[currentColorCode]);

        nextColorCode = GetRandomColorCode();
        nextBubble.SetColor(nextColorCode, colors[nextColorCode]);

        return currentBubble;
    }

}
