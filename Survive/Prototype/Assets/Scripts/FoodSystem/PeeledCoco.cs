using DefaultNamespace.EventBus;
using UnityEngine;

namespace FoodSystem
{
    public class PeeledCoco : Food
    {
        // Role: this coco when can not be harvested further and can only be eaten  
        [SerializeField] private ObjSo Bowl;

        protected override void Awake()
        {
            base.Awake();
            canUse = true;
            useMeDescription = "Drink";
        }

        public override void UseMe()
        {
            var obj = GlobalPool.instance.Get(Bowl.prefab, transform.position);
            Obj<ObjSo> res = obj.GetComponent<Obj<ObjSo>>();
            res.So = Bowl;
            EventManager.Instance.reseourceEvent.GatherResource(obj);
            base.UseMe();
        }
    }
}