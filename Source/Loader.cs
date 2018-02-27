using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColossalFramework;
using ICities;

namespace CitiesConext.Source
{
    public class Loader : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            GoogleApiHandler googleApiHandler = new GoogleApiHandler();
            googleApiHandler.SendRefreshTokenRequestRequest();
        }
    }
}
