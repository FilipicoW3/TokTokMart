using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GokilKisah.Editor
{
    [InitializeOnLoad]
    public static class ReleasePolishTools
    {
        const string Art="Assets/GokilKisah/ArtSources/November17/";
        const string Runtime="Assets/GokilKisah/Resources/NovemberArt/";
        const string Prefab="Assets/GokilKisah/Resources/TokMartEditableUI.prefab";
        static System.Collections.Generic.HashSet<string> existingKeys;
        [Serializable] class LayoutEntry {public string key;public float x,y,w,h,px,py;}
        [Serializable] class LayoutFile {public LayoutEntry[] entries;}
        static System.Collections.Generic.Dictionary<string,LayoutEntry> capturedLayouts;
        static LayoutEntry Capture(TokMartCanvasElement e){var r=e.GetComponent<RectTransform>();return new LayoutEntry{key=e.key,x=r.anchoredPosition.x,y=r.anchoredPosition.y,w=r.sizeDelta.x,h=r.sizeDelta.y,px=r.pivot.x,py=r.pivot.y};}
        static int Phase {get=>SessionState.GetInt("TokMart.ReleaseUI",0);set=>SessionState.SetInt("TokMart.ReleaseUI",value);}
        static ReleasePolishTools(){EditorApplication.update+=Tick;}
        public static void ImportArt(string source)
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first");
            Directory.CreateDirectory(Art);Directory.CreateDirectory(Runtime+"Customers");AssetDatabase.Refresh();
            foreach(string skin in new[]{"Cat","Girl","Zomboy","Zomgirl"})
            {
                string file=skin=="Cat"?"cat.png":skin=="Girl"?"girl toktok.png":skin.ToLowerInvariant()+".png";
                Copy(Path.Combine(source,"Character",file),Art+file);StaticSprite(Art+file,skin);
                if(skin=="Cat")continue;
                foreach(string pose in new[]{"Walk","Idle"})
                {
                    string relative="Animation/"+skin+"/"+skin+pose+"/"+skin+pose+"_Sheet_24FPS.png";
                    Copy(Path.Combine(source,relative),Art+skin+pose+".png");ImportSequence(Art+skin+pose+".png",skin+pose);
                }
            }
            foreach(var pair in new[]{("UI/Asset 12.png","StartButton"),("UI/Asset 13.png","SettingsButton"),("UI/Asset 14.png","LevelsButton"),("UI/Asset 15.png","MenuPole"),("Environment/main menu background.png","MenuBackground")})
            {
                Copy(Path.Combine(source,pair.Item1),Art+pair.Item2+".png");
                Copy(Path.Combine(source,pair.Item1),Runtime+"UI/"+pair.Item2+".png");
                Texture(Runtime+"UI/"+pair.Item2+".png",pair.Item2=="MenuBackground"?2048:1024);
            }
            AssetDatabase.SaveAssets();Debug.Log("Release art imported: cat, TokTok girl, two pocong customers and supplied menu signs.");
        }
        static void Copy(string from,string to){Directory.CreateDirectory(Path.GetDirectoryName(to));File.Copy(from,to,true);AssetDatabase.ImportAsset(to);}
        static void Texture(string path,int max)
        {
            var imp=(TextureImporter)AssetImporter.GetAtPath(path);imp.textureType=TextureImporterType.Default;imp.npotScale=TextureImporterNPOTScale.None;imp.maxTextureSize=max;
            imp.alphaIsTransparency=true;imp.mipmapEnabled=false;imp.isReadable=false;imp.filterMode=FilterMode.Bilinear;imp.wrapMode=TextureWrapMode.Clamp;
            imp.textureCompression=TextureImporterCompression.Compressed;imp.SaveAndReimport();
        }
        static RectInt Bounds(Color32[] pixels,int width,int height)
        {
            int l=width,b=height,r=-1,t=-1;
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(pixels[y*width+x].a>8){l=Math.Min(l,x);r=Math.Max(r,x);b=Math.Min(b,y);t=Math.Max(t,y);}
            return r<l?new RectInt():new RectInt(l,b,r-l+1,t-b+1);
        }
        static void StaticSprite(string path,string skin)
        {
            var read=new Texture2D(2,2);read.LoadImage(File.ReadAllBytes(path));var b=Bounds(read.GetPixels32(),read.width,read.height);UnityEngine.Object.DestroyImmediate(read);
            Texture(path,1024);var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            SaveSprite(Sprite.Create(texture,new Rect(b.x,b.y,b.width,b.height),new Vector2(.5f,0),b.height/1.88f,0,SpriteMeshType.FullRect),"Assets/GokilKisah/Resources/PresentationSprites/"+skin+"Static.asset");
        }
        static void SaveSprite(Sprite sprite,string path)
        {
            var old=AssetDatabase.LoadAssetAtPath<Sprite>(path);sprite.name=Path.GetFileNameWithoutExtension(path);
            if(old){EditorUtility.CopySerialized(sprite,old);UnityEngine.Object.DestroyImmediate(sprite);}else AssetDatabase.CreateAsset(sprite,path);
        }
        static void ImportSequence(string path,string name)
        {
            var read=new Texture2D(2,2);read.LoadImage(File.ReadAllBytes(path));int cols=read.width/256,rows=read.height/256;
            var boxes=new System.Collections.Generic.List<RectInt>();int l=256,b=256,r=0,t=0;
            for(int i=0;i<cols*rows;i++)
            {
                var box=Bounds(Array.ConvertAll(read.GetPixels(i%cols*256,(rows-1-i/cols)*256,256,256),c=>(Color32)c),256,256);
                if(box.width==0)break;boxes.Add(box);l=Math.Min(l,box.x);b=Math.Min(b,box.y);r=Math.Max(r,box.xMax);t=Math.Max(t,box.yMax);
            }
            UnityEngine.Object.DestroyImmediate(read);Texture(path,2048);var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Directory.CreateDirectory(Runtime+"Customers/"+name);AssetDatabase.Refresh();
            for(int i=0;i<boxes.Count;i++)SaveSprite(Sprite.Create(texture,new Rect(i%cols*256+l,(rows-1-i/cols)*256+b,r-l,t-b),new Vector2(.5f,0),(t-b)/1.88f,0,SpriteMeshType.FullRect),Runtime+"Customers/"+name+"/"+i.ToString("D3")+".asset");
        }
        public static void PrepareUI()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first");
            Directory.CreateDirectory("UI-Backups");EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),"UI-Backups/Before-Release05.unity",true);
            File.Copy(Prefab,"UI-Backups/Before-Release05.prefab",true);
            Phase=1;EditorApplication.ExecuteMenuItem("Window/General/Game");EditorApplication.isPlaying=true;
        }
        static void Tick()
        {
            if(Phase==0||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            try
            {
                var game=UnityEngine.Object.FindFirstObjectByType<StoreGameController>();
                if(Phase==1&&EditorApplication.isPlaying&&game)
                {
                    var entries=game.editableUI.GetComponentsInChildren<TokMartCanvasElement>(true);
                    existingKeys=new System.Collections.Generic.HashSet<string>(entries.Select(e=>e.key));capturedLayouts=entries.ToDictionary(e=>e.key,Capture);
                    game.BakeEditableScreens();Phase=2;
                }
                else if(Phase==2&&game&&game.CanvasBakeComplete)
                {
                    var ui=game.editableUI;
                    foreach(var e in ui.GetComponentsInChildren<TokMartCanvasElement>(true))e.ResetBindingPreview(existingKeys==null||!existingKeys.Contains(e.key));
                    // Binding deltas are transient. Save each existing authored rectangle,
                    // rather than baking the last animated control size into its next baseline.
                    foreach(var e in ui.GetComponentsInChildren<TokMartCanvasElement>(true))if(capturedLayouts!=null&&capturedLayouts.TryGetValue(e.key,out var layout))Restore(e,layout);
                    if(File.Exists("Temp/OriginalCanvasLayout.json"))
                    {
                        var originals=JsonUtility.FromJson<LayoutFile>(File.ReadAllText("Temp/OriginalCanvasLayout.json")).entries.ToDictionary(e=>e.key);
                        foreach(var e in ui.GetComponentsInChildren<TokMartCanvasElement>(true))
                        {
                            if(originals.TryGetValue(e.key,out var layout))Restore(e,layout);
                            else {e.ResetBindingPreview();var r=e.GetComponent<RectTransform>();r.pivot=new Vector2(0,1);if(!e.hasLayout){r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(1600,900);}r.localScale=Vector3.one;}
                            var motion=e.GetComponent<TokMartButtonMotion>();if(motion)motion.centeredLayout=false;
                        }
                        File.Copy("Temp/OriginalCanvasLayout.json","UI-Backups/OriginalCanvasLayout-0.4.0.json",true);File.Delete("Temp/OriginalCanvasLayout.json");
                    }
                    CanvasMigrationTools.PersistArtwork(ui);InstallMotion(ui);PrefabUtility.SaveAsPrefabAsset(ui.gameObject,Prefab);
                    Phase=3;EditorApplication.isPlaying=false;
                }
                else if(Phase==3&&!EditorApplication.isPlaying&&!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    // Existing scene/prefab layout and unrelated controls are retained.
                    AuthoredUIProjectTools.Prepare();AssetDatabase.SaveAssets();Phase=0;
                    Directory.CreateDirectory("Verification/Release05");File.WriteAllText("Verification/Release05/ui-authoring.txt","Updated screens and motion saved in authored Canvas; existing controls retained.");
                }
            }
            catch(Exception ex){Phase=0;Debug.LogException(ex);Directory.CreateDirectory("Verification/Release05");File.WriteAllText("Verification/Release05/ui-error.txt",ex.ToString());}
        }
        static void Restore(TokMartCanvasElement element,LayoutEntry layout)
        {var r=element.GetComponent<RectTransform>();r.pivot=new Vector2(layout.px,layout.py);r.anchoredPosition=new Vector2(layout.x,layout.y);r.sizeDelta=new Vector2(layout.w,layout.h);r.localScale=Vector3.one;}
        static void InstallMotion(TokMartCanvas ui)
        {
            var source=ui.GetComponent<AudioSource>();if(!source)source=ui.gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.clip=Resources.Load<AudioClip>("Audio/Success");
            ui.starAudio=source;
            foreach(var button in ui.GetComponentsInChildren<Button>(true))
            {
                var parent=button.transform.parent;var group=parent.GetComponent<TokMartCanvasElement>();
                if(!group||group.kind!="Group"||!parent.name.StartsWith("UiButton:"))continue;
                var motion=button.GetComponent<TokMartButtonMotion>();if(!motion)motion=button.gameObject.AddComponent<TokMartButtonMotion>();
                var rt=parent.GetComponent<RectTransform>();
                if(rt.pivot==new Vector2(0,1))
                {
                    var hit=button.GetComponent<RectTransform>();var origin=hit.anchoredPosition;var center=origin+new Vector2(hit.sizeDelta.x/2,-hit.sizeDelta.y/2);
                    rt.anchoredPosition=center;rt.sizeDelta=hit.sizeDelta;rt.pivot=new Vector2(.5f,.5f);
                    foreach(RectTransform child in parent)child.anchoredPosition-=origin;
                }
                else if(!motion.centeredLayout)foreach(RectTransform child in parent)child.anchoredPosition+=new Vector2(rt.sizeDelta.x/2,-rt.sizeDelta.y/2);
                motion.centeredLayout=true;
                motion.target=rt;
            }
            int starIndex=0;
            foreach(var raw in ui.GetComponentsInChildren<RawImage>(true))
            {
                var e=raw.GetComponent<TokMartCanvasElement>();if(!e||!e.key.StartsWith("ShiftResults_0/ResultStars_0/"))continue;
                var burst=raw.GetComponent<TokMartStarBurst>();if(!burst)burst=raw.gameObject.AddComponent<TokMartStarBurst>();burst.star=raw;burst.delay=.18f+starIndex++*.28f;
                var rt=raw.rectTransform;if(rt.pivot==new Vector2(0,1)){rt.anchoredPosition+=new Vector2(rt.sizeDelta.x/2,-rt.sizeDelta.y/2);rt.pivot=new Vector2(.5f,.5f);}
                if(burst.sparks==null||burst.sparks.Length==0)
                {
                    burst.sparks=new RectTransform[8];
                    for(int i=0;i<8;i++)
                    {var spark=new GameObject("Star spark "+i,typeof(RectTransform),typeof(Image));var r=spark.GetComponent<RectTransform>();r.SetParent(rt,false);r.sizeDelta=new Vector2(9,18);r.localRotation=Quaternion.Euler(0,0,45+i*45);var im=spark.GetComponent<Image>();im.color=i%2==0?new Color(1,.68f,.08f):new Color(.12f,.7f,.54f);im.raycastTarget=false;burst.sparks[i]=r;}
                }
            }
        }
    }
}
