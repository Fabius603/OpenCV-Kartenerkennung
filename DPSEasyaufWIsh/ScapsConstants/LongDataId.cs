﻿//using System.Runtime.InteropServices.WindowsRuntime;

namespace DPSEasyaufWish.ScapsConstants
{
    public enum LongDataId
    {
        MirrorOnPlane = 120,
        FlagDontUpdateView = 0x10000,
        FlagDontUpdateEntity = 0x20000,
        FlagToplevelOnly = 0x200000,
        FlagEnqueueCtrlCmd = 0x80000,
        FlagEnqueueLastCtrlCmd = 0x100000,
        UserData = 1,
        TextAlignment = 2,
        TextAlignmentCenter = 0x1,
        TextAlignmentLeft = 0x2,
        TextAlignmentRight = 0x4,
        TextAlignmentTop = 0x8,
        TextAlignmentBottom = 0x10,
        TextAlignmentMiddle = 0x20,
        TextAlignmentRadialCenter = 0x40,
        TextAlignmentRadialEnd = 0x80,
        TextAlignmentLineLeft = 0x100,
        TextAlignmentLineRight = 0x200,
        TextAlignmentLineCenter = 0x400,
        EntitySelected = 3,
        EntityArrayCountX = 4,
        EntityArrayCountY = 5,
        EntityArrayStepX = 6,
        EntityArrayStepY = 7,
        EntityArrayOrderFlags = 8,
        EntityArrayOrderFlagNegX = 0x100,
        EntityArrayOrderFlagNegY = 0x200,
        EntityArrayOrderFlagMainDirX = 0x400,
        EntityArrayOrderFlagBiDir = 0x800,
        TextCharFlags = 9,
        TextCharFlagMonoSpaced = 0x3,
        TextCharFlagItalic = 0x10000,
        TextCharFlagRadial = 0x20000,
        TextCharFlagRadialAlignToCharOutline = 0x40000,
        TextCharFlagReverseOrder = 0x80000,
        TextCharFlagMirrorCharOnXAxis = 0x100000,
        TextCharFlagMirrorCharOnYAxis = 0x200000,
        TextCharFlagSwapLines = 0x400000,
        TextCharFlagSetToLimitLength = 0x800000,
        TextCharFlagSetToLimitHeight = 0x1000000,
        TextCharFlagSetToLimitKeepAspect = 0x2000000,
        TextCharFlagOrderXDown = 0x4000000,
        TextCharFlagOrderYUp = 0x8000000,
        TextCharFlagOrderYMainUp = 0x10000000,
        TextCharFlagOrderBiDir = 0x20000000,
        TextCharFlagRadialCenterMode = 0x40000000,
        TextFontAvailable = 10,
        BitmapMode = 49,
        BitmapModeInvert = 0x1,
        BitmapModeGreyscale = 0x2,
        BitmapModeDrillmode = 0x4,
        BitmapModeBidirectional = 0x8,
        BitmapModeStartlastline = 0x10,
        BitmapModeNolineincr = 0x20,
        BitmapModeShowBitmap = 0x100,
        BitmapModeShowScanner = 0x200,
        BitmapModeScanXDir = 0x400,
        BitmapModePenFrequency = 0x800,
        BitmapModeJumpOverBlankPixels = 0x1000,
        BitmapModeDrillGreyscale = 0x2000,
        TextWeight = 50,
        scComCharWeightThin = 0x64,
        scComCharWeightExtraLight = 0xC8,
        scComCharWeightLight = 0x12C,
        scComCharWeightNormal = 0x190,
        scComCharWeightMedium = 0x1F1,
        scComCharWeightSemiBold = 0x258,
        scComCharWeightBold = 0x2BC,
        scComCharWeightExtraBold = 0x320,
        scComCharWeightHeavy = 0x384,
        EnableHatching1 = 51,
        EnableHatching2 = 52,
        EntityMarkLoopCount = 55,
        EntityMarkBeatCount = 56,
        EntityMarkStartCount = 57,
        EntityMarkFlags = 58,
        EntityMarkFlagMarkContour = 0x1,
        EntityMarkFlagMarkHatch = 0x2,
        EntitySetPen = 60,
        EntitySetTimerValue = 61,
        EntitySetInOutValue = 62,
        EntitySetOutputPulse = 64,
        EntitySetInOutLevel = 65,
        EntityGetTimerValue = 66,
        EntityGetInOutValue = 67,
        EntityGetOutputPulse = 69,
        EntityGetInOutLevel = 70,
        EntitySerialStartValue = 71,
        EntitySerialIncrValue = 72,
        EntitySerialCurrValue = 73,
        EntityGetPen = 74,
        EntityOpticFlags = 75,
        EntityOpticFlagContour = 0x1,
        EntityOpticFlagHatch = 0x2,
        EntitySplittable = 76,
        EntitySerialNumLines = 77,
        EntitySetAsBackgroundEntity = 78,
        EntitySerialBeatCount = 79,
        EntitySerialResetCount = 80,
        SetHatchFlags1 = 81,
        SetHatchFlags2 = 82,
        ClearHatchFlags1 = 91,
        ClearHatchFlags2 = 92,
        HatchFlagNoSort = 0x100,
        HatchFlagAllLines = 0x400,
        HatchFlagPolyLineBeamComp = 0x2000,
        HatchFlagDontFillRest = 0x4000,
        HatchFlagKeepAngle = 0x80000,
        HatchFlagEqualizeDistance = 0x1000000,
        HatchFlagBeamCompLoopReverseOrder = 0x2000000,
        BarcodeSetFlags = 101,
        BarcodeClearFlags = 102,
        BarcodeFlagVariableLength = 0x1,
        BarcodeFlagInvert = 0x2,
        BarcodeFlagDisableAutoQuietZone = 0x4,
        BarcodeFlagQuietZoneAbsolute = 0x8,
        BarcodeFlagGenerateCheckCode = 0x10,
        BarcodeFlagInvertExceptText = 0x20,
        BarcodeFlagInvertCellMode = 0x40,
        BarcodeFlagCompactMode = 0x80,
        DataMatrixSetSymbolMode = 103,
        DataMatrixClearSymbolMode = 104,
        DataMatrixExSymbolModeRectangle = 0x1,
        DataMatrixExSymbolModeAutoSize = 0x10000,
        DataMatrixExSymbolModeAutoEncodation = 0x20000,
        DataMatrixExSymbolModeDots = 0x40000,
        DataMatrixExSymbolModeTilde = 0x80000,
        DataMatrixExSymbolModeCells = 0x100000,
        DataMatrixExSymbolModeNoFinderCells = 0x200000,
        DataMatrixExSymbolModeEllipse = 0x400000,
        DataMatrixExSymbolModeTextFreelyEditable = 0x800000,
        DataMatrixSymbolSize = 105,
        DataMatrixEncoding = 106,
        DataMatrixExEncodationAscii = 0x1,
        DataMatrixExEncodationBase256 = 0x2,
        DataMatrixExEncodationC40 = 0x3,
        DataMatrixExEncodationText = 0x4,
        DataMatrixExEncodationAnsiX12 = 0x5,
        DataMatrixExEncodationEdifact = 0x6,
        DataBarcodeTextEnable = 107,
        DataBarcodeLevel = 108,
        DataBarcodeMode = 109,
        DataBarcodeSize = 110,
        EntityLayerCount = 111,
        EntitySetAsHiddenEntity = 112,
        SpiralNumInnerRotations = 113,
        SpiralNumOuterRotations = 114,
        SpiralNumOuterSegments = 115,
        SpiralFlags = 116,
        Clockwise = 0x1,
        StartFromOuter = 0x2,
        SetReturnPath = 0x4,
        SerialNumModesFlags = 117,
        SerialNumber2DModeText = 0x1,
        SerialNumber2DModeBarCode = 0x2,
        SerialNumber2DModeASCIIFile = 0x4,
        SerialNumber2DModeDateTime = 0x8,
        SerialNumber2DModeCustomFormat = 0x10,
        EntityGroupPenPaths = 118,
        EntityGroupCluster = 119,
        EntityMirrorOnPlane = 120,
        EntityOutputAsBitmap = 121,
        BitmapBlankThreshold = 122,
        BitmapLineIndexStep = 123,
        DataMatrixNumberOfDots = 124,
        EntityGenerate = 125,
        EnableControl = 126,
        QrCodeExSetMode = 127,
        QrCodeExClearMode = 128,
        QrCodeExModeDots = 0x1,
        QrCodeExModeCells = 0x100000,
        QrCodeExModeTextFreelyEditable = 0x800000,
        EntityCenter = 129,
        EntityChangeable = 130,
        EntityRedpointer = 131,
        EntityJump = 132,
        EntitySetToDefaultHatchPair = 133,
        HatchLoop1 = 134,
        HatchLoop2 = 135,
        HatchLayerSolidNumLoops = 136,
        HatchLayerSolidPointOffset = 137,
        BarcodeFlagsHigh = 138,
        BarcodeFlagsHighLimitLength = 0x1,
        BarcodeFlagsHighLimitHeight = 0x2,
        BarcodeFlagsHighKeepAspect = 0x4,
        BarcodeFlagsHigh_2 = 0x8A,
        HatchLayerSolidBeamCompOutToIn = 139,
        EntityNonMarkableEntity = 140,
        EntityNonEditableEntity = 141,
        EntityNonSplittablePre = 142,
        EntityNonSplittablePost = 143,
        UseMMForSpacing = 144,
        UseMMforLineSpacing = 145,
        EntityIoControlObjectMask = 146,
        EntityIoControlObjectStates = 147,
    };
}

