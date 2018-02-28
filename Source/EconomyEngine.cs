using ColossalFramework.Plugins;
using System;


namespace CitiesConext
{
    class EconomyEngine
    {
        public void SetMoneyAmount(int amount)
        {
            try
            {
                var type = typeof(EconomyManager);
                var cashAmountField = type.GetField("m_cashAmount", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                cashAmountField.SetValue(EconomyManager.instance, amount * 100);
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, e.ToString());
            }
        }

        public void MultiplyIncome(int multiplier)
        {
            try
            {
                var type = typeof(EconomyManager);
                var incomeField = type.GetField("m_income", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                long[] currentIncome = (long[])(incomeField.GetValue(EconomyManager.instance));
                //for (int i = 0; i < currentIncome.Length; i++)
                //{
                //    DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Current income is " + currentIncome[i]);
                //}

                incomeField.SetValue(EconomyManager.instance, 10000);
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, e.ToString());
            }
        }

        public void SetTaxMultiplier(int multiplier)
        {
            //Still unsure how tax multiplier works
            try
            {
                var type = typeof(EconomyManager);
                var cashAmountField = type.GetField("m_taxMultiplier", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                cashAmountField.SetValue(EconomyManager.instance, multiplier);
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, e.ToString());
            }
        }
    }
}
