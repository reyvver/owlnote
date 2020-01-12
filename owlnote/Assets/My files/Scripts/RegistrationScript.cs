using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RegistrationScript: MonoBehaviour {

 // protected Firebase.Auth.FirebaseAuth Auth;
 // protected Dictionary<string, Firebase.Auth.FirebaseUser> UserByAuth = new Dictionary<string, Firebase.Auth.FirebaseUser>();

  private Coroutine _Registration;
  
  public TextMeshProUGUI Email;
  public TextMeshProUGUI Password;
  public TextMeshProUGUI VerifyPassword;

  public void CreateUser()
  {
      if (CheckValue())
      {
         // _Registration = StartCoroutine(CreateUserByEmail(Email.text, Password.text));
         CreateUserByEmail(Email.text, Password.text);
          Debug.Log("Done");
      }
  }

  private void CreateUserByEmail(string email, string password)
  {
      try
      {
          //var registration = Auth.CreateUserWithEmailAndPasswordAsync(email, password);
          // yield return new WaitUntil(predicate: () => registration.IsCompleted);

          //  if (registration.Exception != null)
          //     Debug.Log("Что-то пошло не так");

          //  _Registration = null;
      }
      catch (Exception ex)
      {
          Debug.Log(ex.Message);
      }
  }

  private bool CheckValue()
  {
      bool chk;
      //Чот не работает это условие,не понимаю почему. Из-за того, что в placeholder что то есть?
      if (string.IsNullOrEmpty(Email.text) || string.IsNullOrEmpty(Password.text) || string.IsNullOrEmpty(VerifyPassword.text)) {
          Debug.Log("Не все поля заполнены");
          chk = false;
      } 
      
      if (Password.text != VerifyPassword.text)
      {
          Debug.Log("Введеные пароли не совпадают");
          chk = false;
      }
      else chk = true;

      return chk;
  }
  

}
