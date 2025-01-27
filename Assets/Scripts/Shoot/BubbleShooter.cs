using UnityEngine;

public class BubbleShooter : MonoBehaviour {
    [SerializeField] private Bubble nextBubble;

    private GameController gameController;

    private BubbleCreator bubbleCreator;
    private TrajectoryDrawer trajectoryDrawer;

    public void Init() {
        gameController = ServiceLocator.Get<GameController>();

        if (bubbleCreator == null) {
            bubbleCreator = new BubbleCreator(nextBubble);
        }
        else {
            bubbleCreator.ReloadInit();
        }

        if (trajectoryDrawer == null) {
            trajectoryDrawer = new TrajectoryDrawer(bubbleCreator);
        }
        else {
            trajectoryDrawer.ReloadInit();
        }

    }

    public bool CanShoot() {
        return trajectoryDrawer.canShoot;
    }

    void Update() {
        if (!trajectoryDrawer.canShoot || gameController.gameOver || !gameController.endWaitFall) return;

        HandleDragAndShoot();
    }

    private void HandleDragAndShoot() {
        trajectoryDrawer.HandleDragStart();
        trajectoryDrawer.HandleDragging();
        trajectoryDrawer.HandleShoot();
    }
}