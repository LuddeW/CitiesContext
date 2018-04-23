using ColossalFramework.Plugins;
using ICities;
using System;
using System.Collections.Generic;

namespace CitiesConext
{
    public class MyEconomyExtension : EconomyExtensionBase
    {
        int taxBonus = 1;
        float educationCostBonus = 1;
        float electricityCostBonus = 1;
        float healthCareCostBonus = 1;
        int taxVariable = 9000;
        int avgSteps = 10000;
        

        GoogleApiHandler apiHandler;
        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            //Still unsure how tax multiplier works
            try
            {
                var type = typeof(EconomyManager);
                var taxMultiplier = type.GetField("m_taxMultiplier", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                taxMultiplier.SetValue(EconomyManager.instance, CalculateTaxBonus(avgSteps));
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
            int steps = 8000;
            

            apiHandler = new GoogleApiHandler();
            steps = apiHandler.GetSteps();
            avgSteps = apiHandler.GetAvgSteps();
            SetConstructionCostBonuses(steps);
            SetConstructionBonusesFromActiveHours();

        }

        int CalculateTaxBonus(float steps)
        {//ssss
            if (steps < 8000)
            {
                return (int)(0.92f * taxVariable);
            }
            else if (steps <= 10000 && steps >= 8000)
            {
                return (int)(1.0f * taxVariable);
            }
            else if (steps <= 12000 && steps > 10000)
            {
                return (int)(1.03f * taxVariable);               
            }
            else
            {
                return (int)(1.07f * taxVariable);
            }

        }

        /// <summary>
        /// Bonuses from number of steps
        /// </summary>
        /// <param name="steps"></param>
        void SetConstructionCostBonuses(int steps)
        {
            if (steps < 5000)
            {
                educationCostBonus = 1.3f;
                electricityCostBonus = 1.3f;
            }
            else if (steps >= 5000 && steps < 10000)
            {
                educationCostBonus = 1f;
                electricityCostBonus = 1f;
            }
            else if (steps >= 10000 && steps < 20000)
            {
                educationCostBonus = 0.6f;
                electricityCostBonus = 0.6f;
            }
            else
            {
                educationCostBonus = 0.3f;
                electricityCostBonus = 0.3f;
            }
        }

        /// <summary>
        /// Bonuses from active hours. Inactive hours is every hour that has no speed data between 09 and 18
        /// </summary>
        /// <param name="nrOfInactiveHours"></param>
        public void SetConstructionBonusesFromActiveHours()
        {
            List<SpeedModel> speedModels = apiHandler.GetSpeedModels();  //Gets speeds for 9 hours yesterday. If every hour has been active speedModels.Length should be 9
            int nrOfInactiveHours = 9 - speedModels.Count;

            if (nrOfInactiveHours == 9)
            {
                healthCareCostBonus = 1.4f;
            }
            else if (nrOfInactiveHours < 9 && nrOfInactiveHours >= 5)
            {
                healthCareCostBonus = 1.1f;
            }
            else
            {
                healthCareCostBonus = 0.75f;
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
