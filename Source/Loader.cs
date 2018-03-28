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
            int steps = googleApiHandler.GetSteps();
            InitializeEconomyBonuses(steps);
            MessageManager.instance.QueueMessage(new InfoMessage("Step-master", "Your total steps: " + steps + " Results from this will be etc etc etc..."));
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
