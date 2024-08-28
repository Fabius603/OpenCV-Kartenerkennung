using DPSEasyaufWish.ScapsConstants;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPSEasyaufWish
{
    public class Laser
    {
        private SAMLightBasics _samLightBasics;

        public Laser(SAMLightBasics samLightBasics)
        {
            _samLightBasics = samLightBasics;
        }

        public bool TranslateEntity(string entityName, double xPosition, double yPosition)
        {
            if (_samLightBasics.ExecuteCommand(CommandBuilder.TranslateEntity(entityName, xPosition, yPosition, 0), out string response))
            {
                if (response[0] == '1')
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public bool SetEntityRotationAngle(string entityName, double angle)
        {
            if (_samLightBasics.ExecuteCommand(CommandBuilder.SetEntityDoubleData(entityName, DoubleDataId.RotationAngle, angle), out string response))
            {
                if (response[0] == '1')
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
}
