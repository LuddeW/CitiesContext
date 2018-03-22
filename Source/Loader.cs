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
            economyEngine = new EconomyEngine();
            economyEngine.SetMoneyAmount(100000);
            economyEngine.SetTaxMultiplier(1000000);

            base.OnLevelLoaded(mode); //Needed?
        }
    }
}
