using Avalonia;
using System;
using System.Collections.Generic;
using PixelWallE.Core;
using PixelWallE.Core.Common;
using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser;
using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Tests;

namespace PixelWallE;

class Program
{
    public static void Main(string[] args)
    {
        CentralErrorReporter.Reset();
    }
}