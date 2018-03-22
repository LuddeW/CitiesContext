using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICities;
using ColossalFramework.Plugins;

namespace CitiesConext
{
    public class MyEconomyExtension : EconomyExtensionBase
    {
        int taxBonus = 1;
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

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "OnCreate finished");
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
    }
}
