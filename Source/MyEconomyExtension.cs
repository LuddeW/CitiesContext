using ColossalFramework.Plugins;
using ICities;
using System;

namespace CitiesConext
{
    public class MyEconomyExtension : EconomyExtensionBase
    {
        int taxBonus = 1;
        float educationCostBonus = 1;
        float electricityCostBonus = 1;
        float healthCareCostBonus = 1;

        GoogleApiHandler apiHandler;
        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            //Still unsure how tax multiplier works
            try
            {
                var type = typeof(EconomyManager);
                var taxMultiplier = type.GetField("m_taxMultiplier", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                taxMultiplier.SetValue(EconomyManager.instance, taxBonus * 100); // *2000 should be about right, but needs more testing
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, e.ToString());
            }
            return base.OnUpdateMoneyAmount(internalMoneyAmount);
        }

        public override void OnCreated(IEconomy economy)
        {
            int steps;

            apiHandler = new GoogleApiHandler();
            steps = apiHandler.GetSteps();

            taxBonus = CalculateTaxBonus(steps);
            SetConstructionCostBonuses(steps);


        }

        int CalculateTaxBonus(int steps)
        {
            if (steps < 10000)
            {
                return 0;
            }
            else
            {
                return ((steps % 10000) / 1000) + 1;
            }

        }

        void SetConstructionCostBonuses(int steps)
        {
            if (steps < 5000)
            {
                educationCostBonus = 1.7f;
                healthCareCostBonus = 1.7f;
                electricityCostBonus = 1.7f;
            }
            else if (steps >= 5000 && steps < 10000)
            {
                educationCostBonus = 1f;
                healthCareCostBonus = 1f;
                electricityCostBonus = 1f;
            }
            else if (steps >= 10000 && steps < 20000)
            {
                educationCostBonus = 0.6f;
                healthCareCostBonus = 0.6f;
                electricityCostBonus = 0.6f;
            }
            else
            {
                educationCostBonus = 0.3f;
                healthCareCostBonus = 0.3f;
                electricityCostBonus = 0.3f;
            }
        }

        public override int OnGetConstructionCost(int originalConstructionCost, Service service, SubService subService, Level level)
        {
            if (service == Service.Education)
            {
                return (int)(originalConstructionCost * educationCostBonus);
            }
            else if (service == Service.HealthCare)
            {
                return (int)(originalConstructionCost * healthCareCostBonus);
            }
            else if (service == Service.Education)
            {
                return (int)(originalConstructionCost * electricityCostBonus);
            }
            else
                return base.OnGetConstructionCost(originalConstructionCost, service, subService, level);

        }


    }
}
