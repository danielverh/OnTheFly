//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Fly.g4 by ANTLR 4.8

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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="FlyParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public interface IFlyVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProgram([NotNull] FlyParser.ProgramContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] FlyParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.returnStmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturnStmt([NotNull] FlyParser.ReturnStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.breakStmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBreakStmt([NotNull] FlyParser.BreakStmtContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.varAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarAssignment([NotNull] FlyParser.VarAssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.varMultiAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarMultiAssignment([NotNull] FlyParser.VarMultiAssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.importStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitImportStatement([NotNull] FlyParser.ImportStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.package"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPackage([NotNull] FlyParser.PackageContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.ifElse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIfElse([NotNull] FlyParser.IfElseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.forLoop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForLoop([NotNull] FlyParser.ForLoopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.statementBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatementBlock([NotNull] FlyParser.StatementBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.methodDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMethodDefinition([NotNull] FlyParser.MethodDefinitionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.anonymousMethodDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnonymousMethodDefinition([NotNull] FlyParser.AnonymousMethodDefinitionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.arrOrVar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrOrVar([NotNull] FlyParser.ArrOrVarContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] FlyParser.ExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.methodCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMethodCall([NotNull] FlyParser.MethodCallContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlyParser.array"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArray([NotNull] FlyParser.ArrayContext context);
}
} // namespace FlyLang
