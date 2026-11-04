using System;

namespace SurviveTheWild.EventBus.Events
{
    public class ChunkEvents
    {
        public Action<Chunk> ActiveChunk;
        public Action<Chunk> DeActiveChunk;
        
        
    }
}