namespace GXCompiler.GXParser;

public abstract class AstNode {}
public abstract class StatementNode : AstNode {}
public abstract class ExpressionNode : AstNode {}

// Program
public class ProgramNode : AstNode
{
    public EntrypointNode? Entrypoint = null;
    public List<ClassNode> Classes = [];
    public List<StatementNode> Statements { get; set; } = [];
}

// Entrypoint
public class EntrypointNode : StatementNode
{
    public BlockNode Body { get; set; } = new();
}

// Class
public class ClassNode : StatementNode
{
    public BlockNode Body { get; set; } = new();
}

// Block
public class BlockNode : StatementNode
{
    public List<StatementNode> Statements { get; set; } = [];
}

// Variable declaration
public class VariableDeclarationNode(string type, string name, ExpressionNode expression) : StatementNode
{
    public string Type { get; set; } = type;
    public string Name { get; set; } = name;
    public ExpressionNode Value { get; set; } = expression;
}

// Output
public class OutNode : StatementNode
{
    public ExpressionNode? Value { get; set; }
}

// Expressions
public class LiteralNode : ExpressionNode
{
    public object? Value { get; set; }
}

public class IdentifierNode(string name) : ExpressionNode
{
    public string Name { get; set; } = name;
}
