using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace DialogueSystem.Editor
{
    public class Editor : EditorWindow
{
    Dialogue selectedDialogue=null;
    [NonSerialized]
    GUIStyle nodeStyle;
    [NonSerialized]
    GUIStyle playerNodeStyle;
    //bool isDragging=false;
    [NonSerialized]
    DialogueNode draggingNode= null;
    [NonSerialized]
    Vector2 draggingOffSet;
    [NonSerialized]
    DialogueNode creatingNode=null;
    [NonSerialized]
    DialogueNode deletingNode=null;
    [NonSerialized]
    DialogueNode linkingParentNode=null;
    Vector2 scrollPosition;
    [NonSerialized]
    bool draggingCanvas=false;
    [NonSerialized]
    Vector2 draggingCanvasOffset;
    const float canvasSize=4000;
    const float arrowSize=6f;


    [MenuItem("Window/Dialogue Editor")] // yani windowdan dialogu.... gelince show editor window fonksiyonunu çağıracak.
    //aslında aşşadaki fonkun start veya update den hiçbi farkı yok
    public static void ShowEditorWindow(){
        GetWindow(typeof(Editor), false , "Dialogue Editor");//bu kodla bi tane window açılmasını sağlıyoruz
         // buurada false dediğimiz şey utility window olmasın
        //peki utility window dediğimiz şey ne 
        // utility window; tek seferlik, bi yere iliştiremediğimizbi window, yani ayarı yapar kapatırsın bu kadar. Biz bunu istemiyoruz false yaptık
        //biz animator, tile palette vs gibi bi window istediğimiz için false yaptık onu
 
    }


    //şimdi, so e tıklayınca bu windowun açılmasını sağlayacak kodu yazacağız


    [OnOpenAssetAttribute(1)]//bu sıra belirtiyo yani ilk bu fonku çağıracak doubleclick atınca
    public static bool OnOpenAsset(int instanceID, int line){
        Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
        if (dialogue!= null)
        {
            ShowEditorWindow();
        }
        return false;
    }   


    private void OnEnable() {
        Selection.selectionChanged +=OnSelectionChanged;
        //aşşada node ları stilize ediyoruz sadece
        nodeStyle=new GUIStyle();
        nodeStyle.normal.background=EditorGUIUtility.Load("node0") as Texture2D;
        nodeStyle.padding= new RectOffset(20,20,20,20);
        nodeStyle.border= new RectOffset(12,12,12,12);
        
        
        playerNodeStyle=new GUIStyle();
        playerNodeStyle.normal.background=EditorGUIUtility.Load("node1") as Texture2D;
        playerNodeStyle.padding= new RectOffset(20,20,20,20);
        playerNodeStyle.border= new RectOffset(12,12,12,12);
    }


    private void OnSelectionChanged() {
        Dialogue newDialogue = Selection.activeObject as Dialogue;

        if (newDialogue!=null)
        {
            selectedDialogue= newDialogue;
        }
    }



    private void OnGUI() {
        

        if (selectedDialogue==null)
        {
            EditorGUILayout.LabelField("No dialogue selected"); // bu fonksiyonla istediğimiz pozisyona istediğimiz yazıyı yazıyoruz sadece
        }
        else
        {
            ProcessEvents();
            // buradaki kod senin text kısmına yazdığın textleri pencereye yazdırıyo
            scrollPosition= EditorGUILayout.BeginScrollView(scrollPosition);

           Rect canvas= GUILayoutUtility.GetRect(canvasSize,canvasSize);
            
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    
                    DrawConnections(node);

                }

            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                    

                }

            EditorGUILayout.EndScrollView();

            if (creatingNode!= null)
            {
                
                selectedDialogue.CreateNode(creatingNode);
                creatingNode=null;
            }
            if (deletingNode!=null)
            {   
                selectedDialogue.DeleteNode(deletingNode);
                deletingNode=null;
            }
        }

        
    }

        private void DrawConnections(DialogueNode node)
        {
            foreach (DialogueNode childNode in selectedDialogue.GetAllChilden(node))
            {
                Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controllPointOffSet = new Vector2(100, 0);
                Handles.DrawBezier(startPosition, endPosition, startPosition + controllPointOffSet, endPosition - controllPointOffSet, Color.white, null, 4f);

                //parent tan childa yön belirlemek için///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                DrawArrowsParentToChild(startPosition, endPosition);

            }
        }

        private void DrawArrowsParentToChild(Vector3 startPosition, Vector3 endPosition)
        {
            Vector2 direction = endPosition - startPosition;
            Vector2 midPosition = (endPosition + startPosition) / 2f;


            Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * arrowSize;
            Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * arrowSize;

            Vector2 arrowHeadPoint = midPosition + direction.normalized * arrowSize;

            //draw arrows
            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, 4f);
            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, 4f);
        }

        private void ProcessEvents(){
        if (Event.current.type==EventType.MouseDown&&draggingNode==null)
        {
            draggingNode= GetNodeAtPoint(Event.current.mousePosition + scrollPosition);

            if (draggingNode!= null)
            {
                draggingOffSet= draggingNode.GetRect().position-Event.current.mousePosition;
                Selection.activeObject= draggingNode;
            }
            else
            {
                draggingCanvas=true;
                draggingCanvasOffset=Event.current.mousePosition + scrollPosition;
                Selection.activeObject= selectedDialogue;
            }
        }
        else if(Event.current.type==EventType.MouseDrag&&draggingNode!=null)
        {
            
            draggingNode.SetPosition( Event.current.mousePosition + draggingOffSet);
            
            GUI.changed=true;
        }
        else if(Event.current.type==EventType.MouseDrag&&draggingCanvas){
            scrollPosition=draggingCanvasOffset-Event.current.mousePosition;
            GUI.changed=true;
        }
        else if (Event.current.type==EventType.MouseUp&&draggingNode!=null){
            draggingNode=null;
            
        }
        else if(Event.current.type==EventType.MouseUp&&draggingCanvas)
        {
            draggingCanvas=false;
        }
    }


    private DialogueNode GetNodeAtPoint(Vector2 point){
        DialogueNode foundNode= null;
        foreach (DialogueNode node in selectedDialogue.GetAllNodes())
        {
            if (node.GetRect().Contains(point))
            {
                foundNode=node;
            }
        }
        return foundNode;
    }

        private void DrawNode(DialogueNode node)
        {

            GUIStyle style=nodeStyle;

            if (node.IsPlayerSpeaking())
            {
                style=playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);

            //EditorGUI.BeginChangeCheck();


            node.SetText(EditorGUILayout.TextField(node.GetText()));


           

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                creatingNode = node;

            }
            if (GUILayout.Button("Delete"))
            {
                deletingNode = node;

            }
            GUILayout.EndHorizontal();

            DrawLinkButtons(node);

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {

                    linkingParentNode = node;
                }

            }
            else if(linkingParentNode==node)
            {
                if (GUILayout.Button("Cancel"))
                {

                    linkingParentNode = null;
                }
            }

            else if(linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink child"))
                {

                    
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Set as child"))
                {

                    Undo.RecordObject(selectedDialogue, "Add dialogue link");
                    linkingParentNode.AddChild( node.name);
                    linkingParentNode = null;
                }
            }
        }
    }
}
