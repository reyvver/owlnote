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
        GoogleSignIn.DefaultInstance.SignOut();

        FirebaseAuth.GetAuth(FirebaseApp.GetInstance("owlnote")).SignOut();
        SceneManager.LoadScene(0);
    }
}
