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
            int avgSteps = googleApiHandler.GetSteps();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, avgSteps.ToString());
            InitializeEconomyBonuses(avgSteps);

            base.OnLevelLoaded(mode); //Needed?
        }

        void InitializeEconomyBonuses(int avgSteps)
        {
            economyEngine = new EconomyEngine();

            economyEngine.SetMoneyAmount(CalculateCashBonus(avgSteps));
            
        }

        int CalculateCashBonus(int avgSteps)
        {
            if (avgSteps < 10000)
            {
                return 0;
            }
            else
            {
                return avgSteps * 5;
            }
        }
    }
}
