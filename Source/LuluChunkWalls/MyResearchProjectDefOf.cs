using RimWorld;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ChunkWalls
{
    [DefOf]
    public static class MyResearchProjectDefOf
    {
        static MyResearchProjectDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ResearchProjectDef));
        }
        
        public static ResearchProjectDef Stonecutting;
    }
}
