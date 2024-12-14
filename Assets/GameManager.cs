using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Scene LevelScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadAsyncScene()); // 1
        StartCoroutine(LoadAsyncScene()); // 2
        StartCoroutine(LoadAsyncScene()); // 3
        StartCoroutine(LoadAsyncScene()); // 4
        StartCoroutine(LoadAsyncScene()); // 5
    }

    IEnumerator LoadAsyncScene()
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/Level", LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}