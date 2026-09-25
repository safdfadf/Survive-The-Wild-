using DefaultNamespace.Weapon.WeaponAnims;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace DefaultNamespace.Weapon
{
    public class ProjectileParent : WeaponBehaviour
    {
        [SerializeField] protected Transform RestPoint;
        private Vector3 crossHairPoint;
        protected bool isAiming;
        protected ProjectileAnim _animator;


        protected virtual void Awake()
        {
            _animator = GetComponent<ProjectileAnim>();
        }

        protected virtual void Update()
        {
            UpdateAimTarget();
            UpdateCrosshair();
           
        }

        private void LateUpdate()
        {
            Player.SetSpineControl(isAiming);
        }

        private void UpdateAimTarget()
        {
            Vector3 camPos = cameraTransform.position;
            Vector3 camForward = cameraTransform.forward;
            Vector3 crosshairWorldPoint = camPos + camForward * 100f;


            float aimOffsetX = Vector3.Dot(
                RestPoint.position - camPos,
                cameraTransform.right
            );

            Vector3 offsetOrigin = camPos + cameraTransform.right * aimOffsetX;

            Vector3 targetPoint = offsetOrigin + camForward * 100f;

            Vector3 rayDirection = (crosshairWorldPoint - offsetOrigin).normalized;

            Ray ray = new Ray(offsetOrigin, rayDirection);

            crossHairPoint = targetPoint;
            int mask = ~LayerMask.GetMask("Resources");

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * 100f;
            }


            aimTarget.position = targetPoint;
        }

        private void UpdateCrosshair()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(crossHairPoint);
            weapon.crosshair.transform.position = screenPos;
        }

        public override void OnInput(InputAction.CallbackContext ctx)
        {
            if (weapon.RestrictUse) return;
            if (ctx.interaction is TapInteraction)
            {
                StartAiming();
            }

            else if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Performed)
            {
                StartAiming();
            }

            else if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Canceled && !isAiming)
            {
                StopAiming();
            }
            else if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Canceled)
            {
                Shoot();
            }
        }

        protected virtual void StartAiming()
        {
        }

        protected virtual void StopAiming()
        {
        }

        protected virtual void Shoot()
        {
        }

        protected virtual void UpdateRestRotation()
        {
            Vector3 direction = (aimTarget.position - RestPoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            RestPoint.rotation = Quaternion.Slerp(RestPoint.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }
}