using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ChunkWalls
{
	[StaticConstructorOnStartup]
	public static class MyStaticConstructor
	{
		static MyStaticConstructor()
		{
			List<ThingDef> impliedDefs = new List<ThingDef>();
			StringBuilder stringBuilder = new StringBuilder("[LuluChunkWalls] Dynamic patched the following defs: ");
			bool first = true;
			float wallWork = ThingDefOf.Wall.statBases.GetStatValueFromList(StatDefOf.WorkToBuild, 135);
			float floorWork = TerrainDefOf.Concrete.statBases.GetStatValueFromList(StatDefOf.WorkToBuild, 100);

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

						StuffProperties productProps = (chunkDef.butcherProducts?.Where(p => p.thingDef.IsStuff).FirstOrDefault()?.thingDef ?? ThingDefOf.BlocksGranite).stuffProps;
						float stuffWork = productProps.statOffsets.GetStatOffsetFromList(StatDefOf.WorkToBuild);
						float stuffFactor = productProps.statFactors.GetStatFactorFromList(StatDefOf.WorkToBuild);

						// Patch the def.
						thingDef.building.blueprintGraphicData.texPath = "Lulu/ChunkWalls/ChunkWalls_Blueprint_Atlas";
						thingDef.constructionSkillPrerequisite = 6;
						thingDef.costList.Add(new ThingDefCountClass(chunkDef, 2));
						thingDef.designationCategory = DesignationCategoryDefOf.Structure;
						thingDef.designatorDropdown = MyDefOf.LuluChunkWalls_NaturalWall;
						thingDef.placingDraggableDimensions = 1;
						thingDef.researchPrerequisites.Add(MyDefOf.Stonecutting);
						thingDef.statBases.Add(new StatModifier { stat = StatDefOf.WorkToBuild, value = (wallWork+stuffWork)*(stuffFactor+3) }); // was 2430
						thingDef.terrainAffordanceNeeded = TerrainAffordanceDefOf.Heavy;
						thingDef.uiIconPath = "Lulu/ChunkWalls/ChunkWalls_MenuIcon";

						// Manually create implied defs (we're past the point where they'd be generated automatically).
						impliedDefs.Add((ThingDef)typeof(ThingDefGenerator_Buildings).GetMethod("NewBlueprintDef_Thing", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { thingDef, false, null }));
						impliedDefs.Add((ThingDef)typeof(ThingDefGenerator_Buildings).GetMethod("NewFrameDef_Thing",     BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { thingDef }));

						// Finalize graphic data.
						thingDef.ResolveReferences();
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

						TerrainDef terrainDef = thingDef.building.leaveTerrain;

						if (terrainDef != null)
						{
							if (terrainDef.costList == null)              terrainDef.costList              = new List<ThingDefCountClass>();
							if (terrainDef.researchPrerequisites == null) terrainDef.researchPrerequisites = new List<ResearchProjectDef>();
							if (terrainDef.statBases == null)             terrainDef.statBases             = new List<StatModifier>();
							terrainDef.constructionSkillPrerequisite = 6;
							terrainDef.costList.Add(new ThingDefCountClass(chunkDef, 1));
							terrainDef.designationCategory = MyDefOf.Floors;
							terrainDef.designatorDropdown = MyDefOf.LuluChunkWalls_NaturalFloor;
							terrainDef.researchPrerequisites.Add(MyDefOf.Stonecutting);
							terrainDef.statBases.Add(new StatModifier { stat = StatDefOf.WorkToBuild, value = (floorWork+stuffWork)*(stuffFactor+3) }); // was 1500
							terrainDef.terrainAffordanceNeeded = TerrainAffordanceDefOf.Heavy;
							impliedDefs.Add((ThingDef)typeof(ThingDefGenerator_Buildings).GetMethod("NewBlueprintDef_Terrain", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { terrainDef }));
							impliedDefs.Add((ThingDef)typeof(ThingDefGenerator_Buildings).GetMethod("NewFrameDef_Terrain",	  BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { terrainDef }));
							stringBuilder.AppendWithComma(terrainDef.defName);
						}
					}
				}
			}

			// Add the created implied defs to the def database.
			foreach (ThingDef impliedDef in impliedDefs)
			{
				DefGenerator.AddImpliedDef(impliedDef);
				impliedDef.ResolveReferences();
				typeof(ShortHashGiver).GetMethod("GiveShortHash", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { impliedDef, typeof(ThingDef) });
			}

			// Regenerate the references in designation categories to make sure everything shows up to the player.
			DesignationCategoryDefOf.Structure.ResolveReferences();
			MyDefOf.Floors.ResolveReferences();

			// Report on what we've done.
			Log.Message(stringBuilder.ToString());
		}
	}
}
