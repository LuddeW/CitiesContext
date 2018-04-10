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
            if (mode == LoadMode.NewGame)
            {
                MessageManager.instance.QueueMessage(new InfoMessage("Simon & Ludwig", "Since this is a new game you'll have some starting cash bonus if you're above 10000 steps during the last 24h and you did" + steps + ". Your bonus is " + CalculateCashBonus(steps)));
            }
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
            BuildAndSendSpeedMessage();
            BuildAndSendStepMessage(steps);
        }

        void BuildAndSendSpeedMessage()
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
                speedMessage += "You didn't move at all yesterday! You'll be looking at some pretty hight Helath care costs today...    The cost of health care buildings is increase by 1.4x";
            }
            else if (inactiveHours < 9 && inactiveHours > 5)
            {
                speedMessage += "You should try to move more. You seem to have been sitting for " + inactiveHours + " hours yesterday     The cost of health care buildings will be increased by 1.1x";
            }
            else if (inactiveHours <= 5)
            {
                speedMessage += "Wow! An active day yesterday. You only sat still for about " + inactiveHours + " hours yesterday.     The cost of health care buildings are now 0.75 of the original cost!";
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
        }

        void BuildAndSendStepMessage(int steps)
        {
            string message = "Your total step count during the last 24h is " + steps;

            if (steps < 5000)
            {
                message += ". Scientists say that you should do 10000 so save some money buy running behind a bus! The price of Electricity- and Education buildings are increased by 1.3x";
            }
            else if (steps >= 5000 && steps < 10000)
            {
                message += ". Your total goal should be 10000. Do more and get some sweet deals for Education- and Electricity buildings. As of now they remain at the original price.";
            }
            else if (steps >= 10000 && steps < 20000)
            {
                message += ". Well done! You're over 10000 steps. Prices for Education- and Electricity buildings are down by 30%";
            }
            else
            {
                message += "  WOW! That is a lot of walking. The price of Education- and Electricity buldings are down by 60%! Enjoy!!!";
            }

            MessageManager.instance.QueueMessage(new InfoMessage("Step-master", message));

            int avgSteps = googleApiHandler.GetAvgSteps();

            string avgMessage = "Your avarage step count during the last three days is " + avgSteps + ". Your avarage step count effect how much tax income your housing, industries, offices and stores generate, so keep taking those steps and make more $$$";
            MessageManager.instance.QueueMessage(new InfoMessage("Avarage steps", avgMessage));
        }
    }
}
