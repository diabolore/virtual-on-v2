using UnityEngine;

namespace VirtualOnRevived.AI
{
    public enum BotState
    {
        NeutralApproach,
        AerialBoost,
        RangedHarass,
        SecondaryAttackOpportunist
    }

    public class MechBotController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float optimalRangedDistance = 30f;
        [SerializeField] private float secondaryAttackEngageDistance = 10f;
        
        private BotState currentState = BotState.NeutralApproach;

        private void Update()
        {
            if (target == null) return;

            EvaluateState();
            ExecuteState();
        }

        private void EvaluateState()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget < secondaryAttackEngageDistance)
            {
                currentState = BotState.SecondaryAttackOpportunist;
            }
            else if (distanceToTarget > optimalRangedDistance)
            {
                currentState = BotState.NeutralApproach;
            }
            else
            {
                // Mid-range decision logic
                if (Random.value < 0.01f) // Example trigger
                    currentState = BotState.AerialBoost;
                else if (currentState != BotState.AerialBoost)
                    currentState = BotState.RangedHarass;
            }
        }

        private void ExecuteState()
        {
            switch (currentState)
            {
                case BotState.NeutralApproach:
                    // Move towards target
                    break;
                case BotState.AerialBoost:
                    // Trigger jump/thrusters
                    break;
                case BotState.RangedHarass:
                    // Strafe and use primary attack
                    break;
                case BotState.SecondaryAttackOpportunist:
                    // Dash in and use secondary attack
                    break;
            }
        }
    }
}
