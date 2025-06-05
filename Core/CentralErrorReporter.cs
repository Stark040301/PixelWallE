using System;
using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Parser;

namespace PixelWallE.Core;

public static class CentralErrorReporter
{
    public static bool HadError { get; set; }
    public static bool HadRuntimeError { get; set; }
    public static class ExitCodes
    {
        public const int Success = 0;
        public const int SyntaxError = 65;
        public const int RuntimeError = 70;
    }
    public static int GetExitCode()
    {
        if (HadError) return ExitCodes.SyntaxError;
        if (HadRuntimeError) return ExitCodes.RuntimeError;
        return ExitCodes.Success;
    }

    public static void Reset()
    {
        HadError = false;
        HadRuntimeError = false;
    }
}