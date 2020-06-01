using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using TMPro;

public class DBUser : MonoBehaviour
{
    [Header("Textboxes for errors")] 
    public TextMeshProUGUI errorDelete;
    public TextMeshProUGUI errorReset;
    public Transform PanelUserOperation;
    public TMP_InputField newPassword;
    public TMP_InputField verifyPassword;

    public List<TMP_InputField> inputs;
    private  FirebaseUser currentUser;
    
    private static string  operation, _errorMessage, currentPassword;
    private bool _emailVerified, _delete, _reset;
    private TextMeshProUGUI textSuccess;
    private static bool _reauthenticate;
    
    // Start is called before the first frame update
    void Start()
    {
        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        textSuccess = PanelUserOperation.GetChild(1).GetComponent<TextMeshProUGUI>();
        ReloadUser();
    }

    // Update is called once per frame
    void Update()
    {
        if (_emailVerified)
        {
            if (currentUser.IsEmailVerified == false)
            {
                ViewModel.EmailVerifyPanel.SetActive(true);
                ViewModel.emailVerify.text = currentUser.Email;
            }
            else
            {
                ViewModel.EmailVerifyPanel.SetActive(false);
            }
            _emailVerified = false;

        }

        switch (operation)
        {
            case "delete":
            {
                errorDelete.text = _errorMessage;
                break;
            }
            case "password-reset":
            {
                errorReset.text = _errorMessage;
                break;
            }
        }

        if (!_reauthenticate) return;



        if (_delete)
        {
            textSuccess.text = "Аккаунт успешно удален";
            _delete = false;
            _reauthenticate = false;
            PanelUserOperation.gameObject.SetActive(true);
            PanelUserOperation.SetAsLastSibling();
        }
        if (_reset)
        {
            
            textSuccess.text = "Пароль успешно изменен. Перезайдите в приложение";
            _reset = false;
            _reauthenticate = false;
            FirebaseAuth.DefaultInstance.SignOut();
            PanelUserOperation.gameObject.SetActive(true);
            PanelUserOperation.SetAsLastSibling();
        }
        
        Clear();

    }




    //Проверка подтвержденности почты
    public void CheckEmailVerifiedAgain()
    {
        ReloadUser();
    }
    private void ReloadUser()
    {
        FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync().ContinueWith(HandleReloadUser);
    }
    private void HandleReloadUser(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _emailVerified = true;
        }
    }
    
    
    
    
    //отправка повторного сообщения для подтверждения почты
    public void SendNewEmail()
    {
        currentUser.SendEmailVerificationAsync().ContinueWith(HandleSendNewEmail);
    }
    private void HandleSendNewEmail(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            Debug.Log("повторное сообщение отправлено");
        }
    }
    
    
    

    
    
    


    public void LogOutUser()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene(0);
    }
    
    private bool LogTaskCompletion(Task task) {
        var complete = false;
        if (task.IsCanceled) {
            //Debug.Log("canceled");
        }
        else if (task.IsFaulted)
        {
            // Debug.Log("error");
            if (task.Exception != null)
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    string authErrorCode = "";

                    if (exception is FirebaseException firebaseEx)
                    {
                        authErrorCode = $"AuthError.{((AuthError) firebaseEx.ErrorCode).ToString()}: ";
                    }

                    _errorMessage = exception.Message;
                }
        }
        else if (task.IsCompleted) {
            complete = true;
        }
        return complete;
    }


    //подтверждение важной операции через реавторизацию
    private void Reauthenticate()
    {
        Debug.Log(currentPassword);
        string emailUser = currentUser.Email;
        Credential credential = EmailAuthProvider.GetCredential(emailUser, currentPassword);
        currentUser?.ReauthenticateAsync(credential).ContinueWith(HandleReauthenticate);
    }
    private void HandleReauthenticate(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _reauthenticate = true;
            if (operation == "delete")
            {
                DeleteUser();
            }
            if (operation == "password-reset")
            {
                ResetPassword();
            }
        }
    }
    
    
    
    public void DeletingUser(TMP_InputField verifyToDelete)
    {
        _reauthenticate = false;
        _errorMessage = "";
        operation = "delete";
        
        inputs.Add(verifyToDelete);
        
        currentPassword = verifyToDelete.text;
        Reauthenticate();
    }
    private void DeleteUser()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        user?.DeleteAsync().ContinueWith(HandleDeletingUser);
    }
    private void HandleDeletingUser(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _delete = true;
        }
    }

    private void Clear()
    {
        foreach (TMP_InputField txt in inputs)
        {
            txt.text = "";
        }
        inputs.Clear();
    }
    
    public void ChangingPassword(TMP_InputField passwordCurrent)
    {
        _reauthenticate = false;
        _reset = false;
        _errorMessage = "";
        operation = "password-reset";
        
        inputs.Add(passwordCurrent);
        inputs.Add(newPassword);
        inputs.Add(verifyPassword);
        
        currentPassword =  passwordCurrent.text;
        Reauthenticate();
    }
    
    private void ResetPassword()
    {
        if (CheckResetPassword())
        { 
            currentUser.UpdatePasswordAsync(currentPassword).ContinueWith(HandleResetPassword);
        }
    }
    private void HandleResetPassword(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _reset = true;
        }
    }

    private bool CheckResetPassword()
    {
        bool result = false;
        if (newPassword.text != "" && verifyPassword.text != "")
        {
            if (newPassword.text == verifyPassword.text)
            {
                currentPassword = newPassword.text;
                _errorMessage = "";
                result = true;
            }
            else _errorMessage = "Ошибка: пароли не совпадают.";
        }
        else  _errorMessage = "Ошибка: заполните все требуемые поля";

        return result;
    }

}
