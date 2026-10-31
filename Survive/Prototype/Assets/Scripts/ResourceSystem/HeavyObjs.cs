namespace SurviveTheWild.ResourceSystem
{
    public class HeavyObjs : Obj<ObjSo>
    {
        protected override void Awake()
        {
            base.Awake();
            Gm = this.gameObject;
        }
    }
}