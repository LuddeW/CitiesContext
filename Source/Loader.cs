using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColossalFramework;
using ICities;
using System.Web.Script.Serialization;

namespace CitiesConext.Source
{
    public class Loader : LoadingExtensionBase
    {
        GoogleApiHandler googleApiHandler;

        EconomyEngine economyEngine;

        public override void OnLevelLoaded(LoadMode mode)
        {
            googleApiHandler = new GoogleApiHandler();
            googleApiHandler.SendRefreshTokenRequestRequest();

            economyEngine = new EconomyEngine();
            economyEngine.SetMoneyAmount(500000);
            economyEngine.SetTaxMultiplier(100);

            base.OnLevelLoaded(mode); //Needed?
        }
    }
}
