using System.IO;
using LevelManagement;

namespace Editor_LevelCreation
{
    public static class LevelPathHelper
    {
        public static string BuildBaseLevelPath(string LevelBasePath, LevelZonesID AdventureLevelID, LevelZonesID LevelZonesID)
        {
            var directoryPath = LevelBasePath + "/" + AdventureLevelID.ToString();
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var scenePath = directoryPath + "/" + LevelZonesID.ToString() + ".unity";
            return scenePath;
        }

        public static string BuilChunkPath(string LevelBasePath, LevelZonesID AdventureLevelID, LevelZoneChunkID LevelZoneChunkID)
        {
            var directoryPath = LevelBasePath + "/" + AdventureLevelID.ToString();
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            directoryPath = directoryPath + "/Chunks";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath + "/" + LevelZoneChunkID.ToString() + "_Chunk.unity";
        }
    }
}