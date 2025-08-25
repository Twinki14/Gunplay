using UnityEngine;
using Verse;

namespace Gunplay
{
    internal class ProjectileTrail : ThingWithComps
    {
        private Matrix4x4 _drawingMatrix;

        private int _ticksUtilDeath;
        private int _ticksIdle;
        private Projectile _projectile;
        private Vector3 _a;
        private Vector3 _exactPosition;
        private Vector3 _previousPosition;
        private float _speed;
        private Vector3 _dir;
        private float _length;
        private float _width;

        public void Initialize(Projectile proj, Vector3 destination, Thing equipment)
        {
            _ticksUtilDeath = -1;
            _ticksIdle = 0;
            _projectile = proj;
            _a = _projectile.ExactPosition;
            var b = destination;
            _a.y = b.y = proj.def.Altitude;
            _speed = proj.def.projectile.SpeedTilesPerTick;
            _exactPosition = _a;

            _length = _speed * 15f;
            _dir = (b - _a).normalized;
            _width = proj.DamageAmount * 0.006f;

            if (def is ProjectileTrailDef trailDef && !Mathf.Approximately(trailDef.trailWidth, -1))
            {
                _width = trailDef.trailWidth;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref _projectile, "projectile");
            Scribe_Values.Look(ref _a, "a");
            Scribe_Values.Look(ref _speed, "speed");
            Scribe_Values.Look(ref _dir, "dir");
            Scribe_Values.Look(ref _width, "width");
            Scribe_Values.Look(ref _exactPosition, "exactPosition");
            Scribe_Values.Look(ref _ticksUtilDeath, "ticksUtilDeath");
        }

        public override Vector3 DrawPos => _exactPosition;

        private float TicksPerLength => _length / _speed;

        protected override void Tick()
        {
            base.Tick();

            if (_projectile != null)
            {
                if (_projectile.Destroyed || _projectile?.ExactPosition is null)
                {
                    _projectile = null;
                    _ticksUtilDeath = Mathf.CeilToInt(TicksPerLength);
                }
                else
                {
                    _exactPosition = _projectile.ExactPosition;
                    _exactPosition.y = _projectile.def.Altitude;

                    if (_ticksUtilDeath == -1 && _ticksIdle > 10 && _previousPosition == _exactPosition)
                    {
                        _projectile = null;
                        _ticksUtilDeath = Mathf.CeilToInt(TicksPerLength);
                    }
                }
            }

            if (_previousPosition == _exactPosition)
                _ticksIdle++;

            _previousPosition = _exactPosition;

            Position = _exactPosition.ToIntVec3();

            if (_ticksUtilDeath > 0)
                _ticksUtilDeath--;
            else if(_ticksUtilDeath == 0)
                Destroy(DestroyMode.Vanish);
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            var traveled = (_a - _exactPosition).magnitude;
            float len;
            Vector3 drawPos;
            if (traveled < _length)
            {
                len = traveled;
                if (_ticksUtilDeath == -1)
                {
                    drawPos = (_a + _exactPosition) / 2;
                }
                else
                {
                    len *= _ticksUtilDeath / TicksPerLength;
                    drawPos = _exactPosition - _dir * len / 2;
                }
            }
            else if (_ticksUtilDeath != -1)
            {
                len = _length * _ticksUtilDeath / TicksPerLength;
                drawPos = _exactPosition - _dir * len / 2;
            }
            else
            {
                len = _length;
                drawPos = _exactPosition - _dir * len / 2;
            }

            var drawingScale = new Vector3(_width, 1f, len);
            _drawingMatrix.SetTRS(drawPos, Quaternion.LookRotation(_dir), drawingScale);
            Graphics.DrawMesh(MeshPool.plane10, _drawingMatrix, Graphic.MatSingle, 0);

            Comps_PostDraw();
        }
    }
}
