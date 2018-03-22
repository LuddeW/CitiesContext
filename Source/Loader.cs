using ColossalFramework.Plugins;
using ICities;

namespace CitiesConext
{
    public class Loader : LoadingExtensionBase
    {
        GoogleApiHandler googleApiHandler;

        EconomyEngine economyEngine;

        public override void OnLevelLoaded(LoadMode mode)
        {
            googleApiHandler = new GoogleApiHandler();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, googleApiHandler.GetSteps().ToString());
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, googleApiHandler.GetAvgSteps().ToString());
            int steps = googleApiHandler.GetSteps();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, steps.ToString());
            InitializeEconomyBonuses(steps);

            base.OnLevelLoaded(mode); //Needed?
        }

        void InitializeEconomyBonuses(int steps)
        {
            economyEngine = new EconomyEngine();

            economyEngine.SetMoneyAmount(CalculateCashBonus(steps));
            
        }

        int CalculateCashBonus(int steps)
        {
            if (steps < 10000)
            {
                return 0;
            }
            else
            {
                return steps * 5;
            }
        }
    }
}
