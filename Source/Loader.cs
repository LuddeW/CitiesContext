using ColossalFramework.Plugins;
using ICities;
using System.Collections.Generic;

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
            QueueStartUpMessages(steps);

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

        void QueueStartUpMessages(int steps)
        {
            List<SpeedModel> a = googleApiHandler.GetSpeedModels();
            int inactiveHours = 9 - a.Count;
            int topSpeed = 0;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].speeds[1] > topSpeed)
                {
                    topSpeed = (int)a[i].speeds[1];
                }
            }

            string speedMessage = "";

            if (inactiveHours == 9)
            {
                speedMessage += "You didn't move at all yesterday! You'll be looking at some pretty hight Helath care costs today...    The cost of health care buildings is increase by 1.7x";
            }
            else if (inactiveHours < 9 && inactiveHours > 5)
            {
                speedMessage += "You should try to move more. You seem to have been sitting for " + inactiveHours + " yesterday     The cost of health care buildings will be increased by 1.3x";
            }
            else if (inactiveHours <= 5)
            {
                speedMessage += "Wow! An active day yesterday. You only sat still for about " + inactiveHours + " yesterday.     The cost of health care buildings are now 0.75 of the original cost!";
            }

            speedMessage += "Your top speed yesterday was " + topSpeed + " m/s";

            if (topSpeed < 1)
            {
                speedMessage += ". You are kind of a slow walker.";
            }
            else if (topSpeed >= 1 && topSpeed < 2)
            {
                speedMessage += " I guess you were not in a hurry?";
            }
            else if (topSpeed >= 2 && topSpeed < 4)
            {
                speedMessage += " That's a good pace!";
            }
            else if (topSpeed >= 4)
            {
                speedMessage += " Looks like you went for a run... Or biking... Or maybe you were sitting still on a slow moving train. I can't know everything, I'm not psycic";
            }


            MessageManager.instance.QueueMessage(new InfoMessage("The intrusive doctor", speedMessage));

            MessageManager.instance.QueueMessage(new InfoMessage("Step-master", "Your total steps: " + steps + " Results from this will be etc etc etc..."));
        }
    }
}
