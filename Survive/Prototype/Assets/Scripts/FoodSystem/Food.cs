using DefaultNamespace;
using Effect;
using Player;
using UnityEngine;
using UnityEngine.Serialization;


namespace FoodSystem
{
    public class Food : Obj<ObjSo>, ICookable //Todo: after being collected start a timer if food gets rotten 
    {
        private int _health;

        [Header("cooking Threshold")] [SerializeField]
        public float cookTime = 10;

        [SerializeField] public float burnTime = 20;
        private MeshRenderer[] _mrs;
        private Material[] _materials;
        public FoodConsumptionData data = new();

        public bool canCookMe;

        [Header("StateData")] public RawState rawState;
        public CookState cookState;
        public BurntState burntState;

        public StateData currentState { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Gm = gameObject;
            useMeDescription = "Eat";
            _mrs = gameObject.GetComponentsInChildren<MeshRenderer>();
            currentState = rawState;
        }

        public override void Initialize(ObjSo so)
        {
            So = so;
            FoodSo s = So as FoodSo;
            if (s == null) return;
            data.NutrientsCount = s.nutrientsCount;
        }

        public override void UseMe()
        {
            PlayerRepository.instance.ConsumeFood(data);
            base.UseMe();
        }

        private void ApplyState(StateData state)
        {
            currentState = state;
            foreach (var r in _mrs)
            {
                r.material = state.material;
            }

            data.SanityEffect = state.sanityEffect;
            if (!state.isPoison) return;
            SelfAttack atk = new SelfAttack(Mathf.CeilToInt(state.effect.damage), Vector3.zero);
            atk.Effects = state.effect;
            data.SelfAttack = atk;
        }

        public override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = true;
        }

        public virtual void ExecuteCooking(float deltaTime)
        {
            if (!canCookMe) return;
            if (currentState == rawState)
            {
                cookTime -= deltaTime;
                if (cookTime <= 0)
                {
                    ApplyState(cookState);
                }
            }
            else if (currentState == cookState)
            {
                burnTime -= deltaTime;
                if (burnTime <= 0)
                {
                    ApplyState(burntState);
                    canCookMe = false;
                }
            }
        }
    }
}

[System.Serializable]
public class StateData
{
    public int sanityEffect;
    public bool isPoison;
    public EffectsSo effect;
    public Material material;
}

[System.Serializable]
public class RawState : StateData
{
}

[System.Serializable]
public class CookState : StateData
{
}

[System.Serializable]
public class BurntState : StateData
{
}