using UnityEngine;

namespace FoodSystem
{
    public class Coconut : Food
    {
        [SerializeField] private ObjSo splitCoconut;
        [SerializeField] private Material harvestedMaterial;
        private Material _originalMaterial;

        protected override void Awake()
        {
            base.Awake();
            canUse = false;
            canCookMe = false;
        }

        public override void Harvest()
        {
            // harvest/ Load  screen
            MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
            renderer.material = harvestedMaterial;
            canUse = true;
            useMeDescription = "Drink";
        }

        public override void UseMe()
        {
            // Add Split Coconut to the inventory 

            base.UseMe();
        }
    }
}