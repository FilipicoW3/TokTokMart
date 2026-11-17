using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GokilKisah.Editor
{
    public static class FriendUIIntegration
    {
        public static void Apply()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first.");
            NovemberArtImporter.ImportUI("UI/Art_TTM_UI_StockIcon01.png","StockIcon01");
            NovemberArtImporter.ImportUI("UI/Art_TTM_UI_TimerIcon01.png","TimerIcon01");
            AssetDatabase.SaveAssets();ReleasePolishTools.PrepareUI();
        }
        public static void Verify(string folder)
        {
            Directory.CreateDirectory(folder);var checks=new List<string>();
            Action<bool,string> check=(ok,label)=>checks.Add((ok?"PASS ":"FAIL ")+label);
            var font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            check(font,"Friend's TextMesh Pro font resource loads");
            check(GameArtLibrary.UI("StockIcon01"),"Friend's stock icon is available to gameplay UI");
            check(GameArtLibrary.UI("TimerIcon01"),"Friend's timer icon is available to gameplay UI");
            var ui=TokMartCanvas.Create();
            try
            {
                ui.authoringBake=true;
                var style=new GUIStyle{fontSize=24};style.normal.textColor=Color.white;
                var rect=new Rect(20,20,200,50);
                ui.Begin(Matrix4x4.identity);ui.Label(rect,"GOAL 0",style,Matrix4x4.identity);ui.End();
                var binding=ui.GetComponentsInChildren<TokMartCanvasElement>().Single(e=>e.kind=="Text");
                UnityEngine.Object.DestroyImmediate(binding.GetComponent<Text>());
                var tmp=binding.gameObject.AddComponent<TextMeshProUGUI>();tmp.font=font;tmp.text="Designer text";
                ui.authoringBake=false;int objects=ui.GetComponentsInChildren<Transform>(true).Length;
                style.fontSize=28;style.normal.textColor=Color.green;
                ui.Begin(Matrix4x4.identity);ui.Label(rect,"GOAL 4",style,Matrix4x4.identity);ui.End();
                check(tmp.text=="GOAL 4"&&Mathf.Approximately(tmp.fontSize,28),"Native label binding updates TMP counters and size");
                check(tmp.color==TokMartCanvas.GuiColor(Color.green),"Native label binding updates TMP feedback color");
                check(tmp.font==font,"Binding preserves the designer's TMP font");
                check(!binding.GetComponent<Text>()&&ui.GetComponentsInChildren<Transform>(true).Length==objects,"TMP binding adds no replacement text component or UI object");
                check(ui.MissingBindingCount==0,"TMP-authored label is accepted by the existing presenter");
                binding.bindGameData=false;tmp.text="My authored label";
                ui.Begin(Matrix4x4.identity);ui.Label(rect,"GOAL 9",style,Matrix4x4.identity);ui.End();
                check(tmp.text=="My authored label","Disabling game-data binding preserves TMP-authored text");
                binding.bindGameData=true;binding.ResetBindingPreview(false);
                check(tmp.text=="GOAL 0"&&tmp.font==font,"TMP binding preview restores data without replacing the font");
            }
            finally{UnityEngine.Object.DestroyImmediate(ui.gameObject);}
            File.WriteAllLines(Path.Combine(folder,"tmp-bindings.txt"),checks);
            if(checks.Any(s=>s.StartsWith("FAIL")))throw new InvalidOperationException("PR UI binding checks failed.");
        }
    }
}
