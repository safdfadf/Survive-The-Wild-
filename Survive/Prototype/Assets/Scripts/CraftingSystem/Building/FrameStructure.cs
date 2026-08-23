using System.Collections.Generic;
using UnityEngine;

public class FrameStructure : BaseStructure
{
    [SerializeField] protected List<Sockets> sockets = new();
    protected override void OnStructureAssembled()
    {
        base.OnStructureAssembled();
        // Show sockets or hint UI if you want
        // e.g. UIManager.instance.ShowSockets(sockets);
    }

    public Sockets GetClosestValidSocket(StructureType type, Vector3 fromPos, float maxDistance = 3f)
    {
        Sockets best = null;
        float bestDist = maxDistance;

        foreach (var socket in sockets)
        {
            if (!socket.CanAccept(type)) continue;

            float dist = Vector3.Distance(fromPos, socket.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = socket;
            }
        }

        return best;
    }
    public void RegisterChild(BaseStructure child)
    {
        if (!childStructures.Contains(child))
            childStructures.Add(child);
    }
}