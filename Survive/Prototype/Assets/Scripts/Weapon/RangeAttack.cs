using System;
using Player;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace DefaultNamespace.Weapon
{
    public class RangeAttack : WeaponBehaviour
    {
        [SerializeField] protected Transform RestPoint;
        protected float drawTime;
        [SerializeField] protected float maxDrawTime = 1f;
        private Vector3 crossHairPoint;
        [SerializeField] private float _mixArrowSpeed = 0f;
        [SerializeField] private float _maxArrowSpeed = 50;
        private PlayerInventory playerInventory;
        [SerializeField] private ObjSo shootables;

        private bool isAiming;
        public GameObject CurrentArrow { get; private set; }
        private MovementHandler movementHandler;

        private void Awake()
        {
            movementHandler = FindAnyObjectByType<MovementHandler>();
        }

        public override void OnInput(InputAction.CallbackContext ctx)
        {
            Debug.Log(ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Canceled); 
            if (ctx.interaction is TapInteraction)
            {
                StartAiming();
            }

            else if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Performed)
            {
                Debug.Log("start");
                StartAiming();
            }

            // EARLY RELEASE → stop aiming
            else if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Canceled && !isAiming)
            {
                Debug.Log("stop");
                StopAiming();
            }
            else if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Canceled )
            {
                Debug.Log("shoot");
                Shoot();
            }
        }

        protected void Update()
        {
            if (!isAiming || CurrentArrow == null || !isEquipped) return;
            UpdateBowRotation();
            PullArrow();
            UpdateAimTarget();
            UpdateCrosshair();
          
        }

        private void LateUpdate()
        {
            movementHandler.SetSpineControl(isAiming);
        }

        private void PullArrow()
        {
            drawTime += Time.deltaTime;
            drawTime = Mathf.Clamp(drawTime, 0f, maxDrawTime);
            float drawPercent = Mathf.Clamp01(drawTime / maxDrawTime);
            float drawOffset = Mathf.Lerp(0f, -0.5f, drawPercent);
            CurrentArrow.transform.localPosition = new Vector3(0f, 0f, drawOffset);
        }

        private void PrepareNextArrow() // this function should be here 
        {
            animator.DrawArrow();
            CurrentArrow = PlayerRepository.instance.GetResource(shootables); // how can we get current arrow 
            if (CurrentArrow == null) return;
            ArrowScript Arrow = CurrentArrow.GetComponent<ArrowScript>();
            if (Arrow == null)
            {
                Debug.Log("no arrow found");
            }

            Arrow.canBeCollected = false;
            CurrentArrow.transform.SetParent(RestPoint, false);
            CurrentArrow.transform.localPosition = Vector3.zero;
            CurrentArrow.transform.localRotation = Quaternion.identity;
            CurrentArrow.SetActive(true);
        }

        public void StartAiming()
        {
            drawTime = 0;
            weapon.crosshair.SetActive(true);
            isAiming = true;
            animator.Aim(isAiming);
            if (CurrentArrow == null)
            {
                PrepareNextArrow();
            }
        }

        public virtual void StopAiming()
        {
            isAiming = false;
            animator.Aim(false);
            drawTime = 0f;
            weapon.crosshair.SetActive(false);
        }

        private void UpdateBowRotation()
        {
            Vector3 direction = (aimTarget.position - RestPoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            RestPoint.rotation = Quaternion.Slerp(RestPoint.rotation, lookRotation, Time.deltaTime * 10f);
        }

        public void Shoot()
        {
            animator.FireArrow(true);
          
            if (CurrentArrow == null)
            {
                return;
            }


            ArrowScript arrowScript = CurrentArrow.GetComponent<ArrowScript>();
            if (arrowScript != null)
            {
                arrowScript.canBeCollected = true;
            }

            if (CurrentArrow == null && !isAiming) return;

            float powerPercent = drawTime / maxDrawTime;
            float arrowSpeed = Mathf.Lerp(_mixArrowSpeed, _maxArrowSpeed, powerPercent);
            CurrentArrow.transform.SetParent(null);


            Vector3 shootDirection = (aimTarget.position - RestPoint.position).normalized;
           arrowScript.ShootArrow(shootDirection, arrowSpeed);
            StartCoroutine(animator.ResetFireArrow());

            CurrentArrow = null;
            drawTime = 0f;
            isAiming = false;
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
    }
}

public class ShootableData
{
    public ObjSo so { get; set; }
}