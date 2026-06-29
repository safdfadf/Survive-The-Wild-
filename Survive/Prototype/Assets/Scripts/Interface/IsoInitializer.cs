using UnityEngine;

public interface IsoInitializer<TSo>
{
    void Initialize(TSo so);
    void SeCashedPos(PosInChunk casedPos);
}
