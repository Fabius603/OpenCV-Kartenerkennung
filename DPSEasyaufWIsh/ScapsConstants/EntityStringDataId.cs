using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPSEasyaufWish.ScapsConstants
{
    /// <summary>
    /// http://download.scaps.com/downloads/Software/Programming/Client_Control_Interface/Manual/html/index.html?dll_functions.htm
    /// </summary>
    public enum EntityStringDataId
    {
        FlagDontUpdateView = 0x10000,
        FlagDontUpdateEntity = 0x20000,
        FlagToplevelOnly = 0x200000,

        TextFontName = 1,
        TextText = 2,
        GetToplevelEntity = 17,
        FindEntityWithText = 18,
        SetBarcodeType = 19,
        GetBarcodeType = 20,
        GetEntityName = 21,
        GetEntityType = 22,
        SetEntityName = 23,
        EntitySerialASCIIFileNameExcelFileName = 24,
        SetToplevelEntity = 25,
        SetMotionCtrls = 26,
        SerialNumberFormatString = 27,
        ArrayCopyHard = 28,
        Translate = 29,
        Rotate = 30,
        OutlineAndRotate = 31,
        BarCodeFormatString = 32,
        SetMotionCtrlsString = 33,
        SpecialPenAndMore = 34,
        ExportScannerBmp = 36,
    }
}
