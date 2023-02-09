using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue", order = 0)]
public class Dialogue : ScriptableObject, ISerializationCallbackReceiver {
    [SerializeField]
    List<DialogueNode> nodes= new List<DialogueNode>(); //= new List<DialogueNode>();
    [SerializeField]
    Vector2 newNodeOffSet = new Vector2(250,0);
    Dictionary<string,DialogueNode> nodeLookup= new Dictionary<string, DialogueNode>();



#if UNITY_EDITOR // sadece editörde çalışacak kodlar
     private void Awake() {
        OnValidate();
        
    }
#endif


private void OnValidate() {
    
    nodeLookup.Clear();
    foreach (DialogueNode node in GetAllNodes())
    {
        nodeLookup[node.name]=node;
        
    }
}


public IEnumerable<DialogueNode> GetAllNodes(){
    return nodes;
}
   

public DialogueNode GetRootNode(){
    return nodes[0];
}

        public IEnumerable<DialogueNode> GetAllChilden(DialogueNode parentNode)
        {
           
            foreach (string childID in parentNode.GetChildren())
            {
               if(nodeLookup.ContainsKey(childID)){
                yield return nodeLookup[childID];
               }
                
            }

           
        }
#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "created dialogue node");
            Undo.RecordObject(this, "Added dialogue node");
            AddNode(newNode);
        }

        
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this,"Deleted dialogue node");

            nodes.Remove(nodeToDelete);
            
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);

            OnValidate();
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parent != null)
            {
                parent.AddChild(newNode.name);

                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position+ newNodeOffSet);
            }

            return newNode;
        }


        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
                        }
        }
#endif

        public void OnBeforeSerialize()
        {

#if UNITY_EDITOR
            if (nodes.Count==0)
            {   
            DialogueNode newNode = MakeNode(null);
            
            AddNode(newNode);
            }
            if(AssetDatabase.GetAssetPath(this)!=""){
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node)=="")
                    {
                        AssetDatabase.AddObjectToAsset(node,this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}

