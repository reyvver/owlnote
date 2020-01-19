
using Firebase.Auth;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LogOut : MonoBehaviour
{
    public void LogOutUser()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene(2);
    }
}
