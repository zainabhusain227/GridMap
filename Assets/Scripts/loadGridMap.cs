using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadGridMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneAfterDelay(2)); // Start the coroutine
    }

    IEnumerator LoadSceneAfterDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds); // Wait for the specified delay
        SceneManager.LoadScene(0); // Load the scene
    }
}
