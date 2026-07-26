using UnityEngine;

public class UnScheduleAnimal : AnimalBase// these animals do not have a schedule and spawned around the player
{
    // i want this animal check if player is in radius CheckPlayerInProximity based on change state and do damage to the player 
    public void Initialize(AnimalSo so)
    {
        AnimalSo = so;
    }

    public void Attack()
    {
        // will use IAttack 
    }
}
