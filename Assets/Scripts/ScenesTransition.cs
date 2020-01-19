using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesTransition : MonoBehaviour
{
    public void MovedTo(int NumberOfScene)
    {
        SceneManager.LoadScene(NumberOfScene);
    }
}
