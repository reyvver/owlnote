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
                    currentList.itemsList = new Dictionary<string, string>();

                    foreach (DataSnapshot item in childDataSnapshot.Child(listName).Children)
                    {
                        string name = item.Key;
                        string status = item.Value.ToString();
                        
                        Debug.Log(name + " "+status);
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

    private void DebugValues()
    {
        Debug.Log("мы тут");
        foreach (var obj in todoValues)
        {
            Debug.Log(obj.Key);
            foreach (var list in obj.Value)
            {
                Debug.Log(list.nameList);

                foreach (var item in list.itemsList)
                {
                    Debug.Log(item.Key + "  "+item.Value);
                }
            }
        }
    }

    public static void DeleteList(string date,string listName)
    {
        string dateNote = ReplaceWith(date);
        _reference.Child(dateNote).Child(listName).RemoveValueAsync();
    }

    public static void AddList(string date, string listName)
    {
        
    }

    public static void AddItemInList(string date, string listName, string newItem)
    {
        string dateNote = ReplaceWith(date);
        _reference.Child(dateNote).Child(listName).Child(newItem).SetValueAsync("false");
    }

    private static string ReplaceWith(string str)
    {
        string result = "";

        result = str.Replace(str.Contains(@"/") ? "/" : ".", @"\");

        return result;
    }
}
