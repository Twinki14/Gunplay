using Verse;

namespace Gunplay
{
    public class CompSpinningGun : CompPrimer
    {
        new CompPropertiesSpinningGun props => base.props as CompPropertiesSpinningGun;
        public override int TicksToIdle => props.ticksToIdle;

        private float _rotation;

        public override Graphic GetGraphic(int ticksPassed)
        {
            _rotation += position * props.rotationSpeed * ticksPassed;

            var frame = (int)_rotation % props.frames.Count;
            return props.frames[frame].Graphic;
        }
    }
}
