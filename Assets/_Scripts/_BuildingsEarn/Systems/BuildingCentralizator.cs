using UnityEngine;

namespace _Scripts._BuildingsEarn.Systems
{
    public class BuildingCentralizator : MonoBehaviour
    {
        public void _StopWorking()
        {
            ChangeActiveState(false);
        }

        public void _StartWorking()
        {
            ChangeActiveState(true);
        }

        private void ChangeActiveState(bool enable)
        {
            var earner = GetComponent<ResourceEarnerBuilding>();
            var consumable = GetComponent<ConsumableBuilding>();

            if (enable)
            {
                earner?.ContinueEarn();
                consumable?.ResumeConsuming();
            }
            else
            {
                earner?.StopEarn();
                consumable?.StopConsume();
            }
        }
    }
}