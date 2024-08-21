using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAMLightClientCtrl;

namespace DPSEasyaufWish
{
    public class Laser
    {
        private SAMLightClientCtrl _samlight;

        public void CreateLaser()
        {
            SAMLightClientCtrl _samlight = new SAMLightClientCtrl();
            _samlight.Connect();
        }
    }
}
