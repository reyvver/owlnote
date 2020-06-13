using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class DBTodo : MonoBehaviour
{
    private static Dictionary<string, List<MTodo>> todoValues = new Dictionary<string, List<MTodo>>();
    private static DatabaseReference _reference;

    private void Start()
    {
        InitializeDatabase();
    }
    private void InitializeDatabase()
    {
        _reference =
            FirebaseDatabase.DefaultInstance.GetReference("/todo/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        _reference.ValueChanged += HandleValueChanged;
        Debug.Log("done todo");
    }
    private void OnDestroy()
    {
        try
        {
            _reference.ValueChanged -= HandleValueChanged;
        }
        catch
        {
            Debug.Log("Empty todo");
        }

    }

    
    
    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        todoValues.Clear();

        DataSnapshot snapshot = args.Snapshot;

        foreach (DataSnapshot childDataSnapshot in snapshot.Children)
        {
            string date = ReplaceWith(childDataSnapshot.Key);
            List<MTodo> dateLists = new List<MTodo>();

            if (childDataSnapshot.ChildrenCount > 0)
                foreach (DataSnapshot list in childDataSnapshot.Children)
                {
                    MTodo currentList = new MTodo();
                    
                    string listName = list.Key;
                    currentList.nameList = listName;
                    currentList.itemsList = new Dictionary<string, bool>();

                    foreach (DataSnapshot item in childDataSnapshot.Child(listName).Children)
                    {
                        string name = item.Key;
                        bool status = Convert.ToBoolean(item.Value);
                        currentList.itemsList.Add(name,status);
                    }
                    
                    if (todoValues.ContainsKey(date))
                    {
                        todoValues[date].Add(currentList);
                    }
                    else
                    {
                        dateLists.Add(currentList);
                        todoValues.Add(date, dateLists);
                    }
                }
        }
        ViewModel.SetTodoValues(todoValues);
    }

    public static void DeleteList(string date,string listName)
    {
        string dateTodo = ReplaceWith(date);
        _reference.Child(dateTodo).Child(listName).RemoveValueAsync();
    }
    public static void DeleteListItem(string date,string listName,string itemName)
    {
        string dateTodo = ReplaceWith(date);
        _reference.Child(dateTodo).Child(listName).Child(itemName).RemoveValueAsync();
    }

    public static void UpdateItemState(string date,string listName,string itemName)
    {
        string dateTodo = ReplaceWith(date);
        _reference.Child(dateTodo).Child(listName).Child(itemName).SetValueAsync(true);
    }

    public static void AddList(string date, MTodo list)
    {
        string dateTodo = ReplaceWith(date);
        string listName = list.nameList;

        foreach (var item in list.itemsList)
        {
            _reference.Child(dateTodo).Child(listName).Child(item.Key).SetValueAsync(item.Value);
        }
    }

    public static void AddItemInList(string date, string listName, string newItem)
    {
        string dateTodo = ReplaceWith(date);
        _reference.Child(dateTodo).Child(listName).Child(newItem).SetValueAsync("false");
    }

    private static string ReplaceWith(string str)
    {
        string result = "";

        result = str.Replace(str.Contains(@"/") ? "/" : ".", @"\");

        return result;
    }

}
