using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;
namespace PixelWallE.Core.Parser.AST.Statements;

public class VarDecl : Statement
{
    public string VariableName { get; }
    public Expression Initializer { get; }

    public VarDecl(Token identifier, Expression initializer)
    {
        VariableName = identifier.Lexeme;
        Initializer = initializer;
    }

    public override void Accept(IStatementVisitor statementVisitor)
   {
       statementVisitor.Visit(this);
   }
}