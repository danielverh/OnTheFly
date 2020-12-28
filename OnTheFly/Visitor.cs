using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyLang;
using OnTheFly.Code;

namespace OnTheFly
{
    public class Listener : FlyBaseListener
    {
        public Instructions Instructions = new Instructions();
        public CodeContexts Contexts = new CodeContexts();

        public Listener()
        {
        }

        public override void EnterProgram(FlyParser.ProgramContext context)
        {
            for (int i = 0; i < context.statement().Length; i++)
            {
                EnterStatement(context.statement(i));
            }
        }

        public override void EnterStatement(FlyParser.StatementContext context)
        {
            var ctx = new CodeContext();
            ctx.Code = context.GetText();
            if (context.Start != null && context.Stop != null)
                ctx.Line = $"{context.Start.Line}-{context.Stop.Line}";
            if (context.Start != null && context.Stop != null)
                ctx.Position = $"{context.Start.Column}-{context.Stop.Column}";
            ctx.OpCodeStart = Instructions.Count;
            if (context.expression() != null)
                EnterExpression(context.expression());
            else if (context.ifElse() != null)
                EnterIfElse(context.ifElse());
            else if (context.methodDefinition() != null)
                EnterMethodDefinition(context.methodDefinition());
            else if (context.methodCall() != null)
                EnterMethodCall(context.methodCall());
            else if (context.forLoop() != null)
                EnterForLoop(context.forLoop());
            else if (context.returnStmt() != null)
                EnterReturnStmt(context.returnStmt());
            else if (context.importStatement() != null)
                EnterImportStatement(context.importStatement());
            else if (context.breakStmt() != null)
            {
                Instructions.Add(OpCode.BREAK);
            }

            ctx.OpCodeEnd = Instructions.Count;
            Contexts.Add(ctx);
        }

        public override void EnterExpression(FlyParser.ExpressionContext context)
        {
            if (context.INT() != null) // Int to operator stack
                Instructions.LoadInt(context.INT().GetText());
            else if (context.NIL() != null) // Null to operator stack
                Instructions.Add(OpCode.LOAD_NIL);
            else if (context.parenExp != null)
                EnterExpression(context.parenExp);
            else if (context.STRING() != null) // Load string to constants, and push to operator stack
            {
                Instructions.Add(OpCode.LOAD_STR);
                var stringIndex = Instructions.AddString(context.STRING().GetText(), true);
                Instructions.AddInt(stringIndex);
            }
            else if (context.FLOAT() != null) // Float to operator stack
                Instructions.LoadFloat(context.FLOAT().GetText());
            else if (context.BOOL() != null)
                Instructions.LoadBool(context.BOOL().GetText());
            else if (context.ID() != null)
            {
                var i = Instructions.AddString(context.ID().GetText());
                Instructions.Add(OpCode.GET_VAR);
                Instructions.AddInt(i);
                if (context.index != null)
                {
                    EnterExpression(context.index);
                    Instructions.Add(OpCode.ARRAY_GET);
                }
            }
            else if (context.callOn != null)
            {
                if (Imports.Contains(context.target.GetText()))
                {
                    var lib = context.target.GetText();
                    if (context.callOn.methodCall() == null)
                        throw new Exception("No method call found.");
                    var method = context.callOn.methodCall().ID().GetText();
                    var expressions = context.callOn.methodCall().expression();
                    foreach (var expr in expressions)
                    {
                        EnterExpression(expr);
                    }

                    Instructions.Add(OpCode.CALL_BUILTIN);
                    Instructions.AddInt(Instructions.AddString(lib));
                    Instructions.AddInt(Instructions.AddString(method));
                }
                else
                {
                    EnterExpression(context.target);
                    EnterExpression(context.callOn);
                }
            }
            else if (context.array() != null)
            {
                EnterArray(context.array());
            }
            else if (context.unary != null)
            {
                EnterExpression(context.right);
                Instructions.Add(OpCode.UNINV);
            }
            else if (context.left != null && context.right != null && context.op != null)
            {
                EnterExpression(context.right);
                EnterExpression(context.left);
                switch (context.op.Text)
                {
                    case "+":
                        Instructions.Add(OpCode.ADD);
                        break;
                    case "-":
                        Instructions.Add(OpCode.SUB);
                        break;
                    case "*":
                        Instructions.Add(OpCode.MUL);
                        break;
                    case "/":
                        Instructions.Add(OpCode.DIV);
                        break;
                    case "%":
                        Instructions.Add(OpCode.MOD);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            else if (context.left != null && context.right != null && context.comp != null)
            {
                EnterExpression(context.right);
                EnterExpression(context.left);
                switch (context.comp.Text)
                {
                    case "==":
                        Instructions.Add(OpCode.EQUALS);
                        break;
                    case "!=":
                        Instructions.Add(OpCode.NOT_EQ);
                        break;
                    case "<":
                        Instructions.Add(OpCode.SMALLER);
                        break;
                    case ">":
                        Instructions.Add(OpCode.LARGER);
                        break;
                    case ">=":
                        Instructions.Add(OpCode.LARGER_EQ);
                        break;
                    case "<=":
                        Instructions.Add(OpCode.SMALLER_EQ);
                        break;
                }
            }
            else if (context.methodCall() != null)
                EnterMethodCall(context.methodCall());
            else if (context.varAssignment() != null)
                EnterVarAssignment(context.varAssignment());
            else
                throw new Exception("Not a valid expression");
        }

        public override void EnterVarAssignment(FlyParser.VarAssignmentContext context)
        {
            var name = context.ID().GetText();
            EnterExpression(context.value);

            if (context.op != null)
            {
                if (context.index != null)
                {
                    Instructions.Add(OpCode.GET_VAR);
                    Instructions.AddInt(Instructions.AddString(name));
                    EnterExpression(context.index);
                    Instructions.Add(OpCode.ARRAY_GET);
                }
                else
                {
                    Instructions.Add(OpCode.GET_VAR);
                    Instructions.AddInt(Instructions.AddString(name));
                }

                switch (context.op.Text)
                {
                    case "+":
                        Instructions.Add(OpCode.ADD);
                        break;
                    case "-":
                        Instructions.Add(OpCode.SUB);
                        break;
                    case "*":
                        Instructions.Add(OpCode.MUL);
                        break;
                    case "/":
                        Instructions.Add(OpCode.DIV);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            if (context.index != null)
            {
                EnterExpression(context.index);
                Instructions.Add(OpCode.GET_VAR);
                Instructions.AddInt(Instructions.AddString(name));
                Instructions.Add(OpCode.ARRAY_SET);
            }
            else
            {
                Instructions.Add(OpCode.SET_VAR);
                Instructions.AddInt(Instructions.AddString(name));
            }
        }

        public override void EnterIfElse(FlyParser.IfElseContext context)
        {
            var endPositions = new List<int>();

            EnterExpression(context.ifExpr);
            Instructions.Add(OpCode.JMP_FALSE);
            // Create an empty parameter which should be the end of the statement
            var ifEndPos = Instructions.FillableInt();
            Instructions.Block(() =>
            {
                foreach (var statement in context._if)
                {
                    EnterStatement(statement);
                }
            });
            Instructions.Add(OpCode.JMP);
            endPositions.Add(Instructions.FillableInt());

            Instructions.FillInt(ifEndPos, Instructions.Count);
            if (context._elifExpr.Count > 0)
            {
                for (var i = 0; i < context._elifExpr.Count; i++)
                {
                    var expr = context._elifExpr[i];
                    var block = context._elifSb[i];

                    EnterExpression(expr);
                    Instructions.Add(OpCode.JMP_FALSE);
                    var elifEndPos = Instructions.FillableInt();
                    Instructions.Block(() =>
                    {
                        foreach (var stmt in block.statement())
                        {
                            EnterStatement(stmt);
                        }
                    });

                    Instructions.Add(OpCode.JMP);
                    endPositions.Add(Instructions.FillableInt());
                    Instructions.FillInt(elifEndPos, Instructions.Count);
                }
            }

            if (context._else.Count > 0)
            {
                Instructions.Block(() =>
                {
                    foreach (var statement in context._else)
                    {
                        EnterStatement(statement);
                    }
                });
            }

            endPositions.ForEach(x => Instructions.FillInt(x, Instructions.Count));
        }

        public override void EnterMethodDefinition(FlyParser.MethodDefinitionContext context)
        {
            Instructions.Add(OpCode.ADD_FUNCTION);
            Instructions.AddInt(Instructions.AddString(context.name.Text));
            Instructions.AddInt(context._args.Count);
            foreach (var arg in context._args)
            {
                Instructions.AddInt(Instructions.AddString(arg.Text));
            }

            var endPos = Instructions.FillableInt();
            Instructions.Block(() =>
            {
                foreach (var statement in context.statement())
                {
                    EnterStatement(statement);
                }

                Instructions.Add(OpCode.RETURN);
            });


            Instructions.FillInt(endPos, Instructions.Count);
        }

        public override void EnterMethodCall(FlyParser.MethodCallContext context)
        {
            var name = context.ID().GetText();
            var expressions = context.expression().Reverse().ToArray();

            switch (name)
            {
                case "print":
                    foreach (var expr in expressions)
                    {
                        EnterExpression(expr);
                        Instructions.Add(OpCode.PRINT);
                    }

                    Instructions.Add(OpCode.PRINT_LN);
                    break;
                case "input":
                    Instructions.Add(OpCode.READ_LN);
                    break;
                case "count":
                    foreach (var expr in expressions)
                    {
                        EnterExpression(expr);
                    }

                    Instructions.Add(OpCode.COUNT);
                    break;
                case "remove":
                    if (expressions.Length != 1)
                    {
                        throw new Exception($"Arity mismatch, expected 1 got {expressions.Length}");
                    }

                    EnterExpression(expressions[0]);
                    Instructions.Add(OpCode.ARRAY_REMOVE);
                    break;
                case "insert":

                    if (expressions.Length != 2)
                    {
                        throw new Exception($"Arity mismatch, expected 2 got {expressions.Length}");
                    }

                    foreach (var expr in expressions)
                    {
                        EnterExpression(expr);
                    }

                    Instructions.Add(OpCode.ARRAY_INSERT);
                    break;
                default:
                    foreach (var expr in expressions)
                    {
                        EnterExpression(expr);
                    }

                    Instructions.Add(OpCode.CALL_FUNCTION);
                    Instructions.AddInt(Instructions.AddString(name));
                    break;
            }
        }

        public override void EnterReturnStmt(FlyParser.ReturnStmtContext context)
        {
            EnterExpression(context.expression());
            Instructions.Add(OpCode.RETURN);
        }

        private int for_recursion = 0;

        public override void EnterForLoop(FlyParser.ForLoopContext context)
        {
            if (context.var != null)
            {
                var indexer = Instructions.AddString("@f" + for_recursion++);
                Instructions.Add(OpCode.LOAD_I32);
                Instructions.AddInt(0);
                Instructions.Add(OpCode.SET_VAR);
                Instructions.AddInt(indexer);

                var start = Instructions.Count;
                EnterExpression(context.expression());
                Instructions.Add(OpCode.COUNT);
                Instructions.Add(OpCode.GET_VAR);
                Instructions.AddInt(indexer);
                Instructions.Add(OpCode.SMALLER);

                Instructions.Add(OpCode.JMP_FALSE); // Jump to the end if expression is false
                var endPos = Instructions.FillableInt(); // Set the end position
                Instructions.StartBlock(); // Start a block for context dependent variables 
                EnterExpression(context.expression());
                Instructions.Add(OpCode.GET_VAR);
                Instructions.AddInt(indexer);
                Instructions.Add(OpCode.ARRAY_GET);

                Instructions.Add(OpCode.SET_VAR);
                Instructions.AddInt(Instructions.AddString(context.var.Text));
                foreach (var stmt in context.statement())
                    EnterStatement(stmt);

                Instructions.EndBlock(); // Close the current block

                Instructions.Add(OpCode.GET_VAR);
                Instructions.AddInt(indexer);
                Instructions.Add(OpCode.ADD_I1);

                Instructions.Add(OpCode.SET_VAR);
                Instructions.AddInt(indexer);

                Instructions.Add(OpCode.JMP);
                Instructions.AddInt(start);

                Instructions.FillInt(endPos, Instructions.Count);
            }
            else
            {
                var start = Instructions.Count;
                EnterExpression(context.expression());

                Instructions.Add(OpCode.JMP_FALSE); // Jump to the end if expression is false
                var endPos = Instructions.FillableInt(); // Set the end position
                Instructions.Add(OpCode.START_LOOP); // Start a block for context dependent variables 
                var endPos2 = Instructions.FillableInt(); // Set the end position

                foreach (var stmt in context.statement())
                    EnterStatement(stmt);

                Instructions.EndBlock(); // Close the current block

                Instructions.Add(OpCode.JMP);
                Instructions.AddInt(start);

                Instructions.FillInt(endPos, Instructions.Count);
                Instructions.FillInt(endPos2, Instructions.Count);
            }
        }

        public override void EnterArray(FlyParser.ArrayContext context)
        {
            if (context.var != null)
            {
                var hasEnd = context.spliceEnd != null;
                Instructions.Add(OpCode.GET_VAR);
                var varName = context.var.Text;
                Instructions.AddInt(Instructions.AddString(varName));
                EnterExpression(context.spliceStart);
                if (hasEnd)
                    EnterExpression(context.spliceEnd);
                else
                {
                    Instructions.Add(OpCode.GET_VAR);
                    Instructions.AddInt(Instructions.AddString(varName));
                    Instructions.Add(OpCode.COUNT);
                    Instructions.Add(OpCode.SUB_I1);
                }

                Instructions.Add(OpCode.ARRAY_SPLICE);
            }
            else
            {
                var big = context.size != null;
                if (big)
                {
                    EnterExpression(context.addSize);
                    EnterExpression(context.size);
                }

                Instructions.Add(big ? OpCode.ARRAY_ADD_BIG : OpCode.ARRAY_ADD);
                var expressions = context._items;
                for (int i = 0; i < expressions.Count; i++)
                {
                    Instructions.Add(OpCode.CLONE);
                    EnterExpression(context.expression(i));
                    Instructions.Add(OpCode.ARRAY_PUSH);
                }
            }
        }

        public override void EnterImportStatement(FlyParser.ImportStatementContext context)
        {
            var packages = context.package();
            foreach (var package in packages)
            {
                Instructions.Add(OpCode.IMPORT);
                Instructions.AddInt(Instructions.AddString(package.GetText()));
                Imports.Add(package.GetText());
            }
        }

        public List<string> Imports = new List<string>();
    }
}