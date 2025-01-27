using UnityEngine;

public class EntryPoint : MonoBehaviour {
    [SerializeField] private GameController gameController;
    [SerializeField] private GameField gameField;
    [SerializeField] private BubbleShooter bubbleShooter;
    [SerializeField] private LevelChooser levelChooser;
    [SerializeField] private ScoreAndRecord scoreAndRecord;

    [SerializeField] private BubbleColorsSO bubbleColors;
    [SerializeField] private BubblesCounter bubblesCounter;
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private BubblePool bubblePool;


    private void Awake() {
        ServiceLocator.Clear();

        ServiceLocator.Register(gameController);
        ServiceLocator.Register(gameField);
        ServiceLocator.Register(bubbleShooter);
        ServiceLocator.Register(levelChooser);
        ServiceLocator.Register(scoreAndRecord);

        ServiceLocator.Register(bubbleColors);
        ServiceLocator.Register(bubblesCounter);
        ServiceLocator.Register(lineRendererPrefab);
        ServiceLocator.Register(bubblePool);
    }

    private void Start() {
        levelChooser.Init();
        scoreAndRecord.Init();
        gameField.Init();
        gameController.Init();
        bubbleShooter.Init();
    }
}