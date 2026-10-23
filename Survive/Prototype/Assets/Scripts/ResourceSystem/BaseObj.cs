using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        UIManager.instance.ShowLoadingScreen(2f);
        DropObject();
        base.Harvest();
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            var collider = gameObject.GetComponent<Collider>();
            collider.isTrigger = true;
            var rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null) return;
            rb.isKinematic = true;
        }
    }
}