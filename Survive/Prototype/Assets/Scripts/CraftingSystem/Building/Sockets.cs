using UnityEngine;

public class Sockets : MonoBehaviour
{
    public StructureType allowedType;
    public bool isOccupied { get; set; }
    public BaseStructure currentStructure { get; set; }

    public bool CanAccept(StructureType type)
    {
        return !isOccupied && allowedType == type;
    }

    public void Attach(BaseStructure structure)
    {
        currentStructure = structure;
        isOccupied = true;
    }

    public void Detach()
    {
        currentStructure = null;
        isOccupied = false;
    }
}