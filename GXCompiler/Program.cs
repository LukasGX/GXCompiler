using GXCompiler.GXLexer;
using GXCompiler.GXParser;

namespace GXCompiler;

public static class Program {
	public static void Main(string[] args) {
        string content = File.ReadAllText("program.gxc");

		Lexer lexer = new();

		List<Token> tokens = lexer.Lex(content);
		
		Parser parser = new(tokens);

		Console.WriteLine(parser.Parse());
	}
}
