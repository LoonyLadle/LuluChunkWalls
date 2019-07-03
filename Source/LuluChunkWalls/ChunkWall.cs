using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ChunkWalls
{
    public class ChunkWall : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            ThingDef wallDef = DefDatabase<ThingDef>.AllDefsListForReading.Find(d => d.building?.mineableThing == this.Stuff);

            if (wallDef != null)
            {
                Map storedMap = this.Map;
                GenSpawn.WipeExistingThings(this.Position, this.Rotation, wallDef, this.Map, DestroyMode.Vanish);
                GenSpawn.Spawn(wallDef, this.Position, storedMap);
            }
            else
            {
                Messages.Message("Could not build a natural stone wall out of " + this.Stuff.label + ".", MessageTypeDefOf.NeutralEvent, false);
                GenSpawn.Refund(this, this.Map, CellRect.Empty);
            }
        }
    }
}
