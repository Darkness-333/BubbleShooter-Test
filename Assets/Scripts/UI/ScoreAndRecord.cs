using TMPro;
using UnityEngine;

public class ScoreAndRecord : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordText;

    private int score;
    private int record;

    private string levelName;

    public void Init() {
        score = 0;
        levelName = LevelChooser.currentLevelFile.name;
        record = RecordWriter.LoadRecord(levelName);
        UpdateUI();
    }


    private void OnScoreUpdated(int addScore) {
        score += addScore;
        if (record < score) {
            record = score;
            RecordWriter.SaveRecord(levelName, record);
        }
        UpdateUI();
    }

    private void UpdateUI() {
        scoreText.SetText("Счет: " + score.ToString());
        recordText.SetText("Рекорд: " + record.ToString());
    }

    private void OnEnable() {
        EventBus.ScoreUpdated += OnScoreUpdated;
    }

    private void OnDisable() {
        EventBus.ScoreUpdated -= OnScoreUpdated;
    }
}