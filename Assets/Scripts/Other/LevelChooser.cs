using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelChooser : MonoBehaviour {
    [SerializeField] private TextAsset[] levelFiles;
    [SerializeField] private Transform scrollContentTransfrom;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Button chooseLevelButton;
    [SerializeField] private GameObject levelCanvas;

    private GameField gameField;
    private BubbleShooter bubbleShooter;
    private BubblesCounter bubblesCounter;
    private ScoreAndRecord scoreAndRecord;
    private GameController gameController;

    public static TextAsset currentLevelFile { get; private set; }

    public void Init() {
        gameField = ServiceLocator.Get<GameField>();
        bubbleShooter = ServiceLocator.Get<BubbleShooter>();
        bubblesCounter = ServiceLocator.Get<BubblesCounter>();
        scoreAndRecord = ServiceLocator.Get<ScoreAndRecord>();
        gameController = ServiceLocator.Get<GameController>();

        if (currentLevelFile == null) {
            currentLevelFile = levelFiles[0];
        }
        gameField.SetLevelFile(currentLevelFile);

        chooseLevelButton.onClick.AddListener(ShowOrHideLevels);

        AddLevels();
    }

    private void AddLevels() {
        foreach (TextAsset level in levelFiles) {
            Button levelButton = Instantiate(levelButtonPrefab, scrollContentTransfrom);

            string levelName = level.name;
            levelButton.GetComponentInChildren<TextMeshProUGUI>().SetText(levelName);
            levelButton.onClick.AddListener(() => ChangeLevelFile(levelName));
        }
    }

    public void ShowOrHideLevels() {
        bool isCanvasActive = levelCanvas.activeSelf;
        levelCanvas.SetActive(!isCanvasActive);
        bubbleShooter.gameObject.SetActive(isCanvasActive);
    }

    public void ChangeLevelFile(string name) {
        foreach (TextAsset levelFile in levelFiles) {
            if (levelFile.name == name) {
                currentLevelFile = levelFile;
                break;
            }
        }
        ReloadLevel();
    }

    private void ReloadInit() {
        gameField.SetLevelFile(currentLevelFile);

    }

    public void ReloadLevel() {
        if (!gameController.endWaitFall || !bubbleShooter.CanShoot()) return;
        ReloadInit();
        scoreAndRecord.Init();
        gameController.ReloadInit();
        gameField.Init();
        bubblesCounter.Init();
        bubbleShooter.Init();
    }

}