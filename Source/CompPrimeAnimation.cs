using UnityEngine;
using Verse;

namespace Gunplay
{
    public class CompPrimeAnimation : CompPrimer
    {
        private new CompPropertiesPrimeAnimation props => base.props as CompPropertiesPrimeAnimation;

        public override int TicksToIdle => props.ticksToIdle;

        public override Graphic GetGraphic(int ticksPassed)
        {
            var frame = Mathf.FloorToInt(props.frames.Count * position * 0.99999f);
            return props.frames[frame].Graphic;
        }
    }
}
