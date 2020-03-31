using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnLoad: MonoBehaviour
{
   public GameObject ClickLabel;
   private void Start()
   {
      FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChanged;
      CheckUser();
   }

   private void OnDestroy()
   {
      FirebaseAuth.DefaultInstance.StateChanged -= HandleAuthStateChanged;
   }

   private void HandleAuthStateChanged(object sender, EventArgs e)
   {
      CheckUser();
   }

   private void CheckUser()
   {
      if (FirebaseAuth.DefaultInstance.CurrentUser != null)
         SceneManager.LoadScene(3);
      else
      {
         ClickLabel.SetActive(true);
      }
   }
}
