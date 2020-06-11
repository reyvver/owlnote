using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class UserAuthAndRegistryScript : MonoBehaviour
{
  protected FirebaseAuth Auth;
  protected readonly Dictionary<string, FirebaseUser> UserByAuth = new Dictionary<string, FirebaseUser>();
  private string _errorMessage, _errorOperation;
  public GameObject clickLabel, panelConfirm;
  public TMP_InputField emailAuth, emailReg;
  public TMP_InputField passwordAuth, passwordReg, verifyReg;
  public TMP_InputField resetPassword;
  public TMP_Text errorTextReg, errorTextAuth, errorTextReset;
  
  public GameObject objSceneManager;
  private UserAuthAndRegistrSceneManager scriptSceneManager;
  private bool _resetPasswrod;

  protected string emailA = "", emailR = "";
  protected string passwordA = "", passwordR = "", verifyR;
  private bool fetchingToken = false;
  
  DependencyStatus _dependencyStatus = DependencyStatus.UnavailableOther;
  
  void Start()
  {
    scriptSceneManager = objSceneManager.GetComponent<UserAuthAndRegistrSceneManager>();
    FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChanged;
    CheckUser();
    
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
      _dependencyStatus = task.Result;
      if (_dependencyStatus ==DependencyStatus.Available) {
        InitializeFirebase();
      } else {
        Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
      }
    });
  }
  void OnDestroy() {
    FirebaseAuth.DefaultInstance.StateChanged -= HandleAuthStateChanged;
    Auth.StateChanged -= AuthStateChanged;
    Auth.IdTokenChanged -= IdTokenChanged;
    Auth = null;
  }
  void Update() {
    switch (_errorOperation)
    {
      case "Sign-in":
      {
        errorTextAuth.text = _errorMessage;
        break;
      }
      case "User-Creation":
      {
        errorTextReg.text = _errorMessage;
        break;
      }
      case "Password-Reset":
      {
        errorTextReset.text = _errorMessage;
        break;
      }
    }

    if (_resetPasswrod)
    {
      scriptSceneManager.ShowPanel(panelConfirm);
      _resetPasswrod = false;
    }

    emailA = emailAuth.text;
    passwordA = passwordAuth.text;
        

    emailR = emailReg.text;
    passwordR = passwordReg.text;
    verifyR = verifyReg.text;
  }
  
  private void HandleAuthStateChanged(object sender, EventArgs e)
  {
    CheckUser();
  }
  private void CheckUser()
  {
    if (FirebaseAuth.DefaultInstance.CurrentUser != null)
      SceneManager.LoadScene(1);
    else
    {
      clickLabel.SetActive(true);
    }
  }
  void InitializeFirebase() {
    Debug.Log("Setting up Firebase Auth");
    Auth = FirebaseAuth.DefaultInstance;
    Auth.StateChanged += AuthStateChanged;
    Auth.IdTokenChanged += IdTokenChanged;
    AuthStateChanged(this, null);
  }
  void AuthStateChanged(object sender, EventArgs eventArgs) {
    var senderAuth = sender as FirebaseAuth;
    FirebaseUser user = null;
    
    if (senderAuth != null) UserByAuth.TryGetValue(senderAuth.App.Name, out user);

    if (senderAuth == null || (senderAuth != Auth || senderAuth.CurrentUser == user)) return;
    
    var signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
    
    if (!signedIn && user != null) 
    {
      Debug.Log("Signed out " + user.UserId);
      //user is logged out, load login screen 
      //SceneManager.LoadSceneAsync("scene_01");
    }
    
    user = senderAuth.CurrentUser;
    UserByAuth[senderAuth.App.Name] = user;
    
    if (signedIn) 
    {
      Debug.Log("Signed in " + user.UserId);
      SceneManager.LoadSceneAsync(1);
    }
  }
  void IdTokenChanged(object sender, EventArgs eventArgs) {
    if (sender is FirebaseAuth senderAuth && (senderAuth == Auth && senderAuth.CurrentUser != null && !fetchingToken)) {
      senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
        task => Debug.Log($"Token[0:8] = {task.Result.Substring(0, 8)}"));
    }
  }

  
  
  
  //Авторзация
  public void SigninAsync() 
  {
    _errorOperation = "Sign-in";
    
    if (emailA != "" && passwordA != "")
    {
      errorTextAuth.text = "";
      Debug.Log(emailA+"  "+ passwordA);
      Auth.SignInWithEmailAndPasswordAsync(emailA, passwordA).ContinueWith(HandleSigninResult);
    }
    else
    {
      _errorMessage = "Заполните поля";
    }
  }
  private void HandleSigninResult(Task<FirebaseUser> authTask)
  {
    if (LogTaskCompletion(authTask))
    {
      SceneManager.LoadScene(1);
    }
  }
  
  
  
  
  //Регистрация
  public void CreateUserAsync()
  {
    _errorOperation = "User-Creation";
    
    if (CheckInput())
    {
      Auth.CreateUserWithEmailAndPasswordAsync(emailR, passwordR).ContinueWith(HandleCreateUserAsync).Unwrap();
    }
  }
  private Task HandleCreateUserAsync(Task<FirebaseUser> authTask) 
  {
    if (LogTaskCompletion(authTask))
    {
      Auth.CurrentUser?.SendEmailVerificationAsync().ContinueWith(task =>
      {
        if (task.IsCanceled) {
          Debug.LogError("SendEmailVerificationAsync was canceled.");
          return;
        }
        if (task.IsFaulted) {
          Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
          return;
        }
        Debug.Log("Email sent successfully.");
      });
    }
    return Task.FromResult(0);
  }
  private bool CheckInput()
  {
    if (passwordReg.text != "" && verifyReg.text != "" && emailReg.text != "")
    {
      if (passwordReg.text == verifyReg.text) return true;
      _errorMessage = "Введенные пароли не совпадают";
      return false;
    }
    {
      _errorMessage = "Не все поля заполнены";
      return false;
    }
  }



  //Сброс пароля на почту
  public void ResetPassword()
  {
    _errorOperation = "Password-Reset";
    
    string emailAddress = resetPassword.text;

    if (emailAddress != "")
    {
      FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(emailAddress).ContinueWith(HandlePasswordReset);
    }
    else
    {
      _errorMessage = "Заполните поле";
    }
  }
  private Task HandlePasswordReset(Task passTask)
  {
    if (LogTaskCompletion(passTask))
    {
      _resetPasswrod = true;
      ClearInputs();
    }
    return Task.FromResult(0);
  }


  //Другое
  private bool LogTaskCompletion(Task task) {
    var complete = false;
    if (task.IsCanceled) {
      Debug.Log("canceled");
    }
    else if (task.IsFaulted)
    {
      Debug.Log("error");
      if (task.Exception != null)
        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
        {
          string authErrorCode = "";

          if (exception is FirebaseException firebaseEx)
          {
            authErrorCode = $"AuthError.{((AuthError) firebaseEx.ErrorCode).ToString()}: ";
          }

          _errorMessage = exception.Message;
          Debug.Log(authErrorCode + exception);
        }
    }
    else if (task.IsCompleted) {
      Debug.Log( " completed");
      complete = true;
    }
    return complete;
  }
  public void ClearInputs()
  {
    emailAuth.text = "";
    emailReg.text = "";
    passwordAuth.text = "";
    passwordReg.text = "";
    verifyReg.text = "";
    resetPassword.text = "";

    _errorMessage = "";
    _errorOperation = "";
    errorTextAuth.text = "";
    errorTextReg.text = "";
    errorTextReset.text = "";
  }
  
}
