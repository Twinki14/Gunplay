using Verse;

namespace Gunplay
{
    public class ProjectileImpactSoundDef : Def
    {
        public SoundDef fallback;
        public SoundDef fabric;
        public SoundDef flesh;
        public SoundDef metal;
        public SoundDef soil;
        public SoundDef stone;
        public SoundDef wood;

        public SoundDef Effect(MaterialKind kind)
        {
            SoundDef res;

            switch (kind)
            {
                case MaterialKind.None: res = fallback; break;
                case MaterialKind.Fabric: res = fabric; break;
                case MaterialKind.Flesh: res = flesh; break;
                case MaterialKind.Metal: res = metal; break;
                case MaterialKind.Soil: res = soil; break;
                case MaterialKind.Stone: res = stone; break;
                case MaterialKind.Wood: res = wood; break;
                default: res = null; break;
            }

            return res ?? fallback;
        }
    }
}
