using UnityEngine;

namespace GokilKisah
{
    public enum ShiftFocus { Warmup, Rush, Restock, Cleanup, Chaos }
    public readonly struct ShiftTargets
    {
        public readonly int serve,stock,clean;
        public readonly float workloadSeconds,duration;
        public readonly ShiftFocus focus;
        public string Title=>focus==ShiftFocus.Rush?"LUNCH RUSH":focus==ShiftFocus.Restock?"STOCK UP":focus==ShiftFocus.Cleanup?"MOP UP":focus==ShiftFocus.Chaos?"CHAOS SHIFT":"WELCOME SHIFT";
        public string Hint=>focus==ShiftFocus.Rush?"Beat the clock. Keep the checkout moving!":focus==ShiftFocus.Restock?"Grab crates and refill shelves. Your partner covers the till.":focus==ShiftFocus.Cleanup?"Clear the puddles and open up the aisles.":focus==ShiftFocus.Chaos?"Handle the surprise guest and keep chaos below 100%.":"Stock, serve and clean. Find your rhythm!";
        public float ArrivalScale=>focus==ShiftFocus.Rush?.72f:focus==ShiftFocus.Cleanup?1.25f:focus==ShiftFocus.Restock?1.1f:.95f;
        public int SpillEvery=>focus==ShiftFocus.Cleanup?2:focus==ShiftFocus.Chaos?3:focus==ShiftFocus.Rush?8:6;
        public ShiftTargets(int shift)
        {
            int level=Mathf.Clamp(shift,1,10000),tier=Mathf.Min(8,(level-1)/5);
            focus=level==1?ShiftFocus.Warmup:level%5==0?ShiftFocus.Chaos:(ShiftFocus)(1+(level-2)%4);
            serve=3+tier;stock=1+tier/2;clean=1+tier/3;
            switch(focus)
            {
                case ShiftFocus.Rush:serve=5+tier;break;
                case ShiftFocus.Restock:serve=3+tier/2;stock=3+tier;break;
                case ShiftFocus.Cleanup:serve=3+tier/2;clean=4+tier;break;
                case ShiftFocus.Chaos:serve=4+tier;stock=2+tier/2;clean=2+tier/2;break;
            }
            workloadSeconds=serve*8+stock*16+clean*12;
            duration=(focus==ShiftFocus.Rush?85:focus==ShiftFocus.Chaos?115:105)+workloadSeconds;
            if(focus==ShiftFocus.Warmup)duration=172;
        }
    }
}
