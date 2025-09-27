using System;

namespace Editor.Engine
{
    internal class CelestialBody
    {
        public Models CelestialBodyModel { get; set; }
        public byte BodyType { get; set; } //0 for sun, 1 for planet, 2 for moon

        CelestialBody(Models _celestialBodyModel, byte _bodyType)
        {
            CelestialBodyModel = _celestialBodyModel;
            BodyType = _bodyType;
        }

        public void Render()
        {

        }
    }
}
