namespace GXCompiler.GXParser;

public abstract class AstNode
{
    public virtual void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}{GetType().Name}");
    }
}

public abstract class StatementNode : AstNode {}
public abstract class ExpressionNode : AstNode {}

// Program
public class ProgramNode : AstNode
{
    public EntrypointNode? Entrypoint = null;
    public List<ClassNode> Classes = [];
    public List<StatementNode> Statements { get; set; } = [];

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine("ProgramNode");

        List<AstNode> children = [];

        if (Entrypoint != null)
            children.Add(Entrypoint);

        children.AddRange(Classes);
        children.AddRange(Statements);

        for (int i = 0; i < children.Count; i++)
        {
            children[i].Display("", i == children.Count - 1);
        }
    }
}

// Entrypoint
public class EntrypointNode : StatementNode
{
    public BlockNode Body { get; set; } = new();

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}EntrypointNode");

        Body.Display(indent + (isLast ? "    " : "│   "), true);
    }
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

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}BlockNode");

        string childIndent = indent + (isLast ? "    " : "│   ");

        for (int i = 0; i < Statements.Count; i++)
        {
            Statements[i].Display(childIndent, i == Statements.Count - 1);
        }
    }
}

// Comments
public class CommentNode : StatementNode {}

// Variable declaration
public class VariableDeclarationNode(string type, string name, ExpressionNode expression) : StatementNode
{
    public string Type { get; set; } = type;
    public string Name { get; set; } = name;
    public ExpressionNode Value { get; set; } = expression;

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}VariableDeclarationNode");

        string childIndent = indent + (isLast ? "    " : "│   ");

        Console.WriteLine($"{childIndent}├── Type: {Type}");
        Console.WriteLine($"{childIndent}├── Name: {Name}");

        Value?.Display(childIndent, true);
    }
}

// Output
public class OutNode : StatementNode
{
    public ExpressionNode? Value { get; set; }

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}OutNode");

        Value?.Display(indent + (isLast ? "    " : "│   "), true);
    }
}

public class ShoutNode : StatementNode
{
    public ExpressionNode? Value { get; set; }

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}ShoutNode");

        Value?.Display(indent + (isLast ? "    " : "│   "), true);
    }
}

public class ExitNode : StatementNode
{

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}ExitNode");
    }
}

// Expressions
public class LiteralNode(object value) : ExpressionNode
{
    public object Value { get; set; } = value;

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}Literal: {Value}");
    }
}

public class NumberNode(object value) : ExpressionNode
{
    public object Value { get; set; } = value;

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}Number: {Value}");
    }
}

public class BoolLiteralNode(bool value) : ExpressionNode
{
    public bool Value { get; set; } = value;

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}BoolLiteral: {Value}");
    }
}

public class ArrayNode(string type, ExpressionNode[] elements) : ExpressionNode
{
    public string Type = type;
    public ExpressionNode[] Elements = elements;

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}ArrayNode");

        string childIndent = indent + (isLast ? "    " : "│   ");

        Console.WriteLine($"{childIndent}├── Type: {Type}");
        Console.WriteLine($"{childIndent}└── Elements:");

        string elementIndent = childIndent + "    ";

        for (int i = 0; i < Elements.Length; i++)
        {
            Elements[i].Display(
                elementIndent,
                i == Elements.Length - 1
            );
        }
    }
}

public class IdentifierNode(string name) : ExpressionNode
{
    public string Name { get; set; } = name;

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}Identifier: {Name}");
    }
}

public class LeftParenNode : ExpressionNode
{
    public readonly string Value = "(";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}LeftParen");
    }
}

public class RightParenNode : ExpressionNode
{
    public readonly string Value = ")";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}RightParen");
    }
}

public class PlusNode : ExpressionNode
{
    public readonly string Value = "+";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}{Value}");
    }
}

public class MinusNode : ExpressionNode
{
    public readonly string Value = "-";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}{Value}");
    }
}

public class MultiplyNode : ExpressionNode
{
    public readonly string Value = "*";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}{Value}");
    }
}

public class DivideNode : ExpressionNode
{
    public readonly string Value = "/";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}{Value}");
    }
}

public class PowerNode : ExpressionNode
{
    public readonly string Value = "^";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}{Value}");
    }
}

public class NotNode : ExpressionNode
{
    public readonly string Value = "!";

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}{Value}");
    }
}

public class BinaryExpressionNode(string conjunctor) : ExpressionNode
{
    public List<ExpressionNode> Values = [];
    public string Conjunctor = conjunctor;

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}BinaryExpressionNode: {Conjunctor}");

        string childIndent = indent + (isLast ? "    " : "│   ");

        for (int i = 0; i < Values.Count; i++)
        {
            Values[i].Display(childIndent, i == Values.Count - 1);
        }
    }
}

public class ExpressionSequenceNode : ExpressionNode
{
    public List<ExpressionNode> Values = [];

    public override void Display(string indent = "", bool isLast = true)
    {
        Console.WriteLine($"{indent}{(isLast ? "└── " : "├── ")}ExpressionSequenceNode");

        string childIndent = indent + (isLast ? "    " : "│   ");

        for (int i = 0; i < Values.Count; i++)
        {
            Values[i].Display(childIndent, i == Values.Count - 1);
        }
    }
}