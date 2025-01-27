using TMPro;
using UnityEngine;

public class BubblesCounter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI numberOfBubblesText;
    [SerializeField] private int startNumberOfBubbles;

    private int numberOfBubbles;

    private void Start() {
        Init();
    }

    public void Init() {
        numberOfBubbles = startNumberOfBubbles;
        UpdateUI();
    }

    public bool HaveFreeBubbles() => numberOfBubbles > 0;

    public void GetBubble() {
        numberOfBubbles--;
        UpdateUI();
    }

    private void UpdateUI() {
        numberOfBubblesText.SetText(numberOfBubbles.ToString());
    }
}