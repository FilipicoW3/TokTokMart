using System.Collections;
using UnityEngine;

namespace GokilKisah
{
    public sealed partial class StoreGameController
    {
        IEnumerator MoveNearBossPoint(Vector2 point,float radius=48)
        {
            float guard=18;
            while(Vector2.Distance(player,point)>radius&&guard>0&&!gameOver)
            {player=Navigate(player,SafeBossPoint(point),tuning.moveSpeed*Time.deltaTime);guard-=Time.deltaTime;yield return null;}
        }
        IEnumerator PlayActiveBoss(Encounter next)
        {
            float endTime=Time.time+140;
            if(next==Encounter.Goblins)
            {
                yield return MoveNearBossPoint(phone,65);Interact();
                while(!resolved&&!gameOver&&Time.time<endTime)
                {
                    int target=-1;float best=float.MaxValue;
                    for(int i=0;i<GoblinTotal;i++)if(goblinHealth[i]>0){float d=Vector2.Distance(player,goblinPositions[i]);if(d<best){best=d;target=i;}}
                    if(target>=0)
                    {
                        if(best>70)player=Navigate(player,goblinPositions[target],tuning.moveSpeed*Time.deltaTime);
                        else if(goblinStun[target]<2&&shooCooldown<=0)Interact();
                    }
                    yield return null;
                }
            }
            else if(next==Encounter.KKung)
            {
                while(!resolved&&!gameOver&&Time.time<endTime)
                {yield return MoveNearBossPoint(lightSwitch,58);Interact();yield return new WaitForSeconds(.15f);}
            }
            else if(next==Encounter.Salmah)
            {
                yield return MoveNearBossPoint(nailGun,60);Interact();
                while(!resolved&&!gameOver&&Time.time<endTime)
                {
                    if(salmahWindup>0||salmahDash>0||salmahRecovery>0||salmahStun>0)
                    {
                        player=Navigate(player,SafeBossPoint(salmah-salmahFacing*90),tuning.moveSpeed*Time.deltaTime);
                        if(CanHitSalmah()&&nailCooldown<=0)Interact();
                    }
                    else if(Vector2.Distance(player,salmah)>140)player=Navigate(player,salmah,tuning.moveSpeed*Time.deltaTime);
                    yield return null;
                }
            }
            else if(next==Encounter.Bo)
            {
                while(!resolved&&!gameOver&&Time.time<endTime)
                {
                    if(!canFlying){yield return MoveNearBossPoint(can,55);if(boSpawnDelay<=0)Interact();}
                    yield return null;
                }
            }
            if(Time.time>=endTime)Debug.LogError("Active boss replay timed out: "+next);
        }
    }
}
