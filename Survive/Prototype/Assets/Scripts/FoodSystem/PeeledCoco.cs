using DefaultNamespace.EventBus;
using UnityEngine;

namespace FoodSystem
{
    public class PeeledCoco : Food
    {
        [SerializeField] private ObjSo Bowl;

        protected override void Awake()
        {
            base.Awake();
            UseMeDescription = "Drink";
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