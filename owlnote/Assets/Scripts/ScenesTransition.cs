using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesTransition : MonoBehaviour
{
    public int PreviousScene;
    public void MovedBack()
    {
        SceneManager.LoadScene(PreviousScene);
    }

    public void MovedTo(int NumberOfScene)
    {
        PreviousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(NumberOfScene);
    }
}

