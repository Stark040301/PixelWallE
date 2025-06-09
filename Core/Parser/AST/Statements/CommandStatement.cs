/*using System.Collections.Generic;
using PixelWallE.Core.Lexer;
using PixelWallE.Core.Parser.AST.Expressions;
namespace PixelWallE.Core.Parser.AST.Statements;

public class CommandStatement : Statement
{
    public string CommandName { get; }
    public List<Expression> Arguments { get; }

    public CommandStatement(Token commandToken, List<Expression> arguments)
    {
        CommandName = commandToken.Lexeme;
        Arguments = arguments;
    }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}*/