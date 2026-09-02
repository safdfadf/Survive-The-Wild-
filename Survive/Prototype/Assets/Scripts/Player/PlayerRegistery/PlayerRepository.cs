using System;
using Effect;
using UnityEngine;

namespace Player
{
    public class PlayerRepository:MonoBehaviour
    {
        public static PlayerRepository instance;
        private PlayerScentEmitter _playerScentEmitter;
        private PlayerNoiseEmitter _playerNoiseEmitter;
        private MovementHandler _movementHandler;
        private PlayerInventory _playerInventory;
        private PlayerVitalStats _playerVitalStats;
        private PlayerBody _playerBody;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            _playerScentEmitter = GetComponent<PlayerScentEmitter>();
            _playerNoiseEmitter = GetComponent<PlayerNoiseEmitter>();
            _movementHandler = GetComponent<MovementHandler>();
            _playerInventory = GetComponent<PlayerInventory>();
            _playerVitalStats = GetComponent<PlayerVitalStats>();
            _playerBody = GetComponent<PlayerBody>();
        }

        private void LateUpdate()
        {
            
        }

        public float GetScentIntensity(Vector3 position)
        {
           return _playerScentEmitter.GetIntensityAt(position);
        }

        public float GetNoiseIntensity(Vector3 position)
        {
            
           return _playerNoiseEmitter.GetNoiseIntensityAt(position);
        }

        public float GetCurrentNoise()
        {
          return _playerNoiseEmitter.GetCurrentNoise();
        }
        public Transform GetPlayerTransform()
        {
          return _movementHandler.GetPlayerTransform();
        }

        public bool GetIsSprinting()
        {
           return _movementHandler._isSprinting;
        }

        public bool GetIsCrouching()
        {
            return _movementHandler.IsCrouching;
        }

        public bool GetIsWalking()
        {
            return _movementHandler._isWalking;
        }

        public void RemoveResourceFromInventory(Obj<ObjSo> resource,bool isToBeDestroy)
        {
            _playerInventory.RemoveResource(resource,isToBeDestroy);
        }

        public void RemoveWeapon(WeaponSo so)
        {
            _playerInventory.RemoveWeapon(so);
        }
        public void ConsumeFood(FoodSo so)
        {
            _playerVitalStats.ConsumeFood(so);
        }

        public bool GetHunterSense()
        {
            return _movementHandler.isHuntingSenseActive;
        }
        public void CanPlayerMove(bool isLocked)
        {
            _movementHandler.TogglePlayerLock(isLocked);
        }

        public void ApplyDamage(IAttack attack)
        {
            _playerBody.TakeDamage(attack);
        }

        public void HealPlayer(EffectsSo effect)
        {
            _playerBody.HealPlayer(effect);
        }
        public Transform GetApproachPos()
        {
            return _movementHandler.animalApproachPos;
        }

        public void CraftWorldItem(GameObject obj)
        {
            Obj<ObjSo> o = obj.GetComponent<Obj<ObjSo>>();
            _playerInventory.MakeItemAnCraft(o);
        }

        public void ToggleCursor(bool isCursorOn)
        {
            _movementHandler.ToggleCursor();
        }

        public void SetAttacking(bool isAttacking)
        {
            _movementHandler.isAttacking = isAttacking;
        }

        public void HandleSpinRotation(bool toggle)
        {
            _movementHandler.SetSpineControl(toggle);
        }

        public GameObject GetResource(ObjSo so)
        {
            return _playerInventory.GetResource(so);
        }

        public BaseWeapon GetCurrentWeapon()
        {
           return _movementHandler.CurrentWeapon;
        }

        public Vector3 GetPlayerUiPos()
        {
            return _movementHandler.uiPos.position;
        }

    }
}