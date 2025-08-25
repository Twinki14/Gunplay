using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Gunplay
{
    [StaticConstructorOnStartup]
    public class GunplaySetup
    {
        static Dictionary<ThingDef, GunPropDef> propMap = new Dictionary<ThingDef, GunPropDef>();
        static GunPropDef defaultDef = DefDatabase<GunPropDef>.GetNamed("default");

        static GunplaySetup()
        {
            var harmony = new Harmony("com.github.automatic1111.gunplay");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            foreach (var def in DefDatabase<ProjectileTrailDef>.AllDefs)
            {
                if (!def.DrawMatSingle || !def.DrawMatSingle.mainTexture) continue;

                def.DrawMatSingle.mainTexture.wrapMode = TextureWrapMode.Clamp;
            }

            foreach (var def in DefDatabase<GunPropDef>.AllDefs)
            {
                ThingDef target = null;

                if (def.defTarget != null) target = DefDatabase<ThingDef>.GetNamed(def.defTarget, false);
                if (target == null) target = DefDatabase<ThingDef>.GetNamed(def.defName, false);
                if (target != null) propMap[target] = def;

                if (def.trail == null) def.trail = defaultDef.trail;
                if (def.barrelLength == -1) def.barrelLength = defaultDef.barrelLength;

                if (Gunplay.settings.EnableSounds)
                {
                    if (def.projectileImpactSound == null) def.projectileImpactSound = defaultDef.projectileImpactSound;
                    if (def.projectileImpactEffect == null) def.projectileImpactEffect = defaultDef.projectileImpactEffect;
                }
            }

            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                var shoot = def.Verbs.FirstOrDefault(v => typeof(Verb_Shoot).IsAssignableFrom(v.verbClass));
                if (shoot == null) continue;
                propMap.TryAdd(def, defaultDef);

                def.comps.Add(new CompProperties() { compClass = typeof(CompGun) });

                var prop = propMap.TryGetValue(def);
                if (prop != null)
                {
                    if (prop.soundAiming != null) shoot.soundAiming = prop.soundAiming;
                    if (prop.soundCast != null) shoot.soundCast = prop.soundCast;
                    if (prop.spinner != null) def.comps.Add(prop.spinner);
                    if (prop.primer != null) def.comps.Add(prop.primer);
                    if (prop.bow != null) def.comps.Add(prop.bow);
                }
            }
        }

        public static GunPropDef GunProp(ThingDef equipment) => equipment == null ? null : propMap.TryGetValue(equipment, null);

        public static GunPropDef GunProp(Thing equipment) => equipment?.def == null ? null : propMap.TryGetValue(equipment.def, defaultDef);
    }
}
