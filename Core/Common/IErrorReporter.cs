using PixelWallE.Core.Evaluator.Runtime;
using PixelWallE.Core.Lexer;

namespace PixelWallE.Core.Common;

public interface IErrorReporter
{
    void ReportSyntaxError(Token token, string message);
    void ReportRuntimeError(RuntimeError error);
}