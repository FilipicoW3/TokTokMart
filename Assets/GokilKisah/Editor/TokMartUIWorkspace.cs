using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GokilKisah.Editor
{
    public sealed class TokMartUIWorkspace : EditorWindow
    {
        const string PrefabPath="Assets/GokilKisah/Resources/TokMartEditableUI.prefab";
        const string BackupPath="UI-Backups/Before-UI-Workspace-20261117";
        static readonly string[] LegacyKeys={"DrawShiftBriefing_0","DrawArcadeResults_0","DrawNovemberHud_0","DrawNovemberMenu_0","November11ScoreGuide_0","November11Briefing_0","November11Results_0","November11HUD_0","November11Menu_0"};
        static readonly string[] ScreenKeys={"ArtMenu_0","ShiftSelect_0","DrawArcadeSelection_0","ShiftBrief_0","ShiftResults_0","ShiftHud_0","DrawMobileControls_0","DrawSkill0_0","DrawSkill1_0","DrawSkill2_0","StarHint_0","DrawGameOver_0","DrawSettings0_0","DrawSettings1_0","DrawSettings2_0","DrawSettings3_0","DrawSettings4_0","DrawSettings5_0"};
        static readonly string[] ScreenLabels={"Main Menu","Choose a Shift","Character Select","Shift Briefing","Shift Results","Shift HUD","Touch Controls","Restock Task","Checkout Task","Cleaning Task","Star Guide","Game Over","Settings - Audio","Settings - Display","Settings - Gameplay","Settings - Pitch Tools","Settings - Controls","Credits"};
        [SerializeField] TokMartCanvas canvas;
        Vector2 scroll;

        [MenuItem("TokTokMart/UI/Open UI Layout Editor")]
        public static void OpenMenu()=>Open(UnityEngine.Object.FindFirstObjectByType<TokMartCanvas>(FindObjectsInactive.Include));
        public static void Open(TokMartCanvas ui)
        {
            var window=GetWindow<TokMartUIWorkspace>("UI Layout Editor");
            window.canvas=ui;window.minSize=new Vector2(310,440);window.Show();
        }
        public static TokMartCanvasElement Find(TokMartCanvas ui,string key)=>ui?ui.GetComponentsInChildren<TokMartCanvasElement>(true).FirstOrDefault(e=>e.key==key||e.key.EndsWith("/"+key,StringComparison.Ordinal)):null;
        static bool IsScreen(TokMartCanvasElement element)=>ScreenKeys.Any(key=>element.key==key||element.key.EndsWith("/"+key,StringComparison.Ordinal));

        public static void Preview(TokMartCanvas ui,string key)
        {
            if(Application.isPlaying||!ui||!ui.designRoot)return;
            var selected=Find(ui,key);if(!selected)return;
            Undo.RegisterFullObjectHierarchyUndo(ui.gameObject,"Preview UI screen");
            foreach(var e in ui.GetComponentsInChildren<TokMartCanvasElement>(true))e.gameObject.SetActive(false);
            void Show(TokMartCanvasElement screen)
            {
                foreach(var e in screen.GetComponentsInChildren<TokMartCanvasElement>(true))
                {
                    bool nestedScreen=false;
                    for(var t=e.transform;t&&t!=screen.transform;t=t.parent)
                    {var ancestor=t.GetComponent<TokMartCanvasElement>();if(ancestor&&IsScreen(ancestor)){nestedScreen=true;break;}}
                    e.gameObject.SetActive(!nestedScreen&&!e.hideFromGame);
                }
            }
            // Settings appear on the normal menu; nested level-map groups stay hidden.
            if(key.StartsWith("DrawSettings",StringComparison.Ordinal)){var menu=Find(ui,"ArtMenu_0");if(menu)Show(menu);}
            Show(selected);
            for(var ancestor=selected.transform;ancestor&&ancestor!=ui.designRoot;ancestor=ancestor.parent)
            {var e=ancestor.GetComponent<TokMartCanvasElement>();ancestor.gameObject.SetActive(!e||!e.hideFromGame);}
            Selection.activeGameObject=selected.gameObject;
            var sceneView=SceneView.lastActiveSceneView;
            if(sceneView){sceneView.in2DMode=true;sceneView.Frame(new Bounds(ui.designRoot.position,new Vector3(1600,900,1)*ui.designRoot.lossyScale.x),true);}
            EditorUtility.SetDirty(ui);
            if(ui.gameObject.scene.IsValid())EditorSceneManager.MarkSceneDirty(ui.gameObject.scene);
            SceneView.RepaintAll();
        }

        void OnGUI()
        {
            if(!canvas)canvas=UnityEngine.Object.FindFirstObjectByType<TokMartCanvas>(FindObjectsInactive.Include);
            canvas=(TokMartCanvas)EditorGUILayout.ObjectField("Canvas",canvas,typeof(TokMartCanvas),true);
            if(!canvas){if(GUILayout.Button("Find Scene Canvas"))canvas=UnityEngine.Object.FindFirstObjectByType<TokMartCanvas>(FindObjectsInactive.Include);return;}
            EditorGUILayout.HelpBox("Edit with Play stopped. Preview a screen, then use Scene for layout or Game for the final view. Select a group, press T, and adjust its Rect Transform. Save the scene with Ctrl+S.",MessageType.Info);
            using(new EditorGUI.DisabledScope(Application.isPlaying))
            {
                scroll=EditorGUILayout.BeginScrollView(scroll);
                EditorGUILayout.LabelField("Screen previews",EditorStyles.boldLabel);
                for(int i=0;i<ScreenKeys.Length;i++)
                    if(Find(canvas,ScreenKeys[i])&&GUILayout.Button(ScreenLabels[i]))Preview(canvas,ScreenKeys[i]);
                EditorGUILayout.Space();EditorGUILayout.LabelField("Main menu fade",EditorStyles.boldLabel);
                var fade=canvas.GetComponentInChildren<TokMartHorizontalFade>(true);
                if(fade)
                {
                    var binding=fade.GetComponent<TokMartCanvasElement>();
                    bool hidden=EditorGUILayout.Toggle("Hide menu fill",binding.hideFromGame);
                    if(hidden!=binding.hideFromGame){Undo.RecordObject(binding,"Toggle menu fill");binding.hideFromGame=hidden;SaveChange(binding);Preview(canvas,"ArtMenu_0");}
                    var serialized=new SerializedObject(fade);serialized.Update();
                    EditorGUILayout.PropertyField(serialized.FindProperty("leftOpacity"));
                    EditorGUILayout.PropertyField(serialized.FindProperty("rightOpacity"));
                    if(serialized.ApplyModifiedProperties()){fade.GetComponent<Graphic>().SetVerticesDirty();SaveChange(fade);}
                    if(GUILayout.Button("Select Fill - Position / Size / Color")){Preview(canvas,"ArtMenu_0");Selection.activeGameObject=fade.gameObject;}
                }
                EditorGUILayout.Space();EditorGUILayout.LabelField("Credits text",EditorStyles.boldLabel);
                EditCredit("Heading","StoreInterface_23154_0",false);
                EditCredit("Names, roles and sources","StoreInterface_23246_0",true);
                EditCredit("Footer","StoreInterface_23631_0",true);
                if(GUILayout.Button("Preview Credits"))Preview(canvas,"DrawSettings5_0");
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("To remove a decorative item from view, enable Hide From Game on its Tok Mart Canvas Element. Hide a whole group to include its children. Move an entire button group so artwork and its hit area stay together. Edit outside Play mode so changes persist.",MessageType.None);
                EditorGUILayout.EndScrollView();
            }
        }
        void EditCredit(string label,string key,bool multiline)
        {
            var screen=Find(canvas,"DrawSettings5_0");if(!screen)return;
            var group=screen.GetComponentsInChildren<TokMartCanvasElement>(true).FirstOrDefault(e=>e.key.EndsWith("/"+key,StringComparison.Ordinal));
            var text=group?group.GetComponentInChildren<Text>(true):null;
            var tmp=group?group.GetComponentInChildren<TMP_Text>(true):null;
            if(!text&&!tmp)return;
            string before=text?text.text:tmp.text;
            EditorGUILayout.LabelField(label);
            string after=multiline?EditorGUILayout.TextArea(before,new GUIStyle(EditorStyles.textArea){wordWrap=true},GUILayout.Width(Mathf.Max(240,position.width-30)),GUILayout.MinHeight(label=="Footer"?58:180)):EditorGUILayout.TextField(before);
            if(before==after)return;
            var component=text?(Component)text:tmp;Undo.RecordObject(component,"Edit credits");
            if(text)text.text=after;else tmp.text=after;
            var binding=component.GetComponent<TokMartCanvasElement>();Undo.RecordObject(binding,"Keep authored credits");binding.bindGameData=false;
            SaveChange(binding);SaveChange(component);
        }
        static void SaveChange(UnityEngine.Object obj)
        {
            EditorUtility.SetDirty(obj);
            if(PrefabUtility.IsPartOfPrefabInstance(obj))PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
            if(obj is Component c&&c.gameObject.scene.IsValid())EditorSceneManager.MarkSceneDirty(c.gameObject.scene);
        }

        [MenuItem("TokTokMart/UI/Archive old screens and prepare layout editing")]
        public static void Prepare()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play mode first.");
            var ui=UnityEngine.Object.FindFirstObjectByType<TokMartCanvas>(FindObjectsInactive.Include);
            if(!ui)throw new InvalidOperationException("Open the TokMartPitch scene first.");
            Directory.CreateDirectory(BackupPath);
            string backupPrefab=BackupPath+"/TokMartEditableUI.prefab";
            if(!File.Exists(backupPrefab))File.Copy(PrefabPath,backupPrefab);
            string backupScene=BackupPath+"/TokMartPitch-unsaved-editor.unity";
            if(!File.Exists(backupScene))EditorSceneManager.SaveScene(ui.gameObject.scene,backupScene,true);
            var contents=PrefabUtility.LoadPrefabContents(PrefabPath);
            int archived;
            try{archived=PrepareCanvas(contents.GetComponent<TokMartCanvas>());PrefabUtility.SaveAsPrefabAsset(contents,PrefabPath);}
            finally{PrefabUtility.UnloadPrefabContents(contents);}
            Undo.RegisterFullObjectHierarchyUndo(ui.gameObject,"Prepare UI layout workspace");
            PrepareCanvas(ui);Preview(ui,"ArtMenu_0");
            EditorSceneManager.SaveScene(ui.gameObject.scene);AssetDatabase.SaveAssets();
            File.WriteAllText(BackupPath+"/README.md","# UI archive\n\nFull original Canvas prefab and scene (including unsaved editor changes) before cleanup.\n\nRemoved obsolete root screen groups from the active Canvas: "+string.Join(", ",LegacyKeys)+". Current screen binding keys and authored placements are retained. No player build was made.\n");
            Directory.CreateDirectory("Verification/UIWorkspace");
            File.WriteAllText("Verification/UIWorkspace/preparation.txt","Archived legacy prefab screen groups: "+archived+"\nScene elements: "+ui.GetComponentsInChildren<TokMartCanvasElement>(true).Length+"\n"+string.Join("\n",ui.GetComponentsInChildren<TokMartCanvasElement>(true).Where(e=>e.transform.parent==ui.designRoot).Select(e=>e.key+" : "+e.name)));
            Open(ui);Debug.Log("UI workspace ready. Old screens archived; menu fade and authored credits saved. No player build performed.");
        }
        static int PrepareCanvas(TokMartCanvas ui)
        {
            var fill=Find(ui,"ArtMenu_0/_0/Image_0");
            if(!fill||!fill.GetComponent<RawImage>())throw new InvalidOperationException("Could not identify the existing menu fill; no replacement was generated.");
            var credits=Find(ui,"DrawSettings5_0");
            if(!credits)throw new InvalidOperationException("Authored credits screen is missing.");
            int count=0;
            foreach(var element in ui.GetComponentsInChildren<TokMartCanvasElement>(true).Where(e=>e.transform.parent==ui.designRoot&&LegacyKeys.Contains(e.key)).ToArray())
            {UnityEngine.Object.DestroyImmediate(element.gameObject);count++;}
            for(int i=0;i<ScreenKeys.Length;i++){var screen=Find(ui,ScreenKeys[i]);if(screen){screen.name=ScreenLabels[i];SaveChange(screen.gameObject);}}
            fill.transform.parent.name="Menu Fill (hide or resize)";fill.name="Black Fade - Left 100% to Right 0%";
            fill.bindGameData=false;fill.animateLayout=false;
            var raw=fill.GetComponent<RawImage>();raw.color=Color.black;raw.raycastTarget=false;
            if(!fill.GetComponent<TokMartHorizontalFade>())fill.gameObject.AddComponent<TokMartHorizontalFade>();
            SaveChange(fill);SaveChange(raw);SaveChange(fill.gameObject);SaveChange(fill.transform.parent.gameObject);
            foreach(var e in credits.GetComponentsInChildren<TokMartCanvasElement>(true).Where(e=>e.kind=="Text"&&(e.key.Contains("/StoreInterface_23154_0/")||e.key.Contains("/StoreInterface_23246_0/")||e.key.Contains("/StoreInterface_23631_0/"))))
            {
                e.bindGameData=false;SaveChange(e);
                if(e.key.Contains("/StoreInterface_23246_0/"))
                {
                    const string oldFontCredit="Lato / Lilita One\nŁukasz Dziedzic / Juan Montoreano · SIL OFL";
                    var text=e.GetComponent<Text>();var tmp=e.GetComponent<TMP_Text>();
                    if(text&&text.text.Contains(oldFontCredit)){text.text=text.text.Replace(oldFontCredit,"Bigbesty\nKhurasan · supplied project font");SaveChange(text);}
                    if(tmp&&tmp.text.Contains(oldFontCredit)){tmp.text=tmp.text.Replace(oldFontCredit,"Bigbesty\nKhurasan · supplied project font");SaveChange(tmp);}
                }
            }
            return count;
        }
    }
}
