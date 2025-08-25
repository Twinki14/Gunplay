using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Gunplay.Patch
{

    [HarmonyPatch(typeof(Projectile), "Launch", typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef))]
    public class PatchProjectileLaunch
    {
        static PropertyInfo StartingTicksToImpactProp = typeof(Projectile).GetProperty("StartingTicksToImpact", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Postfix(Projectile __instance, Vector3 ___destination, Thing launcher, ref Vector3 ___origin, LocalTargetInfo intendedTarget, Thing equipment, ref int ___ticksToImpact)
        {
            var prop = GunplaySetup.GunProp(equipment);
            if (prop is null)
            {
                return;
            }

            var comp = equipment.TryGetComp<CompGun>();
            if (comp != null)
            {
                var angle = (___destination - ___origin).AngleFlat() - (intendedTarget.CenterVector3 - ___origin).AngleFlat();
                if (angle < -180) {
                    angle += 360;
                }
                comp.RotationOffset = (angle + 180) % 360 - 180;
            }

            if (launcher is Pawn)
            {
                // This is mainly meant for grenades, which we don't want to affect the origin point of.
                // When grenades have their origin point moved, it affects the "landing" animation,
                // and "resets" the grenades animation when it reaches the target tile.
                if (__instance.def.projectile.explosionDelay <= 0)
                {
                    ___origin += (___destination - ___origin).normalized * prop.barrelLength;
                }

                ___ticksToImpact = Mathf.CeilToInt((float) StartingTicksToImpactProp.GetValue(__instance));
                if (___ticksToImpact < 1)
                {
                    ___ticksToImpact = 1;
                }
            }

            if (!Gunplay.settings.enableTrails || launcher?.Map == null)
            {
                return;
            }

            if (GenSpawn.Spawn(prop.trail, ___origin.ToIntVec3(), launcher.Map) is ProjectileTrail trail)
            {
                trail.Initialize(__instance, ___destination);
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), "get_StartingTicksToImpact")]
    public class PatchProjectileStartingTicksToImpact
    {
        public static float Postfix(float value, Projectile __instance)
        {
            var prop = GunplaySetup.GunProp(__instance.EquipmentDef);
            if (prop == null || prop.preserveSpeed) return value;

            return value / Gunplay.settings.projectileSpeed;
        }
    }

    [HarmonyPatch(typeof(Projectile), "Impact")]
    public class PatchProjectileImpact
    {
        public static void Prefix(Projectile __instance, Thing hitThing, Vector3 ___origin)
        {
            var map = __instance.Map;
            if (map == null) return;

            var prop = GunplaySetup.GunProp(__instance.EquipmentDef);
            if (prop == null) return;

            var kind = MaterialKind.None;

            if (hitThing != null)
            {
                kind = MaterialKindGetter.Get(hitThing);
            }

            if(kind == MaterialKind.None)
            {
                var terrainDef = map.terrainGrid.TerrainAt(CellIndicesUtility.CellToIndex(__instance.Position, map.Size.x));
                kind = MaterialKindGetter.Get(terrainDef);
            }

            if (Gunplay.settings.enableSounds) {
                var sound = prop.projectileImpactSound == null ? null : prop.projectileImpactSound.Effect(kind);
                if (sound != null) sound.PlayOneShot(new TargetInfo(__instance.Position, map, false));
            }

            if (Gunplay.settings.enableEffects)
            {
                var effecterDef = prop.projectileImpactEffect?.Effect(kind);
                if (effecterDef != null)
                {
                    var effecter = new Effecter(effecterDef);
                    effecter.Trigger(__instance, new TargetInfo(___origin.ToIntVec3(), __instance.Map));
                    effecter.Cleanup();
                }
            }
        }
    }


}
