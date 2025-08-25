using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Gunplay
{
    public enum MaterialKind
    {
        None = 0,
        Fabric = 1,
        Flesh = 2,
        Metal = 3,
        Soil = 4,
        Stone = 5,
        Wood = 6,
    }

    public static class MaterialKindGetter
    {
        static Dictionary<Def, MaterialKind> map = new Dictionary<Def, MaterialKind>();

        static MaterialKindGetter()
        {
            foreach (var thingDef in from def in DefDatabase<ThingDef>.AllDefs where def.building != null select def)
            {
                var kind = Get(thingDef);
                if (kind == MaterialKind.None)
                {
                    continue;
                }

                if (thingDef.building.naturalTerrain != null)
                {
                    map[thingDef.building.naturalTerrain] = kind;
                    if (thingDef.building.naturalTerrain.smoothedTerrain != null) map[thingDef.building.naturalTerrain.smoothedTerrain] = kind;
                }

                if (thingDef.building.leaveTerrain != null)
                {
                    map[thingDef.building.leaveTerrain] = kind;
                    if (thingDef.building.leaveTerrain.smoothedTerrain != null) map[thingDef.building.leaveTerrain.smoothedTerrain] = kind;
                }
            }
        }


        private static MaterialKind Get(List<ThingDefCountClass> list)
        {
            if (list == null) return MaterialKind.None;

            foreach (var td in list)
            {
                var kind = Get(td.thingDef);
                if (kind != MaterialKind.None) return kind;
            }

            return MaterialKind.None;
        }

        private static MaterialKind GetWithoutCache(ThingDef def)
        {
            MaterialKind kind;

            if (def.race != null)
            {
                return def.race.FleshType == FleshTypeDefOf.Mechanoid ? MaterialKind.Metal : MaterialKind.Flesh;
            }

            if (def.stuffProps != null && def.stuffProps.categories != null && def.stuffProps.categories.Count > 0)
            {
                var cat = def.stuffProps.categories[0];
                if (cat == StuffCategoryDefOf.Metallic) return MaterialKind.Metal;
                if (cat == StuffCategoryDefOf.Fabric) return MaterialKind.Fabric;
                if (cat == StuffCategoryDefOf.Leathery) return MaterialKind.Fabric;
                if (cat == StuffCategoryDefOf.Stony) return MaterialKind.Stone;
                if (cat == StuffCategoryDefOf.Woody) return MaterialKind.Wood;
            }

            if ((kind = Get(def.costList)) != MaterialKind.None) return kind;

            if (def.building != null && (kind = Get(def.building.mineableThing)) != MaterialKind.None) return kind;

            if ((kind = Get(def.butcherProducts)) != MaterialKind.None) return kind;

            if ((kind = Get(def.smeltProducts)) != MaterialKind.None) return kind;

            if (def.plant != null && (kind = Get(def.plant.harvestedThingDef)) != MaterialKind.None) return kind;

            return MaterialKind.None;
        }

        private static MaterialKind GetWithoutCache(BuildableDef def)
        {
            MaterialKind kind;

            if (def == TerrainDefOf.Concrete) return MaterialKind.Stone;

            if ((kind = Get(def.costList)) != MaterialKind.None) return kind;

            return def is TerrainDef ? MaterialKind.Soil : MaterialKind.None;
        }


        public static MaterialKind Get(Thing thing)
        {
            var kind = Get(thing.Stuff);
            if (kind != MaterialKind.None) return kind;

            return Get(thing.def);
        }

        public static MaterialKind Get(Def def)
        {
            if (def == null)
            {
                return MaterialKind.None;
            }

            if (map.TryGetValue(def, out var kind))
            {
                return kind;
            }

            map[def] = MaterialKind.None;

            switch (def)
            {
                case ThingDef thingDef:
                    kind = GetWithoutCache(thingDef);
                    break;
                case BuildableDef buildableDef:
                    kind = GetWithoutCache(buildableDef);
                    break;
            }

            map[def] = kind;
            return kind;
        }
    }
}
