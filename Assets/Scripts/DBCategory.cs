﻿using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;


public class DBCategory : MonoBehaviour
{
    private static Dictionary<string, string> categoriesValues = new Dictionary<string, string>();

    private static DatabaseReference _reference;

    private void Start()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        _reference =
            FirebaseDatabase.DefaultInstance.GetReference(
                "/categories/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        _reference.ValueChanged += HandleValueChanged;
        Debug.Log("done categories");
    }

    private void OnDestroy()
    {
        try
        {
            _reference.ValueChanged -= HandleValueChanged;
        }
        catch
        {
            Debug.Log("Empty categories");
        }

    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        categoriesValues.Clear();

        DataSnapshot snapshot = args.Snapshot;

        if (snapshot.ChildrenCount == 0)
        {
            if(!DBUser._deleteProcess)
                AddDefaultCategory();
        }
        else
        {
            foreach (DataSnapshot childDataSnapshot in snapshot.Children)
            {
                string name = childDataSnapshot.Key;
                string color = (string) childDataSnapshot.GetValue(true);

                categoriesValues.Add(name, color);
            }
        }

        ViewModel.SetCategoriesValues(categoriesValues);
    }

    public static void DBCategoryDelete(string key)
    {
        _reference.Child(key).RemoveValueAsync();
        ViewModel.UpdateEventsAfterDeletingCategory(key);
    }

    public static void DBEventAdd(string name, string colour)
    {
        _reference.Child(name).SetValueAsync(colour);
    }

    private void AddDefaultCategory()
    {
        _reference.Child("По умолчанию").SetValueAsync("#9792F7");
    }
    

}
