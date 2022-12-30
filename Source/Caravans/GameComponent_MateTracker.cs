using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater.Caravans
{
    public class GameComponent_MateTracker : GameComponent
    {
        public Dictionary<Caravan, Caravan_MateTracker> TrackerLinks = new Dictionary<Caravan, Caravan_MateTracker>();
        public HashSet<Caravan> Caravans = new HashSet<Caravan>();
        public HashSet<Caravan_MateTracker> MateTrackers = new HashSet<Caravan_MateTracker>();

        public override void ExposeData()
        {
            base.ExposeData();
            // Save dictionary entries.
        }
    }
}
