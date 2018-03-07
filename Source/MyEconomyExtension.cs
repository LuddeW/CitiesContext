using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICities;
using ColossalFramework.Plugins;

namespace CitiesConext.Source
{
    public class MyEconomyExtension : EconomyExtensionBase
    {
        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Hello from here!");
           // DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Hello from onupdatemoneyamount");
            //Still unsure how tax multiplier works
            try
            {
                var type = typeof(EconomyManager);
                var taxMultiplier = type.GetField("m_taxMultiplier", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                taxMultiplier.SetValue(EconomyManager.instance, 1000000);
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, e.ToString());
            }
            return base.OnUpdateMoneyAmount(internalMoneyAmount);
        }
    }
}
