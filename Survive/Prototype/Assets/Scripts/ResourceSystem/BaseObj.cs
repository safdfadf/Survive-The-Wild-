using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public enum ResourceType
{
    Wood,
    Stone,
    Leaf,
    Metal,
    Log,
    LongWood,
    Stick,
    Rope
}

//ToDo: remove this class 
public class BaseObj : Obj<ObjSo>
{
    private float timeCount;
    protected Mesh originalMesh;
    

    protected override void Awake()
    {
        Gm = gameObject;
        base.Awake();
      
    }

}