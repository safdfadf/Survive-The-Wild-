using UnityEngine.InputSystem;

namespace SurviveTheWild.Interface
{
    public interface IRequestInput
    {
        public void UseInput(InputAction.CallbackContext ctx);
        public void ResetInput();
    }
}