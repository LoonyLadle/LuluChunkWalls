using RimWorld;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ChunkWalls
{
    public class ChunkWall : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            // Going through the effort of setting ourself up seems silly since we will destroy ourself immediately,
            // but there's a lot of registration and such in this method so I am not sure it can safely be skipped.
            base.SpawnSetup(map, respawningAfterLoad);

            // Get a building def that drops our stuff when mined -- this *should* be a natural wall.
            ThingDef wallDef = DefDatabase<ThingDef>.AllDefsListForReading.Find(d => d.building?.mineableThing == this.Stuff);

            // If we found a building def, destroy and replace ourself with a different building.
            if (wallDef != null)
            {
                // Store our map instead of using it directly; it will be invalid after WipeExistingThings.
                Map storedMap = this.Map;
                GenSpawn.WipeExistingThings(this.Position, this.Rotation, wallDef, storedMap, DestroyMode.Vanish);
                GenSpawn.Spawn(wallDef, this.Position, storedMap);
            }
            // Refund ourselves if a building def was not found.
            else
            {
                Messages.Message("Could not build a natural stone wall out of " + this.Stuff.label + ".", MessageTypeDefOf.NeutralEvent, false);
                GenSpawn.Refund(this, this.Map, CellRect.Empty);
            }
        }
    }
}
