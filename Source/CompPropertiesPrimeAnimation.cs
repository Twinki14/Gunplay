using System.Collections.Generic;
using Verse;

namespace Gunplay
{
    public class CompPropertiesPrimeAnimation : CompProperties
    {
        public CompPropertiesPrimeAnimation() { compClass = typeof(CompPrimeAnimation); }

        public List<GraphicData> frames;
        public int ticksToIdle = 0;
    }
}
