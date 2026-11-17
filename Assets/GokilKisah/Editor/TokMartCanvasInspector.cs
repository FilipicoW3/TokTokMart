using UnityEditor;
using UnityEngine;
namespace GokilKisah.Editor
{
    [CustomEditor(typeof(TokMartCanvas))]
    public class TokMartCanvasInspector:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var ui=(TokMartCanvas)target;
            EditorGUILayout.HelpBox("Stop Play mode before editing. Open the layout editor to preview screens, select the menu fade and edit credits. Use the Rect tool (T) and Rect Transform for placement and size. Hide From Game keeps an element hidden during play.",MessageType.Info);
            if(GUILayout.Button("Open UI Layout Editor"))TokMartUIWorkspace.Open(ui);
            if(!Application.isPlaying)
            {
                if(GUILayout.Button("Preview Main Menu"))TokMartUIWorkspace.Preview(ui,"ArtMenu_0");
                if(GUILayout.Button("Preview Shift Briefing"))TokMartUIWorkspace.Preview(ui,"ShiftBrief_0");
                if(GUILayout.Button("Preview Credits"))TokMartUIWorkspace.Preview(ui,"DrawSettings5_0");
            }
            DrawDefaultInspector();
        }
    }
}
