using DPSEasyaufWish.ScapsConstants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.NetworkInformation;
//using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace DPSEasyaufWish
{
    public enum ShowMode
    {
        Hide = 0,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Show = 5,
        Restore = 9
    };

    public enum OutlinePoint
    {
        MinX = 0,
        MinY = 1,
        MinZ = 2,
        MaxX = 3,
        MaxY = 4,
        MaxZ = 5
    };

    [Flags]
    public enum MarkingFlags
    {
        None = 0,
        WaitForTrigger = 1,
        HideOutput = 4,
        DisableHomeJump = 8,
        Preview = 16,
        MarkSelected = 32,
        DisablePreProcessing = 64,
        DisablePostProcessing = 128,
        ControlLoopByEntity = 1024,
        CloseTriggerWindow = 2048,
        DontSwitchToNextLayer = 32768,
        UseAlreadyOpenedMarkDialog = 16384
    };


    public class PixelZones
    {
        public int Zone0 { get; set; }
        public int Zone1 { get; set; }
        public int Zone2 { get; set; }
        public int Zone3 { get; set; }
        public int Zone4 { get; set; }
        public int Zone5 { get; set; }
    }

    [Flags]
    public enum Modes
    {
        None = 0,
        TopLevelOnly = 1,
        DontUpdateView = 2,
        DisableFileCompression = 16,
        EntityNamesSeparatedBySemicolon = 32,
        DisableUndo = 16384
    };

    [Flags]
    public enum ImportFlags : uint
    {
        None = 0,
        KeepOrder = 8,
        ReadPenInfo = 128,
        GCodeLoadPenParameters = 8192,
        GCodeLoadOpticParameters = 16384,
        PointCloud = 16384, //Manual seems wrong here
        NotTryToClose = 131072,
        Optimized = 524288,
        BitmapReimport = 1048576,
        DontUpdateView = 2097152,
        VectorReimport = 8388608,
        TopLevelOnly = 16777216,
        FillWithDefaultHatchStyle = 33554432,
        NoErrorMsg = 67108864,
        CreateOneGroup = 134217728,
        CenterToField = 268435456,
        ImportToPenGroups = 536870912,
        UsePenColors = 1073741824,
        Protected = 2147483648
    };

    [Flags]
    public enum ImportType
    {
        PLT,
        DXF,
        TXT,
        BMP,
        ALL
    };

    public enum LongValueId
    {
        TotalEntityNum = 10,
        TopLevelEntityNum = 11,
        OpticAxisState = 110
    }

    public enum DoubleValueType
    {
        OffsetX = 58,
        OffsetY = 59,

        FieldMinX = 119,
        FieldMinY = 120,
        FieldMaxX = 121,
        FieldMaxY = 122
    }

    public enum StringValueType
    {
        JobFileName = 4,
        CorrectionFileLCF = 29,
    }

    public enum OpticAxisState
    {
        Normal = 0,
        XAxisInverted = 1,
        YAxisInverted = 2,
        XYAxisInverted = 3,
        XYAxisFlipped = 4,
        Invalid = -1
    }

    public enum EntityMirrorMode
    {
        XZField = 1,
        YZField = 2,
        XYField = 3,
        XZEntity = 4,
        YZEntity = 5,
        XYEntity = 6
    }

    public enum StringDataID
    {
        TopLevelEntity = 17, // it provides only the first level entity name (i.e., group)
        SingleEntity = 21,
        TypeEntity = 22,
        TypeAsSamLigthEntity = 35,
    }

    public class CommandBuilder
    {
        public static string GetInterfaceVersion()
        {
            return "ScCciGetInterfaceVersion()\n";
        }

        public static string Shutdown()
        {
            return "ScCciShutdown()\n";
        }

        public static string ShowApp(ShowMode showMode)
        {
            return string.Format("ScCciShowApp({0})\n", (int)showMode);
        }

        public static string GetWorkingArea(OutlinePoint outlinePoint)
        {
            return string.Format("ScCciGetWorkingArea({0})\n", outlinePoint);
        }

        public static string OpticMatrixTranslate(double xTranslation, double yTranslation, double zTranslation)
        {
            return string.Format("ScCciOpticMatrixTranslate({0},{1},{2}\n", xTranslation, yTranslation, zTranslation);
        }

        public static string OpticMatrixRotate(double xCenter, double yCenter, double rotationAngleRadians)
        {
            return string.Format("ScCciOpticMatrixRotate({0},{1},{2}\n", xCenter, yCenter, rotationAngleRadians);
        }

        public static string OpticMatrixScale(double xScale, double yScale)
        {
            return string.Format("ScCciOpticMatrixScale({0},{1})\n", xScale, yScale);
        }

        public static string OpticMatrixReset()
        {
            return "ScCciOpticMatrixReset()\n";
        }

        public static string GetOpticMatrix(OutlinePoint outlinePoint)
        {
            return string.Format("ScCciGetOpticMatrix({0})\n", outlinePoint);
        }

        public static string SetHead(int headIdentifier)
        {
            return string.Format("ScCciSetHead({0})\n", headIdentifier);
        }

        public static string GetHead()
        {
            return "ScCciGetHead()\n";
        }

        public static string IsMarking()
        {
            return "ScCciIsMarking()\n";
        }

        public static string StopMarking()
        {
            return "ScCciStopMarking()\n";
        }

        public static string SetMarkFlags(MarkingFlags markingFlags)
        {
            return string.Format("ScCciSetMarkFlags({0})\n", (int)markingFlags);
        }

        public static string SetMode(Modes modes)
        {
            return string.Format("ScCciSetMode({0})\n", (int)modes);
        }

        public static string GetMarkFlags()
        {
            return "ScCciGetMarkFlags()\n";
        }

        public static string SwitchLaser(bool on)
        {
            return string.Format("ScCciSwitchLaser({0})\n", on ? "1" : "0");
        }

        public static string AbsoluteMove(double xPosition, double yPosition, double zPosition)
        {
            return string.Format("ScCciMoveAbs({0},{1},{2})\n", xPosition, yPosition, zPosition);
        }

        public static string SetPen(int penIdentifier)
        {
            return string.Format("ScCciSetPen({0})\n", penIdentifier);
        }

        public static string GetPen()
        {
            return "ScCciGetPen()\n";
        }

        public static string ResetSequence()
        {
            return "ScCciResetSequence()\n";
        }

        public static string ResetCounter()
        {
            return "ScCciResetCounter()\n";
        }

        public static string ResetSerialNumbers()
        {
            return "ScCciResetSerialNumbers()\n";
        }

        public static string IncrementSerialNumbers()
        {
            return "ScCciIncSerialNumbers()\n";
        }

        public static string DecrementSerialNumbers()
        {
            return "ScCciDecSerialNumbers()\n";
        }

        public static string ResplitJob()
        {
            return "ScCciResplitJob()\n";
        }

        public static string SetPenPixelMap(int penIdentifier, PixelZones pixelZones)
        {
            return string.Format("ScCciSetPixelMapForPen({0},{1},{2},{3},{4},{5},{6})\n",
                penIdentifier,
                pixelZones.Zone0,
                pixelZones.Zone1,
                pixelZones.Zone2,
                pixelZones.Zone3,
                pixelZones.Zone4,
                pixelZones.Zone5);
        }

        public static string MarkEntityByName(string entityName, bool waitForMarkEnd)
        {
            return string.Format("ScCciMarkEntityByName(\"{0}\",{1})\n", entityName, waitForMarkEnd ? "1" : "0");
        }

        public static string ChangeEntityTextByName(string entityName, string entityText)
        {
            return string.Format("ScCciChangeTextByName(\"{0}\",\"{1}\")\n", entityName, entityText);
        }

        public static string ChangeEntitiesTextByNames(List<string> entityNames, List<string> entityValues)
        {
            string names = String.Join(";", entityNames);
            string values = String.Join("\v", entityValues);
            return ChangeEntityTextByName(names, values);
        }

        public static string Import(string entityName, string fileName, ImportType fileType, double resolution, ImportFlags importFlags)
        {
            return string.Format("ScCciImport(\"{0}\",\"{1}\",\"{2}\",{3},{4})\n", entityName, fileName, fileType, resolution.ToString(System.Globalization.CultureInfo.InvariantCulture), (int)importFlags);
        }

        public static string LoadJob(string jobFileName, bool loadEntities, bool overwriteEntities, bool loadMaterials)
        {
            return string.Format("ScCciLoadJob(\"{0}\",{1},{2},{3})\n",
                jobFileName,
                loadEntities ? "1" : "0",
                overwriteEntities ? "1" : "0",
                loadMaterials ? "1" : "0");
        }

        public static string GetEntityOutline(string entityName, OutlinePoint outlinePoint)
        {
            return string.Format("ScCciGetEntityOutline(\"{0}\",{1})\n", entityName, (int)outlinePoint);
        }

        public static string TranslateEntity(string entityName, double xPosition, double yPosition, double zPosition)
        {
            return string.Format("ScCciTranslateEntity(\"{0}\",{1},{2},{3})\n", entityName, xPosition.ToString(System.Globalization.CultureInfo.InvariantCulture), yPosition.ToString(System.Globalization.CultureInfo.InvariantCulture), zPosition.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public static string ScaleEntityCurrentPosition(string entityName, double xScale, double yScale, double zScale)
        {
            return string.Format("ScCciScaleEntity(\"{0}\", {1},{2},{3})\n", entityName, xScale.ToString(System.Globalization.CultureInfo.InvariantCulture), yScale.ToString(System.Globalization.CultureInfo.InvariantCulture), zScale.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public static string SetEntityDoubleData(string entityName, DoubleDataId dataId, double data)
        {
            return string.Format(CultureInfo.InvariantCulture, "ScCciSetEntityDoubleData(\"{0}\",{1},{2})\n", entityName, (long)dataId, data);
        }

        public static string GetEntityDoubleData(string entityName, DoubleDataId dataId)
        {
            return string.Format("ScCciGetEntityDoubleData(\"{0}\",{1})\n", entityName, (long)dataId);
        }

        public static string GetEntityStringData(string entityName, EntityStringDataId clientCtrlFlag)
        {
            return string.Format($"ScCciGetEntityStringData(\"{entityName}\",{(int)clientCtrlFlag})\n");
        }

        public static string GetEntityLongData(string entityName, LongDataId dataId)
        {
            return string.Format("ScCciGetEntityLongData(\"{0}\",{1})\n", entityName, (long)dataId);
        }

        public static string SetEntityLongData(string entityName, LongDataId dataId, long data)
        {
            return string.Format("ScCciSetEntityLongData(\"{0}\",{1}, {2})\n", entityName, (long)dataId, data);
        }

        public static string SetLongValue(LongValueId longValue, long mode)
        {
            return string.Format("ScCciSetLongValue({0}, {1})\n", (int)longValue, (int)mode);
        }

        public static string GetLongValue(LongValueId longValueId)
        {
            return string.Format("ScCciGetLongValue({0})\n", (int)longValueId);
        }

        public static string SetRotationEntity(string entityName, double xCenter, double yCenter, double angle)
        {
            return string.Format(CultureInfo.InvariantCulture, "ScCciRotateEntity(\"{0}\", {1}, {2}, {3})\n", entityName, xCenter, yCenter, angle);
        }

        public static string GetDoubleValue(DoubleValueType doubleValueType)
        {
            return string.Format("ScCciGetDoubleValue({0})\n", (int)doubleValueType);
        }

        public static string SetDoubleValue(DoubleValueType doubleFlag, double doubleValue)
        {
            return string.Format("ScCciSetDoubleValue({0},{1})\n", (int)doubleFlag, doubleValue);
        }

        public static string MoveAbs(double xCoor, double yCoor)
        {
            return string.Format("ScCciMoveAbs({0},{1},0)\n", xCoor, yCoor);
        }

        public static string SetStringValue(long type, string val)
        {
            return string.Format("ScCciSetStringValue({0},\"{1}\")\n", type, val);
        }

        public static string GetStringValue(long type)
        {
            return string.Format("ScCciGetStringValue({0})\n", type);
        }

        public static string GetEntityList(StringDataID stringData, long index)
        {
            return string.Format("ScCciGetIDStringData({0},{1})\n", (int)stringData, index);
        }
    }
}

