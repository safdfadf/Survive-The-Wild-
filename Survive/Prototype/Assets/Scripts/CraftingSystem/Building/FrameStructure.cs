using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FrameStructure : BaseStructure
{
    [FormerlySerializedAs("sockets")] [SerializeField]
    protected List<Sockets> childSockets = new();

    [SerializeField] private List<Sockets> FrameSockets = new();
    private bool otherFrameContact;

    [SerializeField] private StructureType allowedType;

    public Sockets GetClosestValidSocket(StructureType type, Vector3 fromPos, float maxDistance = 3f,
        bool isFrameSocket = false)
    {
        Sockets best = null;
        float bestDist = maxDistance;
        List<Sockets> requestedSockets = new();
        requestedSockets = isFrameSocket ? FrameSockets : childSockets;

        foreach (var socket in requestedSockets)
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

    public override bool CheckPlacement()
    {
        if (otherFrameContact)
        {
            return SocketPlacement();
        }

        return TryFreePlacement();
    }

    private bool SocketPlacement()
    {
        var hits = Physics.OverlapSphere(
            transform.position,
            socketSearchRadius,
            collisionMask
        );
        foreach (var col in hits)
        {
            var frame = col.GetComponentInParent<FrameStructure>();
            if (frame == null) continue;

            var socket = frame.GetClosestValidSocket(
                craftingRecipe.structureType,
                transform.position, 3, true
            );

            if (socket != null)
            {
                transform.position = socket.transform.position;
                transform.rotation = socket.transform.rotation;

                // canPlace = true;
                ToggleMat(true);
                return true;
            }
        }

        //canPlace = false;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<FrameStructure>() == null)
            return;

        otherFrameContact = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<FrameStructure>() == null)
            return;

        otherFrameContact = false;
    }
}