using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class UserAuthAndRegistrScript : MonoBehaviour {

  protected FirebaseAuth auth;
  //private Firebase.Auth.FirebaseAuth otherAuth;
  protected Dictionary<string, FirebaseUser> userByAuth = new Dictionary<string, FirebaseUser>();
  private string logText = "";
  public GameObject ClickLabel, PanelConfrim;
  public TMP_InputField emailAuth, emailReg;
  public TMP_InputField passwordAuth, passwordReg, verifyReg;
  public TMP_InputField resetPassword;
  public TMP_Text errorText;
  
  protected string emailA = "", emailR = "";
  protected string passwordA = "", passwordR = "", verifyR;
  protected string displayName = "";
  private bool fetchingToken = false;

  const int kMaxLogSize = 16382;
  Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
  
  public void Start() {
    
    FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChanged;
    CheckUser();
    
    Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
      dependencyStatus = task.Result;
      if (dependencyStatus == Firebase.DependencyStatus.Available) {
        InitializeFirebase();
      } else {
        Debug.LogError(
          "Could not resolve all Firebase dependencies: " + dependencyStatus);
      }
    });
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
      ClickLabel.SetActive(true);
    }
  }
  
  // Handle initialization of the necessary firebase modules:
  void InitializeFirebase() {
    DebugLog("Setting up Firebase Auth");
    auth = FirebaseAuth.DefaultInstance;
    auth.StateChanged += AuthStateChanged;
    auth.IdTokenChanged += IdTokenChanged;
    AuthStateChanged(this, null);
  }

  // Exit if escape (or back, on mobile) is pressed.
  public void Update() {
       //для авторизации
        emailA = emailAuth.text;
        passwordA = passwordAuth.text;
        
        //для регистрации
        emailR = emailReg.text;
        passwordR = passwordReg.text;
        verifyR = verifyReg.text;
  }

  void OnDestroy() {
    FirebaseAuth.DefaultInstance.StateChanged -= HandleAuthStateChanged;
    auth.StateChanged -= AuthStateChanged;
    auth.IdTokenChanged -= IdTokenChanged;
    auth = null;
  }

  // Output text to the debug log text field, as well as the console.
  public void DebugLog(string s) {
    Debug.Log(s);
    logText += s + "\n";

    while (logText.Length > kMaxLogSize) {
      int index = logText.IndexOf("\n");
      logText = logText.Substring(index + 1);
    }
  }

  // Display user information.
  void DisplayUserInfo(IUserInfo userInfo, int indentLevel) {
    string indent = new String(' ', indentLevel * 2);
    var userProperties = new Dictionary<string, string> {
      {"Display Name", userInfo.DisplayName},
      {"Email", userInfo.Email},
      {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
      {"Provider ID", userInfo.ProviderId},
      {"User ID", userInfo.UserId}
    };
    foreach (var property in userProperties) {
      if (!String.IsNullOrEmpty(property.Value)) {
        DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
      }
    }
  }

  // Display a more detailed view of a FirebaseUser.
  void DisplayDetailedUserInfo(FirebaseUser user, int indentLevel) {
    DisplayUserInfo(user, indentLevel);
    DebugLog("  Anonymous: " + user.IsAnonymous);
    DebugLog("  Email Verified: " + user.IsEmailVerified);
    var providerDataList = new List<IUserInfo>(user.ProviderData);
    if (providerDataList.Count > 0) {
      DebugLog("  Provider Data:");
      foreach (var providerData in user.ProviderData) {
        DisplayUserInfo(providerData, indentLevel + 1);
      }
    }
  }

  // Track state changes of the auth object.
  void AuthStateChanged(object sender, EventArgs eventArgs) {
    FirebaseAuth senderAuth = sender as FirebaseAuth;
    FirebaseUser user = null;
    if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
    if (senderAuth == auth && senderAuth.CurrentUser != user) {
      bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
      if (!signedIn && user != null) {
        DebugLog("Signed out " + user.UserId);
                //user is logged out, load login screen 
                //SceneManager.LoadSceneAsync("scene_01");
      }
      user = senderAuth.CurrentUser;
      userByAuth[senderAuth.App.Name] = user;
      if (signedIn) {
        DebugLog("Signed in " + user.UserId);
        displayName = user.DisplayName ?? "";
        DisplayDetailedUserInfo(user, 1);
        SceneManager.LoadSceneAsync(1);
      }
    }
  }

  // Track ID token changes.
  void IdTokenChanged(object sender, EventArgs eventArgs) {
    FirebaseAuth senderAuth = sender as FirebaseAuth;
    if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken) {
      senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
        task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
    }
  }

  // Log the result of the specified task, returning true if the task
  // completed successfully, false otherwise.
  public bool LogTaskCompletion(Task task, string operation) {
    bool complete = false;
    if (task.IsCanceled) {
      DebugLog(operation + " canceled.");
    }
    else if (task.IsFaulted)
    {
        DebugLog(operation + " encounted an error.");
        foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {

                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;

            if (firebaseEx != null) {

                authErrorCode = String.Format("AuthError.{0}: ",
                ((AuthError)firebaseEx.ErrorCode).ToString());
            }
        DebugLog(authErrorCode + exception.ToString());
        }
    }
    else if (task.IsCompleted) {
      DebugLog(operation + " completed");
      complete = true;
    }
    return complete;
  }

  public void CreateUserAsync() {
    DebugLog(String.Format("Attempting to create user {0}...", emailR));

    // This passes the current displayName through to HandleCreateUserAsync
    // so that it can be passed to UpdateUserProfile().  displayName will be
    // reset by AuthStateChanged() when the new user is created and signed in.
    if (CheckInput())
    {
      string newDisplayName = displayName;
      auth.CreateUserWithEmailAndPasswordAsync(emailR, passwordR).ContinueWith((task) =>
      {
        return HandleCreateUserAsync(task, newDisplayName: newDisplayName);
      }).Unwrap();
    }
  }

  Task HandleCreateUserAsync(Task<FirebaseUser> authTask,
                             string newDisplayName = null) {
    if (LogTaskCompletion(authTask, "User Creation")) {
      if (auth.CurrentUser != null) {
        DebugLog(String.Format("User Info: {0}  {1}", auth.CurrentUser.Email,
                               auth.CurrentUser.ProviderId));
        
        auth.CurrentUser.SendEmailVerificationAsync().ContinueWith(task => {
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
        
        
        return UpdateUserProfileAsync(newDisplayName: newDisplayName);
  
      }
    }
    // Nothing to update, so just return a completed Task.
    return Task.FromResult(0);
  }

  // Update the user's display name with the currently selected display name.
  public Task UpdateUserProfileAsync(string newDisplayName = null) {
    if (auth.CurrentUser == null) {
      DebugLog("Not signed in, unable to update user profile");
      return Task.FromResult(0);
    }
    displayName = newDisplayName ?? displayName;
    DebugLog("Updating user profile");
    return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile {
        DisplayName = displayName,
        PhotoUrl = auth.CurrentUser.PhotoUrl,
      }).ContinueWith(HandleUpdateUserProfile);
  
  }

  void HandleUpdateUserProfile(Task authTask) {
    if (LogTaskCompletion(authTask, "User profile")) {
      DisplayDetailedUserInfo(auth.CurrentUser, 1);
    }
  }

  public void SigninAsync() {
    DebugLog(String.Format("Attempting to sign in as {0}...", emailA));
      auth.SignInWithEmailAndPasswordAsync(emailA, passwordA).ContinueWith(HandleSigninResult);
  }

  void HandleSigninResult(Task<FirebaseUser> authTask) {
    LogTaskCompletion(authTask, "Sign-in");
  }

  public void ReloadUser() {
    if (auth.CurrentUser == null) {
      DebugLog("Not signed in, unable to reload user.");
      return;
    }
    DebugLog("Reload User Data");
    auth.CurrentUser.ReloadAsync().ContinueWith(HandleReloadUser);
  }

  void HandleReloadUser(Task authTask) {
    if (LogTaskCompletion(authTask, "Reload")) {
      DisplayDetailedUserInfo(auth.CurrentUser, 1);
    }
  }

  public void GetUserToken() {
    if (auth.CurrentUser == null) {
      DebugLog("Not signed in, unable to get token.");
      return;
    }
    DebugLog("Fetching user token");
    fetchingToken = true;
    auth.CurrentUser.TokenAsync(false).ContinueWith(HandleGetUserToken);
  }

  void HandleGetUserToken(Task<string> authTask) {
    fetchingToken = false;
    if (LogTaskCompletion(authTask, "User token fetch")) {
      DebugLog("Token = " + authTask.Result);
    }
  }

  void GetUserInfo() {
    if (auth.CurrentUser == null) {
      DebugLog("Not signed in, unable to get info.");
    } else {
      DebugLog("Current user info:");
      DisplayDetailedUserInfo(auth.CurrentUser, 1);
    }
  }

  public void SignOut() {
    DebugLog("Signing out.");
    auth.SignOut();
    SceneManager.LoadScene(2);
  }
  
  public void ResetPassword()
  {
    string emailAddress = resetPassword.text;

    if (emailAddress != "")
    {   
      FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
      {
        if (task.IsCanceled)
        {
          Debug.LogError("SendPasswordResetEmailAsync was canceled.");
          return;
        }

        if (task.IsFaulted)
        {
          Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
          return;
        }

        Debug.Log("Password reset email sent successfully.");        
      });
      GameObject.Find("SceneManager").GetComponent<UserAuthAndRegistrSceneManager>().ShowPanel(PanelConfrim);
      ClearInputs();
    }
    else
    {
      GameObject.Find("ErrorMessagePasswordReset").GetComponent<TMP_Text>().text = "Заполните поле";
      GameObject.Find("ErrorMessagePasswordReset").GetComponent<TMP_Text>().color = Color.red;
    }
    
  }
  
  private bool CheckInput()
  {
    if (passwordReg.text !="" &&  verifyReg.text !="" && emailReg.text!="")
    {
      if (passwordReg.text != verifyReg.text)
      {
        errorText.color = Color.red;
        errorText.text = "Ошибка: Пароли не совпадают";
        return false;
      }
      else
      {
        errorText.color = Color.white;
        return true;
      }
    }
    else
    {
      errorText.color = Color.red;
      errorText.text = "Ошибка: Не все поля заполнены";
      return false;
    }
  }

  private void ClearInputs()
  {
   emailAuth.text  = "";
   emailReg.text  = "";
   passwordAuth.text  = "";
   passwordReg.text  = "";
   verifyReg.text  = "";
    resetPassword.text = "";
  }



}
