using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VisionMultiArea
{
    public class uEyeCamera
    {
        private uEye.Camera _camera;
        private double _exposureTime = 32.0;
        int ID = 0;

        public uEyeCamera(int? id)
        {
            try
            {
                _camera = new uEye.Camera();
            }
            catch (Exception)
            {
                throw new Exception("Unable to connect to the Camera with the specified ID");
            }
            if (id != null)
            {
                ID = (int)id;
            }
            else
            {
                var connected = ConnectedCameras();
                if (connected > 1)
                {//since at least one is connected i can look for its ID
                    try
                    {
                        ID = GetCameraList();

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }


            if (!Init())
            {
                throw new Exception("Unable to connect to the Camera");
            }
        }

        public Bitmap TakeImage()
        {
            uEye.Defines.Status cameraStatus = _camera.Acquisition.Freeze(uEye.Defines.DeviceParameter.Wait);
            if (cameraStatus != uEye.Defines.Status.Success)
            {
                return null;
            }

            cameraStatus = _camera.Memory.CopyToArray(1, out byte[] rgbArray);
            //cameraStatus = _camera.Memory.CopyToBitmap(1, out Bitmap bitmap);
            if (cameraStatus != uEye.Defines.Status.Success)
            {
                throw new Exception("Camera reported acquisition error {0}" + cameraStatus);
            }
            //TODO CHECK: CONVERT BITMAP WITH THE SAME FORMAT FOR OPENCV
            return RGBArray2Bitmap(rgbArray, 1280, 1024);
            //return bitmap;
        }

        public static Bitmap RGBArray2Bitmap(byte[] buffer, int width, int height)
        {
            Bitmap b = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle BoundsRect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            // Add back dummy bytes between lines, make each line be a multiple of 4 bytes
            int skipByte = bmpData.Stride - width * 3;
            byte[] newBuff = new byte[buffer.Length + skipByte * height];
            for (int j = 0; j < height; j++)
            {
                Buffer.BlockCopy(buffer, j * width * 3, newBuff, j * (width * 3 + skipByte), width * 3);
            }

            // Fill in rgbValues
            Marshal.Copy(newBuff, 0, ptr, newBuff.Length);
            b.UnlockBits(bmpData);

            return b;
        }

        private bool Init()
        {

            uEye.Defines.Status cameraStatus;
            //if (ID != 0)
            cameraStatus = _camera.Init(ID);//to test if it cause trouble, since it is called only once, 
                                            //else //it shouldn t, otherwise, DELETE ID AS INPUT PARAMETER
                                            //{
                                            //    cameraStatus = _camera.Init();
                                            //}
            if (cameraStatus != uEye.Defines.Status.Success)
            {
                return false;
            }

            cameraStatus = _camera.Parameter.Load("DPSCameraConfiguration.ini");
            if (cameraStatus != uEye.Defines.Status.Success)
            {
                return false;
            }

            cameraStatus = _camera.Memory.Allocate();
            if (cameraStatus != uEye.Defines.Status.Success)
            {
                return false;
            }

            //Camera parameters are updated after first take, which here is discarded.
            cameraStatus = _camera.Acquisition.Freeze(uEye.Defines.DeviceParameter.Wait);
            if (cameraStatus != uEye.Defines.Status.Success)
            {
                return false;
            }

            return true;
        }

        public int ConnectedCameras()
        {
            uEye.Info.Camera.GetNumberOfDevices(out int s32Value);
            return s32Value;
        }
        public int GetCameraList()
        {
            uEye.Info.Camera.GetCameraList(out uEye.Types.CameraInformation[] CameraList);
            return (int)CameraList[0].CameraID;
        }
    }
}
