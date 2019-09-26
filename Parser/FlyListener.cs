//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from c:\Users\danie\Documents\Projects\OnTheFly\Fly.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace FlyLang {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="FlyParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface IFlyListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] FlyParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] FlyParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] FlyParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] FlyParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.varAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarAssignment([NotNull] FlyParser.VarAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.varAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarAssignment([NotNull] FlyParser.VarAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.importStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterImportStatement([NotNull] FlyParser.ImportStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.importStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitImportStatement([NotNull] FlyParser.ImportStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.ifElse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfElse([NotNull] FlyParser.IfElseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.ifElse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfElse([NotNull] FlyParser.IfElseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.forLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForLoop([NotNull] FlyParser.ForLoopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.forLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForLoop([NotNull] FlyParser.ForLoopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.statementBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatementBlock([NotNull] FlyParser.StatementBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.statementBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatementBlock([NotNull] FlyParser.StatementBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.methodDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethodDefinition([NotNull] FlyParser.MethodDefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.methodDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethodDefinition([NotNull] FlyParser.MethodDefinitionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression([NotNull] FlyParser.ExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression([NotNull] FlyParser.ExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.methodCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethodCall([NotNull] FlyParser.MethodCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.methodCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethodCall([NotNull] FlyParser.MethodCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArray([NotNull] FlyParser.ArrayContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArray([NotNull] FlyParser.ArrayContext context);
}
} // namespace FlyLang
