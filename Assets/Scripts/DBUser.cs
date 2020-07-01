using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class DBUser : MonoBehaviour
{
    [Header("Textboxes for errors")] 
    public TextMeshProUGUI errorDelete;
    public TextMeshProUGUI errorReset;
    public Transform PanelUserOperation;
    public TMP_InputField newPassword;
    public TMP_InputField verifyPassword;
    public GameObject TutionPanel;
    public GameObject checkmark;
    public List<TMP_InputField> inputs;
    private  FirebaseUser currentUser;
    private Toggle showAgain;
    private static string  operation, _errorMessage, currentPassword;
    private bool _emailVerified, _delete, _reset, _showTution;
    private TextMeshProUGUI textSuccess;
    private static bool _reauthenticate;
    public List<RectTransform> tutionPanels;
    private static DatabaseReference _reference;
    private Transform panelsContainer;
    private int index;
    private GameObject ButtonLeft, ButtonRight;
    private RectTransform currentRect;

    private bool _setting;
    public static bool _deleteProcess;
    
    // Start is called before the first frame update
    void Start()
    {
        _setting = false;
        _deleteProcess = false;
        
        showAgain = checkmark.GetComponent<Toggle>();
        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        textSuccess = PanelUserOperation.GetChild(1).GetComponent<TextMeshProUGUI>();
        
        ReloadUser();
        InitializeDatabase();
        
        ButtonLeft =  TutionPanel.transform.Find("Panel/PanelButtons/ButtonLeft").gameObject;
        ButtonRight =  TutionPanel.transform.Find("Panel/PanelButtons/ButtonRight").gameObject;
        
        index = 0;
        panelsContainer = TutionPanel.transform.Find("Panel/TutionsPanels");
        tutionPanels = new List<RectTransform>();
        foreach (Transform panel in panelsContainer)
        {
            tutionPanels.Add(panel.GetComponent<RectTransform>());
        }
    }
    
    private void InitializeDatabase()
    {
        _reference = FirebaseDatabase.DefaultInstance.GetReference("/settings/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        _reference.ValueChanged += HandleValueChanged;
        Debug.Log("done user");
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        
        DataSnapshot snapshot = args.Snapshot;
        
        if (snapshot.ChildrenCount > 0)
        {
            string userShowType = snapshot.Child("showType").Value.ToString();
            _showTution = Convert.ToBoolean(snapshot.Child("tution").Value);
            ViewModel.ChangeContainersOrder(userShowType);
        }
        else
            if (snapshot.ChildrenCount == 0)
            {
                AddDefaultSetting();
            }

        ShowTution();
    }
    private void OnDestroy()
    {
        try
        {
            _reference.ValueChanged -= HandleValueChanged;
        }
        catch
        {
            Debug.Log("Empty settings");
        }

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

            if (operation == "delete")
            { 
                Debug.Log("gee");
                _reauthenticate = false;
                DeleteSettings();
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

    private void AddDefaultSetting()
    {
        if (!_deleteProcess)
        {
            _reference.Child("showType").SetValueAsync("event");
            _reference.Child("tution").SetValueAsync(false);
        }
    }

    public static void ChangeShowType(string newValue)
    {
        _reference.Child("showType").SetValueAsync(newValue);
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
    
    
    

    
    
    


    public static void LogOutUser()
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
                DeleteSettings();
            }
            if (operation == "password-reset")
            {
                ResetPassword();
            }
        }
    }
    
    
    
    public void DeletingUser(TMP_InputField verifyToDelete)
    {
        _deleteProcess = true;
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
    private void ShowTution()
    {
        if (!_showTution)
        {
            TutionPanel.SetActive(true);
        }
        else
        {
            TutionPanel.transform.SetAsFirstSibling();
        }
    }
    public void CloseTution()
    {
        TutionPanel.SetActive(false);
        TutionPanel.transform.SetAsFirstSibling();

        if (showAgain.isOn && !_showTution)
        {
            _reference.Child("tution").SetValueAsync(true);
        }
    }
    public void OnLeft()
    {
        if (index ==1)
        {
            ButtonLeft.SetActive(false);
        }
        else
        {

            ButtonRight.SetActive(true);
        }

        currentRect = tutionPanels[index];
        currentRect.DOAnchorPos(new Vector2(878,1), 0.4f);
        index--;
    }
    public void OnRight()
    {
        index++;
        if (index == 4)
        {
            ButtonRight.SetActive(false);
        }     
        else
        {
            ButtonLeft.SetActive(true);
          
        }
        currentRect = tutionPanels[index];
        currentRect.DOAnchorPos(Vector2.zero, 0.4f);
    }

    
    
    public void DeleteSettings()
    {
        StartCoroutine(DeleteInfo());
    }

    private void DeleteNode(string Node)
    {
        _setting = false;
        DatabaseReference currentReference = FirebaseDatabase.DefaultInstance.GetReference(
            "/"+Node+"/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        currentReference.RemoveValueAsync().ContinueWith(HandleDeletingNode);
    }
    
    private void HandleDeletingNode(Task userTask)
    {
        if (LogTaskCompletion(userTask))
        {
            _setting = true;
        }
    }


    IEnumerator DeleteInfo()
    {
        if (ViewModel.DBRefExist("calendar"))
        {
            DeleteNode("calendar");
            yield return new WaitUntil(() => _setting);
        }
        if (ViewModel.DBRefExist("categories"))
        {
            DeleteNode("categories");
            yield return new WaitUntil(() =>_setting);
        }
        if (ViewModel.DBRefExist("notes"))
        {
            DeleteNode("notes");
            yield return new WaitUntil(() =>_setting);
        }
        if (ViewModel.DBRefExist("plans"))
        {
            DeleteNode("plans");
            yield return new WaitUntil(() =>_setting);
        }
        if (ViewModel.DBRefExist("todo"))
        {
            DeleteNode("categories");
            yield return new WaitUntil(() =>_setting);
        }
        if (ViewModel.DBRefExist("settings"))
        {
            DeleteNode("settings");
            yield return new WaitUntil(() =>_setting);
        }
         yield return new WaitForSeconds(0.01f);
        DeleteUser();
        textSuccess.text = "Аккаунт успешно удален";
        _reauthenticate = false;
        PanelUserOperation.gameObject.SetActive(true);
        PanelUserOperation.SetAsLastSibling();
    }
    
}
