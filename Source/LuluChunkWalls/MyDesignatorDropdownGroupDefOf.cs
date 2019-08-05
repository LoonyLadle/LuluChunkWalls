using RimWorld;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ChunkWalls
{
    [DefOf]
    public static class MyDesignatorDropdownGroupDefOf
    {
        static MyDesignatorDropdownGroupDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DesignatorDropdownGroupDef));
        }

        public static DesignatorDropdownGroupDef LuluChunkWalls_NaturalWall;
    }
}
