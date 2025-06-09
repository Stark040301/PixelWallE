namespace PixelWallE.Core.Parser.AST.Statements;

public class LabelStatement: Statement
{
    public string Name { get; }

    public LabelStatement(string name)
    {
        Name = name;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.Visit(this);
    }
}