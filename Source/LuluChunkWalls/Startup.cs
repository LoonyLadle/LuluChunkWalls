using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ChunkWalls
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool first = true;

            stringBuilder.Append("[LuluChunkWalls] Dynamic patched the following defs: ");

            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.building?.isNaturalRock ?? false)
                {
                    ThingDef chunkDef = thingDef.building.mineableThing;

                    if (chunkDef?.thingCategories?.Contains(ThingCategoryDefOf.StoneChunks) ?? false)
                    {
                        // Ensure lists are initialized.
                        if (thingDef.building.blueprintGraphicData == null) thingDef.building.blueprintGraphicData = new GraphicData();
                        if (thingDef.costList == null)                      thingDef.costList                      = new List<ThingDefCountClass>();
                        if (thingDef.researchPrerequisites == null)         thingDef.researchPrerequisites         = new List<ResearchProjectDef>();
                        if (thingDef.statBases == null)                     thingDef.statBases                     = new List<StatModifier>();

                        // Patch the def.
                        thingDef.building.blueprintGraphicData.texPath = "Lulu/ChunkWalls/ChunkWalls_Blueprint_Atlas";
                        thingDef.constructionSkillPrerequisite = 6;
                        thingDef.costList.Add(new ThingDefCountClass(chunkDef, 1));
                        thingDef.designationCategory = DesignationCategoryDefOf.Structure;
                        thingDef.designatorDropdown = MyDefOf.LuluChunkWalls_NaturalWall;
                        thingDef.researchPrerequisites.Add(MyDefOf.Stonecutting);
                        thingDef.statBases.Add(new StatModifier { stat = StatDefOf.WorkToBuild, value = 2430 });
                        thingDef.uiIconColor = thingDef.graphicData.color;
                        thingDef.uiIconPath = "Lulu/ChunkWalls/ChunkWalls_MenuIcon";

                        //typeof(ThingDef).GetMethod("ResolveIcon", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(thingDef, null);
                        //typeof(GraphicData).GetField("cachedGraphic", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(thingDef.graphicData, default(Graphic));
                        thingDef.PostLoad();

                        // Build the log string.
                        if (first)
                        {
                            stringBuilder.Append(thingDef.defName);
                            first = false;
                        }
                        else
                        {
                            stringBuilder.AppendWithComma(thingDef.defName);
                        }
                    }
                }
            }
            /*
            foreach (Type defType in GenDefDatabase.AllDefTypesWithDatabases())
            {
                // OH GOD LULU WHAT THE FUCK ARE YOU DOING
                typeof(DefDatabase<>).MakeGenericType(defType).GetMethod("ResolveAllReferences", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[]{true});
            }
            */
            DefDatabase<DesignationCategoryDef>.ResolveAllReferences();
            Log.Message(stringBuilder.ToString());
        }
    }
}
