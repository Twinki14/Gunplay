using UnityEngine;
using Verse;

namespace Gunplay
{
    public class GunplaySettings : ModSettings
    {
        public bool EnableTrails = true;
        public bool EnableSounds = true;
        public bool EnableWeaponAnimations = true;
        public bool EnableEffects = true;
        public float ProjectileSpeed = 3f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref EnableTrails, "enableTrails");
            Scribe_Values.Look(ref EnableSounds, "enenableSoundsableTrails");
            Scribe_Values.Look(ref EnableWeaponAnimations, "enableWeaponAnimations");
            Scribe_Values.Look(ref EnableEffects, "enableEffects");
            Scribe_Values.Look(ref ProjectileSpeed, "projectileSpeed");
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("GunplayEnableTrailsName".Translate(), ref EnableTrails, "GunplayEnableTrailsDesc".Translate());
            listingStandard.CheckboxLabeled("GunplayEnableSoundsName".Translate(), ref EnableSounds, "GunplayEnableSoundsDesc".Translate());
            listingStandard.CheckboxLabeled("GunplayEnableWeaponAnimationsName".Translate(), ref EnableWeaponAnimations, "GunplayEnableWeaponAnimationsDesc".Translate());
            listingStandard.CheckboxLabeled("GunplayEnableEffectsName".Translate(), ref EnableEffects, "GunplayEnableEffectsDesc".Translate());
            listingStandard.SliderLabeled("GunplayProjectileSpeedName".Translate(), ref ProjectileSpeed, "GunplayProjectileSpeedDesc".Translate(), 0.1f, 10, ProjectileSpeed.ToStringPercent());
            listingStandard.End();
        }
    }
}
