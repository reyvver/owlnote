using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class DBPlans : MonoBehaviour
{   
    private static Dictionary<string, List<MPlan>> plansValues = new Dictionary<string, List<MPlan>>();
    private static DatabaseReference _reference;
    
    private void Start()
    {
        InitializeDatabase();
    }
    
    private void InitializeDatabase()
    {
        _reference = FirebaseDatabase.DefaultInstance.GetReference("/plans/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        _reference.ValueChanged += HandleValueChanged;
        Debug.Log("done plans");
    }
    
    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        
        plansValues.Clear();
        
        DataSnapshot snapshot = args.Snapshot;
        
        foreach (DataSnapshot childDataSnapshot in snapshot.Children)
        {
            string year = childDataSnapshot.Key;
            List<MPlan> monthPlans = new List<MPlan>();

            if (childDataSnapshot.ChildrenCount > 0)
                foreach (DataSnapshot month in childDataSnapshot.Children)
                {
                    MPlan currentPlan = new MPlan();
                    string monthPlan = month.Key;

                    currentPlan.month = monthPlan;
                    currentPlan.itemsList = new Dictionary<string, bool>();
                    
                    foreach (DataSnapshot item in childDataSnapshot.Child(monthPlan).Children)
                    {
                        string name = item.Key;
                        bool status = Convert.ToBoolean(item.Value);
                        currentPlan.itemsList.Add(name,status);
                    }
                    if (plansValues.ContainsKey(year))
                    {
                        plansValues[year].Add(currentPlan);
                    }
                    else
                    {
                        monthPlans.Add(currentPlan);
                        plansValues.Add(year, monthPlans);
                    }
                }
        }
        ViewModel.SetPlansValues(plansValues);
    }
    private void OnDestroy()
    {
        try
        {
            _reference.ValueChanged -= HandleValueChanged;
        }
        catch
        {
            Debug.Log("Empty plans");
        }

    }

    public static void AddItemInPlan(string dateYear, string dateMonth, string newItem)
    {
        Debug.Log(newItem);
        _reference.Child(dateYear).Child(dateMonth).Child(newItem).SetValueAsync(false);
    }
    
    public static void DeletePLanItem(string dateYear, string dateMonth, string itemName)
    {
        _reference.Child(dateYear).Child(dateMonth).Child(itemName).RemoveValueAsync();
    }
    
    public static void UpdateItemState(string dateYear, string dateMonth, string itemName, bool value)
    {
        _reference.Child(dateYear).Child(dateMonth).Child(itemName).SetValueAsync(value);
    }
    

}
