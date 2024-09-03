using VisionMultiArea;
using System;
using OpenCvSharp;

namespace VisionMultiArea
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Vision());
        }

        public static void Main2()
        {


        }

        static SAMLightConfig GetConfig()
        {
            SAMLightConfig samLightConfig = new SAMLightConfig();

            samLightConfig.LayoutFolder = ""; // TODO: Folder einfügen
            samLightConfig.DesignScaps = false; // TODO: richtigen bool einfügen

            return samLightConfig;
        }
    }
}