using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
public class DBNote : MonoBehaviour
{
    private static Dictionary<string, List<MNote>> notesValues = new Dictionary<string,  List<MNote>>();

    private static DatabaseReference _reference;

    private void Start()
    {
        InitializeDatabase();
    }
    
    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");
        _reference = FirebaseDatabase.DefaultInstance.GetReference("/notes/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        _reference.ValueChanged += HandleValueChanged;
        Debug.Log("done notes");
    }
    
    private void OnDestroy()
    {
        try
        {
            _reference.ValueChanged -= HandleValueChanged;
        }
        catch
        {
            Debug.Log("Empty notes");
        }

    }

   private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        notesValues.Clear();

        DataSnapshot snapshot = args.Snapshot;
        foreach (DataSnapshot childDataSnapshot in snapshot.Children)
        {
            string date = ReplaceWith(childDataSnapshot.Key);
            List<MNote> dateNotes = new List<MNote>();

            if (childDataSnapshot.ChildrenCount > 0)
                foreach (DataSnapshot notes in childDataSnapshot.Children)
                {
                    MNote currentNote = new MNote()
                    {
                        key =  notes.Key,
                        value = notes.Value.ToString()
                    };
                    
                    if (notesValues.ContainsKey(date))
                    {
                        notesValues[date].Add(currentNote);
                    }
                    else
                    {
                        dateNotes.Add(currentNote);
                        notesValues.Add(date, dateNotes);
                    }
                }
        }
        
        
        ViewModel.SetNotesValues(notesValues);
    }

    public static void DBCategoryDelete(string key)
    {

    }

    public static void DBEventAdd(string name, string colour)
    {

    }
    
    private  string ReplaceWith(string str)
    {
        string result = "";

        result = str.Replace(str.Contains(@"/") ? "/" : ".", @"\");

        return result;
    }
    

}
