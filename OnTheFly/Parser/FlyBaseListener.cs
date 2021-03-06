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
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IFlyListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class FlyBaseListener : IFlyListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterProgram([NotNull] FlyParser.ProgramContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.program"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitProgram([NotNull] FlyParser.ProgramContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStatement([NotNull] FlyParser.StatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStatement([NotNull] FlyParser.StatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.returnStmt"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturnStmt([NotNull] FlyParser.ReturnStmtContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.returnStmt"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturnStmt([NotNull] FlyParser.ReturnStmtContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.breakStmt"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBreakStmt([NotNull] FlyParser.BreakStmtContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.breakStmt"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBreakStmt([NotNull] FlyParser.BreakStmtContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.varAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVarAssignment([NotNull] FlyParser.VarAssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.varAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVarAssignment([NotNull] FlyParser.VarAssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.varMultiAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVarMultiAssignment([NotNull] FlyParser.VarMultiAssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.varMultiAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVarMultiAssignment([NotNull] FlyParser.VarMultiAssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.importStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterImportStatement([NotNull] FlyParser.ImportStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.importStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitImportStatement([NotNull] FlyParser.ImportStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.package"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPackage([NotNull] FlyParser.PackageContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.package"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPackage([NotNull] FlyParser.PackageContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.ifElse"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfElse([NotNull] FlyParser.IfElseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.ifElse"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfElse([NotNull] FlyParser.IfElseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.forLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForLoop([NotNull] FlyParser.ForLoopContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.forLoop"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForLoop([NotNull] FlyParser.ForLoopContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.statementBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStatementBlock([NotNull] FlyParser.StatementBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.statementBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStatementBlock([NotNull] FlyParser.StatementBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.lambdaExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLambdaExpression([NotNull] FlyParser.LambdaExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.lambdaExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLambdaExpression([NotNull] FlyParser.LambdaExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.methodDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMethodDefinition([NotNull] FlyParser.MethodDefinitionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.methodDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMethodDefinition([NotNull] FlyParser.MethodDefinitionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.anonymousMethodDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousMethodDefinition([NotNull] FlyParser.AnonymousMethodDefinitionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.anonymousMethodDefinition"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousMethodDefinition([NotNull] FlyParser.AnonymousMethodDefinitionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.arrOrVar"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArrOrVar([NotNull] FlyParser.ArrOrVarContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.arrOrVar"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArrOrVar([NotNull] FlyParser.ArrOrVarContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] FlyParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] FlyParser.ExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.methodCall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMethodCall([NotNull] FlyParser.MethodCallContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.methodCall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMethodCall([NotNull] FlyParser.MethodCallContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.quickArray"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQuickArray([NotNull] FlyParser.QuickArrayContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.quickArray"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQuickArray([NotNull] FlyParser.QuickArrayContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlyParser.array"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArray([NotNull] FlyParser.ArrayContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlyParser.array"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArray([NotNull] FlyParser.ArrayContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
} // namespace FlyLang
