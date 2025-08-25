using Verse;

namespace Gunplay
{
    public class CompGun : ThingComp
    {
        private float _rotationSpeed;
        private float _rotationOffset;
        private int _ticksPreviously;

        private void UpdateRotationOffset(int ticks)
        {
            if (_rotationOffset == 0) return;
            if (ticks <= 0) return;
            if (ticks > 30) ticks = 30;

            if (_rotationOffset > 0)
            {
                _rotationOffset -= _rotationSpeed;
                if (_rotationOffset < 0) _rotationOffset = 0;
            }
            else if (_rotationOffset < 0)
            {
                _rotationOffset += _rotationSpeed;
                if (_rotationOffset > 0) _rotationOffset = 0;
            }

            _rotationSpeed += ticks * 0.01f;
        }

        public float RotationOffset
        {
            get
            {
                var ticks = Find.TickManager.TicksGame;
                UpdateRotationOffset(ticks - _ticksPreviously);
                _ticksPreviously = ticks;

                return _rotationOffset;
            }
            set
            {
                _rotationOffset = value;
                _rotationSpeed = 0;
            }
        }
    }
}
