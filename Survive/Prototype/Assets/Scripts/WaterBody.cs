using System;
using DefaultNamespace.ResourceSystem;
using FoodSystem;
using UnityEngine;

namespace DefaultNamespace
{
    public class WaterBody : MonoBehaviour// this script will be explored later 
    {
        public WaterBodyType bodyType;
        public WaterState waterState;
        private WaterObj _waterObj;
        private void Awake()
        {
            _waterObj = GetComponent<WaterObj>();
        }

        private void WashYourself() // it can be used in two ways one wash your self and consume me 
        {
            // reset player scent emitter 
        }
        public void FillContainer()
        {
            // get the container 
        }

        public void DrinkMe()
        {
            _waterObj.UseMe();
        }
    }

    public enum DirtLevel
    {
        Safe,
        UnSafe,
        Dirty
    }
}