using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NavigationButton : MonoBehaviour {
    [SerializeField] private SceneNames newScene;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(newScene.ToString()));
    }
}