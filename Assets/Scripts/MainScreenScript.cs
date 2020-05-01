using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using TMPro;

public class MainScreenScript : MonoBehaviour
{
    public TextMeshProUGUI verifyEmail, errorLabelResetPassword, errorLabelDeleteAccount;
    public TextMeshProUGUI messageSuccess;
    public GameObject panelSuccess, panelVerify;
    public TMP_InputField passwordCurrent, passwordNew, verifyNew, verifyToDelete;
    private bool _chk_email, _reauthenticate, _passwordReset, _userDeleted;

    private FirebaseUser currentUser;

    // Start is called before the first frame update
    void Start()
    {
        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        StartCoroutine(CheckUserEmail());
    }

    void Update()
    {
    }

    public void ReCheckEmailVerified()
    {
        currentUser.ReloadAsync();
        if (currentUser.IsEmailVerified )
        {
            Debug.Log("подтвержден");
            panelVerify.SetActive(false);
        }
        else       Debug.Log("ne подтвержден");
    }

    IEnumerator CheckUserEmail()
    {
        ReloadUser();
        yield return new WaitUntil(() => _chk_email);
        if (currentUser.UserId != "AVATC0nCWxd1l3saRQhbdoTFjVI3")
        {
            //currentUser.ReloadAsync();
            if (currentUser.IsEmailVerified == false)
            {
                panelVerify.SetActive(true);
                verifyEmail.text = currentUser.Email;
            }
        }
    }

    private void ReloadUser()
    {
        FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Отменено");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Произошла ошибка:  " + task.Exception);
                return;
            }

            _chk_email = true;
        });
    }

    public void SendNewEmail()
    {
        currentUser.SendEmailVerificationAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Email sent successfully.");
        });
    }

    public void LogOutUser()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene(0);
    }

    public void ChangingPassword()
    {
        StartCoroutine(Reauth());
    }

    public void DeletingUser()
    {
        StartCoroutine(Delete());
    }

    IEnumerator Reauth()
    {
        _reauthenticate = false;
        _passwordReset = false;
        
        if (CheckResetPassword())
        {
            Reauthentication(passwordCurrent.text);
            yield return new WaitUntil(() => _reauthenticate);
            ResetPassword();
            yield return new WaitUntil(() => _passwordReset);
            if (_passwordReset)
            {
                errorLabelResetPassword.color = Color.white;
                messageSuccess.text = "Ваш пароль обновлен. Пожалуйста, перезайдите";
                GameObject.Find("SceneManager").GetComponent<MainScreenSceneManager>().ShowPanel(panelSuccess);
            }
        }
    }

    IEnumerator Delete()
    {
        _reauthenticate = false;
        _userDeleted = false;
        
        string password = verifyToDelete.text;
        string email = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        if (password != "")
        {           
            Reauthentication(password);
            yield return new WaitUntil(() =>_reauthenticate);
            DeleteUser();
            yield return new WaitUntil(() =>_userDeleted);
            if (_userDeleted)
            {
                errorLabelDeleteAccount.color = Color.white;
                messageSuccess.text = "Ваш аккаунт ("+email+ ") был удален.";
                GameObject.Find("SceneManager").GetComponent<MainScreenSceneManager>().ShowPanel(panelSuccess);
            }
            else
            {
                errorLabelDeleteAccount.color = Color.red;
                errorLabelDeleteAccount.text = "Произошла ошибка";
            }
        }
        else
        {
            errorLabelResetPassword.color = Color.red;
            errorLabelResetPassword.text = "Неправильный пароль";
        }
    }

    private void ResetPassword()
    {
        Debug.Log("пытаемся изменить пароль");
        string newPassword = passwordNew.text;
        Debug.Log(newPassword);
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        user.UpdatePasswordAsync(newPassword).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdatePasswordAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("UpdatePasswordAsync encountered an error: " + task.Exception);
                return;
            }

            _passwordReset = true;
            Debug.Log("Password updated successfully.");
            
        });
    }

    private bool CheckResetPassword() {
        if (passwordCurrent.text != "" && passwordNew.text != "" && verifyNew.text != "") {
            if (passwordNew.text == verifyNew.text)
            {
                errorLabelResetPassword.color = Color.white;
                return true;
            } 
            else {
                errorLabelResetPassword.color = Color.red;
                errorLabelResetPassword.text = "Ошибка: пароли не совпадают.";
            }
        }
        else{
            errorLabelResetPassword.color = Color.red;
            errorLabelResetPassword.text = "Ошибка: заполните все поля.";
        }

        return false;

    }

    private void Reauthentication(string pass)
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        string email = user.Email;
        string password = pass;
        Credential credential = EmailAuthProvider.GetCredential(email, password);
        if (user != null)
        {
            user.ReauthenticateAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("ReauthenticateAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("ReauthenticateAsync encountered an error: " + task.Exception);
                    return;
                }

                _reauthenticate = true;
                Debug.Log("d " + _reauthenticate);
                Debug.Log("User reauthenticated successfully.");
            });
        }
    }

    private void DeleteUser()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null) {
            user.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                _userDeleted = true;
                Debug.Log("User deleted successfully.");
            });
        }
    }

    public void ClearText()
    {
        passwordCurrent.text = "";
        passwordNew.text  = "";
        verifyNew.text = "";
        verifyToDelete.text = "";
    }
}