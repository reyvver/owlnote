using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransition : MonoBehaviour
{
        public int PreviousScene;

        public void MoveBack()
        {
                SceneManager.LoadScene(PreviousScene);
        }
        public void MoveTo(int NumberOfScene)
        {
                PreviousScene = SceneManager.GetActiveScene().buildIndex;
                //Debug.Log(PreviousScene);
                SceneManager.LoadScene(NumberOfScene);
        }
}
