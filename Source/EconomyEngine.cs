using ColossalFramework.Plugins;
using System;
using ICities;


namespace CitiesConext
{
    class EconomyEngine
    {
        public int TaxMultiplier { get; private set; }

        public void SetMoneyAmount(int amount)
        {
            long currentAmount;
            try
            {
                var type = typeof(EconomyManager);
                var cashAmountField = type.GetField("m_cashAmount", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                long.TryParse(cashAmountField.GetValue(EconomyManager.instance).ToString(), out currentAmount);
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Test");
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, currentAmount.ToString());

                cashAmountField.SetValue(EconomyManager.instance, (amount + currentAmount) * 100);
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
                //for (int i = 0; i < length; i++)
                //{

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
            this.TaxMultiplier = multiplier;
        }


    }
}
