using UnityEngine;
using UnityEngine.UI;

public class QuitMenu : MonoBehaviour {
    [SerializeField] private Button quitButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject quitMenu;

    private void Start() {
        quitMenu.SetActive(false);
        quitButton.onClick.AddListener(ShowQuitMenu);
        confirmButton.onClick.AddListener(Quit);
        cancelButton.onClick.AddListener(HideQuitMenu);

    }

    private void ShowQuitMenu() {
        quitMenu.SetActive(true);
    }

    private void HideQuitMenu() {
        quitMenu.SetActive(false);
    }

    private void Quit() {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }   
}