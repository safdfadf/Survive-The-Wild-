namespace DefaultNamespace.ResourceSystem
{
    public class HeavyObjs : Obj<ObjSo>
    {
        protected override void HandleAction()
        {
            Carry();
        }
        // Tutorial to drop the object
        private void Carry()
        {
            // fire event heard by player and start carrying 
            // there is other things aswell what is player has equipped a weapon 
            // this object gets a position by the player 
        }

        public void Drop()
        {
            // how will this be called ask animator to get back to normal 
            // set parent is set to null and is dropped 
        }
    }
}