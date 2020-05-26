using System;
using System.Threading.Tasks;

using UnityEngine;

using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using TMPro;

public class MainScreenScript : MonoBehaviour
{
    public GameObject objSceneManager;
    private MainScreenSceneManager scriptSceneManager;

    public TextMeshProUGUI verifyEmail, errorLabelResetPassword, errorLabelDeleteAccount, newIventDate;
    public TextMeshProUGUI messageSuccess, textConfirmDelete;
    public GameObject panelSuccess, panelVerify,ConfirmDeleting;
    public TMP_InputField passwordCurrent, passwordNew, verifyNew, verifyToDelete;
    private bool _chkEmail, _reauthenticate, _passwordReset, _userDeleted;
    private string _errorMessage, operation,email;
    private FirebaseUser currentUser;



    public string typeDelete, chosenTime, chosenTitle, chosenCategory, categoryColour, chosenKey;

    void Start()
    {
        chosenCategory = "По умолчанию";
        chosenCategory = "#9792F7";
        scriptSceneManager = objSceneManager.GetComponent<MainScreenSceneManager>();
        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        ReloadUser();
    }

    void Update()
    {
        if (_userDeleted)
        {
            messageSuccess.text = "Ваш аккаунт ("+email+ ") был удален.";
            scriptSceneManager.ShowPanel(panelSuccess);
            scriptSceneManager._importantOperation = true;
            _reauthenticate = false;
            _userDeleted = false;
        }

        if (_passwordReset)
        {
            errorLabelResetPassword.color = Color.white;
            messageSuccess.text = "Ваш пароль обновлен. Пожалуйста, перезайдите";
            scriptSceneManager.ShowPanel(panelSuccess);
            scriptSceneManager._importantOperation = true;
            _reauthenticate = false;
            _passwordReset = false;
        }

        if (_chkEmail)
        {
            if (currentUser.IsEmailVerified == false)
            {
                panelVerify.SetActive(true);
                verifyEmail.text = currentUser.Email;
            }
            else
            {
                panelVerify.SetActive(false);
            }

            _chkEmail = false;
        }

        switch (operation)
        {
            case "delete":
            {
                errorLabelDeleteAccount.text = _errorMessage;
                break;
            }
            case "password-reset":
            {
                errorLabelResetPassword.text = _errorMessage;
                break;
            }
            
        }
    }

    public void ReCheckEmailVerified()
    {
        ReloadUser();
    }

    
    //Проверка подтвержденности почты
    private void ReloadUser()
    {
        FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync().ContinueWith(HandleReloadUser);
    }
    private void HandleReloadUser(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {        
            _chkEmail = true;
        }
    }


    //Выход из аккаунта
    public void LogOutUser()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene(0);
    }
    
    
    //подтверждение важной операции через реавторизацию
    private void Reauthenticate(string pass)
    {
        Debug.Log("мы тут");
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        string email = user.Email;
        string password = pass;
        Credential credential = EmailAuthProvider.GetCredential(email, password);
        user?.ReauthenticateAsync(credential).ContinueWith(HandleReauthenticate);
    }
    private void HandleReauthenticate(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _reauthenticate = true;
            //Debug.Log("User reauthenticated successfully.");
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

    
    
    
    //Смена пароля
    public void ChangingPassword()
    {
        string password = passwordCurrent.text;
        Reauthenticate(password);
        operation = "password-reset";
    }
    private void ResetPassword()
    {
        if (CheckResetPassword())
        {
            string newPassword = passwordNew.text;
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
            user.UpdatePasswordAsync(newPassword).ContinueWith(HandleResetPassword);
        }
    }
    private void HandleResetPassword(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _passwordReset = true;
        }
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
    
    
    
    
    //удаление аккаунта
    public void DeletingUser()
    {
        operation = "delete";
        string password = verifyToDelete.text;
        Reauthenticate(password);
    }
    private void DeleteUser()
    {
        //Debug.Log("удаляем");
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        email = user.Email;
        user?.DeleteAsync().ContinueWith(HandleDeletingUser);
    }
    private void HandleDeletingUser(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _userDeleted = true;
        }
        else
        {
            errorLabelDeleteAccount.color = Color.red;
            errorLabelDeleteAccount.text = _errorMessage;
        }
   
    }

    
    
    
    
    //отправка повторного сообщения для подтверждения почты
    public void SendNewEmail()
    {
        currentUser.SendEmailVerificationAsync().ContinueWith(HandeSendNewEmail);
    }
    private void HandeSendNewEmail(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
           // .Log("повторное сообщение отправлено");
        }
    }
    
    
    
    
    
    //Другое
    public void ClearText()
    {
        passwordCurrent.text = "";
        passwordNew.text  = "";
        verifyNew.text = "";
        verifyToDelete.text = "";
        errorLabelDeleteAccount.text = "";
        errorLabelResetPassword.text = "";
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
                   // Debug.Log(authErrorCode + exception);
                }
        }
        else if (task.IsCompleted) {
           // Debug.Log( " completed");
            complete = true;
        }
        return complete;
    }


    public void ShowConfirmDelete()
    {
        ConfirmDeleting.gameObject.SetActive(true);
        ConfirmDeleting.transform.SetAsLastSibling();
        switch (typeDelete)
        {
            case "event":
            {   
            //   Debug.Log(newIventDate.text + "  "+ chosenTime);
                textConfirmDelete.text = "пункт расписания " + "'"+ chosenTitle+ "'";
                break;
            }
            case "category":
            {
                textConfirmDelete.text = "категорию " + "'" + chosenCategory+ "'";
                break;
            }
        }
        
    }
    

    public void DeleteInf()
    {
        switch (typeDelete)
        {
            case "event":
            {
                string date = ReplaceWith(newIventDate.text);
                DatabaseReference  calendarRef =  FirebaseDatabase.DefaultInstance.GetReference( "/calendar/"+ currentUser.UserId+"/"+date);
                calendarRef.Child(chosenTime).RemoveValueAsync();
            
                break;
            }
            case "note":
            {
                string date = ReplaceWith(newIventDate.text);
                DatabaseReference  calendarRef =  FirebaseDatabase.DefaultInstance.GetReference( "/notes/"+ currentUser.UserId+"/"+date);
                calendarRef.Child(chosenKey).RemoveValueAsync();
                break;
            }
            case "category":
            {
               DatabaseReference categoriesRef = FirebaseDatabase.DefaultInstance.GetReference("/categories/" + currentUser.UserId);
                categoriesRef.Child(chosenCategory).RemoveValueAsync();
                break;
            }
            
        }
        
        ConfirmDeleting.gameObject.SetActive(false);
        ConfirmDeleting.transform.SetAsFirstSibling();
        
    }
    
    private string ReplaceWith(string str)
    {
        string result = "";

        if (str.Contains(@"/"))
        {
            result = str.Replace("/", @"\");
        }
        else
        {
            result = str.Replace(".", @"\");
        }
        return result;

    
    }

}