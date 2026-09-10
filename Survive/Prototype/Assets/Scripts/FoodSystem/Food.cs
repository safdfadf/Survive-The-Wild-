using DefaultNamespace;
using Effect;
using Player;
using UnityEngine;
using UnityEngine.Serialization;


namespace FoodSystem
{
    public class Food : Obj<ObjSo> //Todo: after being collected start a timer if food gets rotten 
    {
        private int _health;

        [Header("cooking Threshold")] [SerializeField]
        public float cookTime = 10;

        [SerializeField] public float burnTime = 20;
        public FoodConsumptionData counsmptionData { get; set; }
       
        [SerializeField] private Material cookedMaterial;
        [SerializeField] private Material burntMaterial;
        private MeshRenderer[] _mrs;
        private Material[] _materials;
        private FoodConsumptionData data = new();

        public bool canCookMe;

        [Header("De buff Probability")] [SerializeField]
        private float minpoisonProbab;

        [SerializeField] private float maxpoisonProbab;
        [Header("StateData")] public RawState rawState;
        public CookState cookState;
        public BurntState burntState;
        [FormerlySerializedAs("currentStateData")] public StateData currentState;

        protected override void Awake()
        {
            canUse = true;
            Gm = gameObject;
            useMeDescription = "Eat";
            base.Awake();
            _mrs = gameObject.GetComponentsInChildren<MeshRenderer>();
            currentState = rawState;
        }

        public override void Initialize(ObjSo so)
        {
            So = so;
            FoodSo s = So as FoodSo;
            counsmptionData = new FoodConsumptionData();
            if (s == null) return;
            counsmptionData.NutrientsCount = s.nutrientsCount;
            data.NutrientsCount = s.nutrientsCount;
        }

        public override void UseMe()
        {
            PlayerRepository.instance.ConsumeFood(data);
            base.UseMe();
        }

        public void ApplyState(StateData state)
        {
            currentState = state;
            foreach (var r in _mrs)
            {
                r.material = state.material;
            }
            data.SanityEffect = state.sanityEffect;
           if(!state.isPoison)return;
           SelfAttack atk = new SelfAttack(Mathf.CeilToInt(state.effect.damage), Vector3.zero);
           atk.Effects.Add( state.effect);
           data.SelfAttack = atk;
        }
        protected override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = true;
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