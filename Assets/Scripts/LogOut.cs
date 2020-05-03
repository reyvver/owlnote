using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using Google;

public class LogOut : MonoBehaviour
{
    public void LogOutUser()
    {
       FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene(0);
    }
}
