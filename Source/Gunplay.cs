using UnityEngine;
using Verse;

namespace Gunplay
{
    public class Gunplay : Mod
    {
        public static GunplaySettings settings;

        public Gunplay(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<GunplaySettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "GunplayTitle".Translate();
        }
    }
}
