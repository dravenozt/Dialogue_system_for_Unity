using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace DialogueSystem
{
   
public class DialogueNode : ScriptableObject
{
   [SerializeField]
   bool isPlayerSpeaking=false;
  // public string uniqueID;
   [SerializeField] 
   string text;
   [SerializeField] 
   List<string> children= new List<string>();

   [SerializeField] 
   Rect rect= new Rect(0,0,200,100);

   public Rect GetRect(){
      return rect;
   }

   public string GetText(){
      return text;
   }

   public List<string> GetChildren(){
      return children;
   }

   public bool IsPlayerSpeaking()
   {
      return isPlayerSpeaking;
   }


#if UNITY_EDITOR
public void SetPosition(Vector2 newPosition){
      Undo.RecordObject(this,"Move Dialogue Node");
      rect.position= newPosition;
      EditorUtility.SetDirty(this);
   }


public void SetText(string newText){
   if (newText!= text)
   {
      Undo.RecordObject(this, "Update Dialogue Text");// bu kod setdirty i oto yapıyo bide üstüne ctrl z atıp geri dönebiliyosun
      text=newText;
      EditorUtility.SetDirty(this);
   }
}

public void AddChild(string childID){
   Undo.RecordObject(this, "Add dialogue link");

   children.Add(childID);
   EditorUtility.SetDirty(this);
}

public void RemoveChild(string childID)
{
   Undo.RecordObject(this, "Remove dialogue link");

   children.Remove(childID);
   EditorUtility.SetDirty(this);
}

   public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
    {
         Undo.RecordObject(this, "Change dialogue speaker");
         isPlayerSpeaking= newIsPlayerSpeaking;
         EditorUtility.SetDirty(this);
     }


#endif

    }
}

