using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


//ToDo: remove this class 
public class BaseObj : Obj<ObjSo>
{
    private float timeCount;
    protected Mesh originalMesh;
    [SerializeField] private ObjSo objSo;
    [SerializeField] private int amount;

    protected override void Awake()
    {
        Gm = gameObject;

        base.Awake();
    }

    public override void Harvest()
    {
        // ToDo:harvest screen
        DropObject();
    }

    private void DropObject()
    {
        int num = 0;
        if (objSo == null) return;
        for (var i = amount; i >= 0; i--)
        {
            GameObject obj = GlobalPool.instance.Get(objSo.prefab, transform.position);
            Obj<ObjSo> baseObj = obj.GetComponent<Obj<ObjSo>>();
            if (baseObj == null) continue;
            baseObj.Initialize(objSo);
            Collider collider = baseObj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            else
            {
                Debug.Log(baseObj.name + " is missing Collider");
            }

            num++;
        }

        print(num);
        GlobalPool.instance.Return(So.prefab, gameObject);
    }
}