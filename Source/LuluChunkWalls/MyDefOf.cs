using RimWorld;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ChunkWalls
{
   [DefOf]
   public static class MyDefOf
   {
      static MyDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MyDefOf));
      public static DesignatorDropdownGroupDef LuluChunkWalls_NaturalFloor;
      public static DesignatorDropdownGroupDef LuluChunkWalls_NaturalWall;
      public static ResearchProjectDef Stonecutting;
   }
}
