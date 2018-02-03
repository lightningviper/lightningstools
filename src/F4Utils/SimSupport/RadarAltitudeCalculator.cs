using F4SharedMem;
using F4SharedMem.Headers;
using F4Utils.Terrain;

namespace F4Utils.SimSupport
{
    public interface IRadarAltitudeCalculator
    {
        float ComputeRadarAltitude(FlightData flightData, TerrainDB terrainDB, uint lod = 2);
    }

    public class RadarAltitudeCalculator : IRadarAltitudeCalculator
    {
        public float ComputeRadarAltitude(FlightData flightData, TerrainDB terrainDB, uint lod = 2)
        {
            var terrainHeight = terrainDB?.CalculateTerrainHeight(flightData.x, flightData.y, lod) ?? 0;
            var ralt = -flightData.z - terrainHeight;

            //reset AGL altitude to zero if we're on the ground
            if (WeightOnWheels(flightData) || OnGround(flightData)) { ralt = 0; }

            if (ralt < 0) { ralt = 0; }
            return ralt;
        }

        private static bool OnGround(FlightData flightData) { return (flightData.lightBits3 & (int) LightBits3.OnGround) == (int) LightBits3.OnGround; }

        private static bool WeightOnWheels(FlightData flightData) { return (flightData.lightBits & (int) LightBits.ONGROUND) == (int) LightBits.ONGROUND; }
    }
}