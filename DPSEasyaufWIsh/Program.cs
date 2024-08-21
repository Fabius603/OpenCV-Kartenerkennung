using DPSEasyaufWish;
using OpenCvSharp;
using OpenCvSharp.Features2D;
using System.Diagnostics;

namespace DPSEasyaufWish
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Mat fullImage = new Mat("InitialImage.bmp");

            ResultValues RValues = ImageCalc.CalculateImage(fullImage);

            Ausgabe(RValues);
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