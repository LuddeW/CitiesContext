using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICities;
using UnityEngine;
using ColossalFramework;

namespace CitiesConext.Source
{
    public class ChirperInfoPresenter : ChirperExtensionBase
    {
        GoogleApiHandler apiHandler;
        public override void OnCreated(IChirper c)
        {
            apiHandler = new GoogleApiHandler();
            InfoMessage m = CreateInfoMessage();
            AddMessage(m);
        }
        public void AddMessage(InfoMessage message)
        {
            MessageManager.instance.QueueMessage(message);
        }
        //REPETITION
        public InfoMessage CreateInfoMessage()
        {
            int steps = apiHandler.GetSteps();
            int taxMultiplier = (steps < 10000) ? 1 : ((steps % 10000) / 1000) + 1;
            float healthCareCostBonus, electricityCostBonus, educationCostBonus = 1;
            if (steps < 5000)
            {
                educationCostBonus = 1.7f;
                healthCareCostBonus = 1.7f;
                electricityCostBonus = 1.7f;
            }
            else if (steps >= 5000 && steps < 10000)
            {
                educationCostBonus = 1f;
                healthCareCostBonus = 1f;
                electricityCostBonus = 1f;
            }
            else if (steps >= 10000 && steps < 20000)
            {
                educationCostBonus = 0.6f;
                healthCareCostBonus = 0.6f;
                electricityCostBonus = 0.6f;
            }
            else
            {
                educationCostBonus = 0.3f;
                healthCareCostBonus = 0.3f;
                electricityCostBonus = 0.3f;
            }

            InfoMessage m = new InfoMessage(
                "Step-master",
                "You have taken " + steps + " steps which gives following rewards: Start bonus = " + (steps * 5) + " Building cost = " + educationCostBonus + "x original cost and the tax multiplier is set to " + taxMultiplier 
                );
            return m;
        }
    }
}
