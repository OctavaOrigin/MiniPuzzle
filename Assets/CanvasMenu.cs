using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasMenu : MonoBehaviour
{
    [SerializeField] GameObject button;
    public void Retry(){
        button.SetActive(false);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ShowButton(){
        button.SetActive(true);
    }
}
