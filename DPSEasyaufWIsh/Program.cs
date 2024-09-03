using DPSEasyaufWish;
using OpenCvSharp;
using OpenCvSharp.Features2D;
using System.Diagnostics;
using System.Drawing;

namespace DPSEasyaufWish
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Laser erstellen
            //SAMLightConfig samLightConfig = GetConfig();
            //SAMLightBasics samLightBasics = new SAMLightBasics(samLightConfig);
            //samLightBasics.Initialize();

            //Laser laser = new Laser(samLightBasics);

            //// Kamera macht Bild TODO: Funktionen einfügen
            //int idueye = 1;
            //uEyeCamera ueyeCamera = new uEyeCamera(idueye);
            //Bitmap bitmap = ueyeCamera.TakeImage();
            //Mat fullImage = ImageCalc.ToMat(bitmap);
            Mat fullImage = new Mat("FullImage.bmp");

            // Vision berechnet
            Mat template = new Mat("Template.bmp");
            ResultValues RValues = ImageCalc.CalculateImage(template, fullImage);

            // Laser wird eingestellt
            //laser.MoveLaser(RValues);
            //laser.Engrave();

            Ausgabe(RValues);
        }

        static SAMLightConfig GetConfig()
        {
            SAMLightConfig samLightConfig = new SAMLightConfig();

            samLightConfig.LayoutFolder = ""; // TODO: Folder einfügen
            samLightConfig.DesignScaps = false; // TODO: richtigen bool einfügen

            return samLightConfig;
        }

        static void Ausgabe(ResultValues RValues)
        {
            Console.Clear();

            Console.WriteLine("Zeit: " + RValues.Time);
            Console.WriteLine("Rotation: " + RValues.Rotation + "\n");

            for (var i = 0; i < RValues.CenterPoints.Length; i++)
            {
                Console.WriteLine("Centerpoint " + i + ": " + RValues.CenterPoints[i]);
                Console.WriteLine("OffsetX " + i + ": " + RValues.OffsetX[i]);
                Console.WriteLine("OffsetY " + i + ": " + RValues.OffsetY[i] + "\n");
            }
        }
    }
}