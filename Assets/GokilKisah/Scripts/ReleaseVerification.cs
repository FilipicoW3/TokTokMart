using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GokilKisah
{
    public sealed partial class StoreGameController
    {
        public void BeginReleaseVerification(string folder)=>StartCoroutine(VerifyRelease(folder));
        IEnumerator VerifyRelease(string folder)
        {
            Directory.CreateDirectory(folder);var checks=new List<string>();
            Action<bool,string> check=(ok,label)=>checks.Add((ok?"PASS ":"FAIL ")+label);
            var rng=UnityEngine.Random.state;bool oldTouch=showTouchControls;bool oldHaptics=MobileHaptics.Enabled,oldFocus=pauseOnFocusLoss;
            testing=verificationRunning=true;settingsOpen=false;pauseOnFocusLoss=false;
            try
            {
                for(int level=1;level<=40;level++)
                {
                    var t=new ShiftTargets(level);
                    check(t.serve>0&&t.stock>0&&t.clean>0&&t.duration>t.workloadSeconds,"Shift "+level+" has bounded, timed goals ("+t.Title+")");
                }
                var rush=new ShiftTargets(2);var stock=new ShiftTargets(3);var mop=new ShiftTargets(4);var chaosShift=new ShiftTargets(5);
                check(rush.serve>stock.serve&&rush.ArrivalScale<stock.ArrivalScale&&rush.duration<stock.duration,"Rush emphasizes customers and a tighter clock");
                check(stock.stock>rush.stock&&mop.clean>stock.clean&&chaosShift.focus==ShiftFocus.Chaos,"Restock, mop and chaos profiles differ");
                // Exercise the real four-stroke completion, then allow several Update/LateUpdate
                // cycles to catch the old stale-view overwrite of the opening spill.
                foreach(int target in new[]{0,1})
                {
                    BeginCampaignLevel(4,false);shiftBriefOpen=false;SyncSpillViews();
                    var opening=spillViews[0];var untouched=spills[1-target];var cleaned=spillViews[target];var position=spills[target];
                    player=position;Interact();check(skillType==2&&skillTarget==target,"Select exact mop hazard "+target);
                    for(int stroke=0;stroke<4;stroke++){taskLastAttempt=-1;skillClock=SkillZoneCenter()/((.75f+.08f*taskCombo)*TaskDifficulty);AttemptSkill();}
                    yield return null;yield return null;yield return null;
                    check(!cleaned||!cleaned.gameObject.activeSelf,"Cleaned hazard visual is retired "+target);
                    check(spills.Contains(untouched)&&!spills.Contains(position)&&hazardsCleared==1,"Only the selected spill is removed after frame synchronization "+target);
                    if(target==0)check(!opening.gameObject.activeSelf,"Initial authored puddle stays removed");
                    ResetRun(Encounter.Store);yield return null;check(authoredSpills.All(s=>s.gameObject.activeSelf)&&spills.Count==initialSpillPositions.Count,"Retry restores opening hazards "+target);
                }
                BeginCampaignLevel(4,false);shiftBriefOpen=false;SyncSpillViews();
                Vector2 manuallyMoved=spills[0]+Vector2.right*10;spillViews[0].transform.position=ShopLayout.ToWorld(manuallyMoved);ReadManualSceneEdits();
                check(spills[0]==manuallyMoved,"Intentional scene hazard edits still work");
                for(int i=0;i<12&&hazardsCleared<CleanGoal;i++){SyncSpillViews();ClearSpill(0);hazardsCleared++;EnsureShiftCleanupWork();yield return null;}
                check(hazardsCleared==CleanGoal,"Mop profile keeps required cleanup reachable and available");
                foreach(string skin in customerSkins)
                {
                    check(GameArtLibrary.Idle(skin),skin+" supplied customer art is loadable");
                    check(GameArtLibrary.CustomerPose(skin,true,0),skin+" walk pose is loadable");
                }
                foreach(string sign in new[]{"StartButton","SettingsButton","LevelsButton","MenuPole"})check(GameArtLibrary.UI(sign),sign+" menu art is loadable");
                MobileHaptics.Enabled=false;check(!MobileHaptics.Enabled,"Vibration can be switched off");MobileHaptics.Pulse();MobileHaptics.Enabled=oldHaptics;
                ReturnToMainMenu();yield return NovemberShot(folder,"01-menu");
                int count=editableUI.GetComponentsInChildren<Transform>(true).Length;
                var settings=editableUI.GetComponentsInChildren<Button>().FirstOrDefault(b=>b.GetComponent<TokMartCanvasElement>().key.StartsWith("ArtMenu_0/Settings_0/"));
                check(settings&&settings.GetComponent<TokMartButtonMotion>(),"Supplied settings sign has native button and motion");
                if(settings)
                {
                    var motion=settings.GetComponent<TokMartButtonMotion>();var pointer=new PointerEventData(EventSystem.current);
                    ExecuteEvents.Execute(settings.gameObject,pointer,ExecuteEvents.pointerEnterHandler);yield return new WaitForSecondsRealtime(.2f);
                    check(motion.target.localScale.x>1.02f,"Hover enlarges the supplied sign");
                    ExecuteEvents.Execute(settings.gameObject,pointer,ExecuteEvents.pointerDownHandler);yield return new WaitForSecondsRealtime(.2f);
                    check(motion.target.localScale.x<.97f,"Press compresses the supplied sign");
                    ExecuteEvents.Execute(settings.gameObject,pointer,ExecuteEvents.pointerUpHandler);yield return new WaitForSecondsRealtime(.06f);
                    check(motion.target.localScale.x>1,"Release adds a visible button pop");
                    ExecuteEvents.Execute(settings.gameObject,pointer,ExecuteEvents.pointerExitHandler);
                    yield return new WaitForSecondsRealtime(.4f);
                }
                if(settings)settings.onClick.Invoke();yield return null;yield return new WaitForEndOfFrame();yield return null;
                check(settingsOpen,"Supplied settings sign opens settings");settingsTab=2;yield return NovemberShot(folder,"02-vibration-setting");settingsOpen=false;
                OpenLevelMap();yield return NovemberShot(folder,"03-level-select");levelMapOpen=false;
                BeginCampaignLevel(3,false);shiftBriefOpen=true;yield return NovemberShot(folder,"04-brief");shiftBriefOpen=false;
                showTouchControls=true;yield return NovemberShot(folder,"05-mobile-hud");
                var queued=SpawnCustomer();queued.state=ShopperState.Queuing;queued.position=QueuePoint(0);customerQueue.Add(queued);
                foreach(bool touch in new[]{true,false})foreach(Encounter active in new[]{Encounter.Store,Encounter.Goblins,Encounter.Bo})
                {showTouchControls=touch;encounter=active;yield return null;yield return new WaitForEndOfFrame();}
                encounter=Encounter.Store;ResetCustomers();
                served=ServeGoal;stockActions=StockGoal;hazardsCleared=CleanGoal;shiftTime=0;chaos=0;FinishCampaignLevel(true,"");
                yield return NovemberShot(folder,"06-star-pop");
                yield return new WaitForSecondsRealtime(1.4f);yield return NovemberShot(folder,"07-results");
                check(editableUI.GetComponentsInChildren<TokMartStarBurst>().Length==3,"Three result stars have authored pop and burst effects");
                check(editableUI.MissingBindingCount==0,"No missing authored UI bindings across release screens");
                check(editableUI.GetComponentsInChildren<Transform>(true).Length==count,"Screen changes create no runtime UI objects");
            }
            finally
            {
                MobileHaptics.Enabled=oldHaptics;UnityEngine.Random.state=rng;showTouchControls=oldTouch;pauseOnFocusLoss=oldFocus;testing=verificationRunning=false;ReturnToMainMenu();
                File.WriteAllText(Path.Combine(folder,"regression.txt"),string.Join("\n",checks)+"\n\n"+checks.Count+" checks; "+checks.Count(x=>x.StartsWith("FAIL"))+" failures.\n");
            }
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--quit-after-verification")>=0)Application.Quit(checks.Any(x=>x.StartsWith("FAIL"))?1:0);
        }
        public void BeginReleasePlaytest(string folder)=>StartCoroutine(ReleasePlaytest(folder));
        IEnumerator ReleasePlaytest(string folder)
        {
            Directory.CreateDirectory(folder);var results=new List<string>();var rng=UnityEngine.Random.state;
            bool oldFocus=pauseOnFocusLoss,oldTouch=showTouchControls;pauseOnFocusLoss=false;showTouchControls=true;
            testing=verificationRunning=false;settingsOpen=false;Time.captureFramerate=30;
            foreach(int level in new[]{1,2,3,4,5})
            {
                BeginCampaignLevel(level,false);UnlockedBossMask=0;randomFights=2;shiftBriefOpen=false;
                if(level==5){encounterRollClock=0;TickCampaign(0,true);yield return PlayActiveBoss(Encounter.Goblins);while(bossActive&&!levelResults)yield return null;}
                int guard=0;
                while(stockActions<StockGoal&&!levelResults&&guard++<20)
                {
                    yield return ReplayRestock();
                }
                guard=0;while(hazardsCleared<CleanGoal&&!levelResults&&spills.Count>0&&guard++<20){yield return Go(spills[0]);Interact();if(skillType>=0)yield return CompleteSkill();}
                if(Targets.focus==ShiftFocus.Rush&&!levelResults)
                {
                    yield return Go(ShopLayout.ToGame(shop.register.approachPoint.position));
                    while(!levelResults&&shiftTime<ShiftBudget+1)
                    {if(CustomerAtRegister()!=null){Interact();if(skillType==1)yield return CompleteSkill();}yield return null;}
                }
                else while(!levelResults&&shiftTime<ShiftBudget+1)yield return null;
                results.Add((levelResults&&levelWon?"PASS ":"FAIL ")+"Shift "+level+" "+Targets.Title+": "+served+"/"+ServeGoal+" served, "+stockActions+"/"+StockGoal+" restocks, "+hazardsCleared+"/"+CleanGoal+" cleanups; "+shiftTime.ToString("F1")+"/"+ShiftBudget+" sec; "+invoiceStars+" stars; chaos "+chaos.ToString("F1")+"; missing UI "+editableUI.MissingBindingCount+"; "+levelFailure);
                yield return NovemberShot(folder,"shift-"+level);
                File.WriteAllLines(Path.Combine(folder,"playthrough.txt"),results);
            }
            Time.captureFramerate=0;UnityEngine.Random.state=rng;pauseOnFocusLoss=oldFocus;showTouchControls=oldTouch;ReturnToMainMenu();Debug.Log(string.Join("\n",results));
        }
    }
}
