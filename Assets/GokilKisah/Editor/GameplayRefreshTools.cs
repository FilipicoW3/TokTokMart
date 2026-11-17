using System.IO;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

namespace GokilKisah.Editor
{
    public static class GameplayRefreshTools
    {
        public static void PrepareEncounterUI()
        {
            if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Stop Play Mode first.");
            const string path="Assets/GokilKisah/Resources/TokMartEditableUI.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                foreach(var e in root.GetComponentsInChildren<TokMartCanvasElement>(true))if(e.hasLayout)
                {
                    var rect=e.originalRect;var t=e.GetComponent<RectTransform>();
                    if(e.key.StartsWith("ShiftHud_0/Encounter_0/")&&(rect.width==1140||rect.width==1110))
                    {rect.width-=400;t.sizeDelta-=Vector2.right*400;e.originalRect=rect;}
                    if(e.key.StartsWith("ShiftHud_0/Queue_0/")&&(rect.y==194||rect.y==201))
                    {rect.y+=24;t.anchoredPosition-=Vector2.up*24;e.originalRect=rect;}
                }
                PrefabUtility.SaveAsPrefabAsset(root,path);
            }
            finally{PrefabUtility.UnloadPrefabContents(root);}
            AssetDatabase.SaveAssets();
        }
        public static void CleanTemporaryEventSystems(string folder)
        {
            var systems=Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            string source=File.ReadAllText("Assets/GokilKisah/Scenes/TokMartPitch.unity");
            var saved=new System.Collections.Generic.HashSet<ulong>();
            foreach(var system in systems)
            {
                string guid=AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(system)));
                foreach(string doc in Regex.Split(source,@"(?m)^--- "))if(doc.Contains("guid: "+guid))
                {var match=Regex.Match(doc,@"^!u!114 &(\d+)");if(match.Success)saved.Add(ulong.Parse(match.Groups[1].Value));}
            }
            var keep=systems.Where(s=>saved.Contains(GlobalObjectId.GetGlobalObjectIdSlow(s).targetObjectId)).ToArray();
            var rows=systems.Select(s=>s.name+" / "+GlobalObjectId.GetGlobalObjectIdSlow(s).targetObjectId).ToList();
            if(keep.Length==1)foreach(var system in systems)
                if(system!=keep[0]&&system.name=="UI EventSystem"&&system.transform.childCount==0)
                {rows.Add("Removed test-created duplicate "+system.GetInstanceID());Object.DestroyImmediate(system.gameObject);}
            Directory.CreateDirectory(folder);File.WriteAllLines(Path.Combine(folder,"event-systems.txt"),rows);
        }
        public static void ReproduceSpills(string folder)
        {
            if(!EditorApplication.isPlaying)throw new System.InvalidOperationException("Enter Play Mode first.");
            var game=Object.FindFirstObjectByType<StoreGameController>();game.ResetRun(StoreGameController.Encounter.Goblins);
            var update=typeof(StoreGameController).GetMethod("UpdateEncounter",BindingFlags.Instance|BindingFlags.NonPublic);
            for(int i=0;i<12;i++)update.Invoke(game,new object[]{4.1f});
            int overlaps=0;for(int i=0;i<game.spills.Count;i++)for(int j=i+1;j<game.spills.Count;j++)if(Vector2.Distance(game.spills[i],game.spills[j])<55)overlaps++;
            Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,"before-spill-repro.txt"),"Live goblin spawn loop: "+game.spills.Count+" puddles; "+overlaps+" overlapping pairs.\n"+string.Join("\n",game.spills.Select(p=>p.ToString())));
            game.ResetRun(StoreGameController.Encounter.Store);
        }
    }
}
