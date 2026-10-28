using UnityEngine;

namespace DefaultNamespace.Helpers
{
    public static class ChunkHelper
    {
        public static bool IsInChunkXZ(Vector3 pos, Bounds bounds)
        {
            return pos.x >= bounds.min.x &&
                   pos.x <= bounds.max.x &&
                   pos.z >= bounds.min.z &&
                   pos.z <= bounds.max.z;
        }
    }
}