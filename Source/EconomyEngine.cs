using ColossalFramework.Plugins;
using System;
using ICities;


namespace CitiesConext
{
    class EconomyEngine
    {

        public void SetMoneyAmount(int amount)
        {
            long currentAmount;

            try
            {
                var type = typeof(EconomyManager);
                var cashAmountField = type.GetField("m_cashAmount", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                long.TryParse(cashAmountField.GetValue(EconomyManager.instance).ToString(), out currentAmount);
                currentAmount = currentAmount / 100;

                cashAmountField.SetValue(EconomyManager.instance, (amount + currentAmount) * 100);
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, e.ToString());
            }
        }


    }
}
