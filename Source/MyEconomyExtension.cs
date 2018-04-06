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
        int taxVariable = 15000;

        GoogleApiHandler apiHandler;
        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            //Still unsure how tax multiplier works
            try
            {
                var type = typeof(EconomyManager);
                var taxMultiplier = type.GetField("m_taxMultiplier", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                taxMultiplier.SetValue(EconomyManager.instance, CalculateTaxBonus(15000f));
                //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, CalculateTaxBonus(8500).ToString());
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
            SetConstructionCostBonuses(steps);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "EconomyExtension is created and electricityBonus = " + electricityCostBonus);

        }

        int CalculateTaxBonus(float steps)
        {//ssss
            if (steps < 8000)
            {
                return (int)(0.7 * taxVariable);
            }
            else
            {
                return (int)((steps / 10000f) * taxVariable);               
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
            else if (service == Service.Electricity)
            {
                return (int)(originalConstructionCost * electricityCostBonus);
            }
            else
                return base.OnGetConstructionCost(originalConstructionCost, service, subService, level);

        }


    }
}
