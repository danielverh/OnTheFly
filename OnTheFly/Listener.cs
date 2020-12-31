using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FlyLang;
using OnTheFly.Code;

namespace OnTheFly
{
    [Serializable]
    public class Listener : FlyBaseListener, ICloneable
    {
        public CodeGenerator Code = new CodeGenerator();

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
            if (context.varAssignment() != null)
            {
                EnterVarAssignment(context.varAssignment());
                Code.Instructions.Add(OpCode.POP);
            }
            else if (context.varMultiAssignment() != null)
                EnterVarMultiAssignment(context.varMultiAssignment());
            else if (context.expression() != null)
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
                Code.Break();
            }
        }

        public override void EnterExpression(FlyParser.ExpressionContext context)
        {
            if (context.INT() != null) // Int to operator stack
                Code.Int(context.INT().GetText());
            else if (context.NIL() != null) // Null to operator stack
                Code.Nil();
            else if (context.parenExp != null)
                EnterExpression(context.parenExp);
            else if (context.STRING() != null) // Load string to constants, and push to operator stack
                Code.String(context.STRING().GetText());
            else if (context.FLOAT() != null) // Float to operator stack
                Code.Float(context.FLOAT().GetText());
            else if (context.BOOL() != null)
                Code.Bool(context.BOOL().GetText());
            else if (context.ID() != null)
            {
                if (context.index != null)
                {
                    EnterExpression(context.index);
                    Code.VarCall(context.ID().GetText());
                    Code.ArrayGet();
                }
                else
                {
                    Code.VarCall(context.ID().GetText());
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

                    Code.LibFunctionCall(lib, method);
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
                Code.UnaryOperator(context.unary.Text);
            }
            else if (context.left != null && context.right != null && context.op != null)
            {
                EnterExpression(context.right);
                EnterExpression(context.left);
                Code.BinaryOperator(context.op.Text);
            }
            else if (context.left != null && context.right != null && context.comp != null)
            {
                EnterExpression(context.right);
                EnterExpression(context.left);
                Code.CompareOperator(context.comp.Text);
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

            // Compound assignment
            if (context.op != null)
            {
                Code.GetVar(name);

                if (context.index != null)
                {
                    EnterExpression(context.index);
                    Code.ArrayGet();
                }

                Code.BinaryOperator(context.op.Text);
            }

            // Array assignment
            if (context.index != null)
            {
                EnterExpression(context.index);
                Code.GetVar(name);
                Code.ArraySet();
            }
            else
            {
                Code.SetVar(name);
            }
        }

        public override void EnterIfElse(FlyParser.IfElseContext context)
        {
            // TODO: Implement in CodeGenerator
            var endPositions = new List<int>();

            EnterExpression(context.ifExpr);
            Code.Instructions.Add(OpCode.JMP_FALSE);
            // Create an empty parameter which should be the end of the statement
            var ifEndPos = Code.Instructions.FillableInt();
            Code.Instructions.Block(() =>
            {
                foreach (var statement in context._if)
                {
                    EnterStatement(statement);
                }
            });
            Code.Instructions.Add(OpCode.JMP);
            endPositions.Add(Code.Instructions.FillableInt());

            Code.Instructions.FillInt(ifEndPos, Code.Instructions.Count);
            if (context._elifExpr.Count > 0)
            {
                for (var i = 0; i < context._elifExpr.Count; i++)
                {
                    var expr = context._elifExpr[i];
                    var block = context._elifSb[i];

                    EnterExpression(expr);
                    Code.Instructions.Add(OpCode.JMP_FALSE);
                    var elifEndPos = Code.Instructions.FillableInt();
                    Code.Instructions.Block(() =>
                    {
                        foreach (var stmt in block.statement())
                        {
                            EnterStatement(stmt);
                        }
                    });

                    Code.Instructions.Add(OpCode.JMP);
                    endPositions.Add(Code.Instructions.FillableInt());
                    Code.Instructions.FillInt(elifEndPos, Code.Instructions.Count);
                }
            }

            if (context._else.Count > 0)
            {
                Code.Instructions.Block(() =>
                {
                    foreach (var statement in context._else)
                    {
                        EnterStatement(statement);
                    }
                });
            }

            endPositions.ForEach(x => Code.Instructions.FillInt(x, Code.Instructions.Count));
        }

        public override void EnterMethodDefinition(FlyParser.MethodDefinitionContext context)
        {
            Code.MethodDefinition(context.name.Text,
                context._args.Select(x => x.Text).ToArray(), () =>
                {
                    foreach (var statement in context.statement())
                    {
                        EnterStatement(statement);
                    }

                    if (Code.Instructions.Last() != (int)OpCode.RETURN)
                    {
                        Code.Nil();
                        Code.Instructions.Add(OpCode.RETURN);
                    }
                }
            );
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
                        Code.Instructions.Add(OpCode.PRINT);
                    }

                    Code.Instructions.Add(OpCode.PRINT_LN);
                    break;
                case "input":
                    Code.Instructions.Add(OpCode.READ_LN);
                    break;
                case "count":
                case "remove":
                case "insert":
                    foreach (var expr in expressions)
                    {
                        EnterExpression(expr);
                    }

                    Code.Builtin(name);
                    break;
                default:
                    foreach (var expr in expressions)
                    {
                        EnterExpression(expr);
                    }

                    Code.FunctionCall(name);
                    break;
            }
        }

        public override void EnterReturnStmt(FlyParser.ReturnStmtContext context)
        {
            EnterExpression(context.expression());
            Code.Return();
        }

        private int for_recursion = 0;

        public override void EnterForLoop(FlyParser.ForLoopContext context)
        {
            // TODO: Implement in CodeGenerator
            if (context.var != null)
            {
                var indexer = Code.Instructions.AddString("@f" + for_recursion++);
                Code.Instructions.Add(OpCode.LOAD_I64);
                Code.Instructions.AddInt(0);
                Code.Instructions.Add(OpCode.SET_VAR);
                Code.Instructions.AddInt(indexer);

                var start = Code.Instructions.Count;
                EnterExpression(context.expression());
                Code.Instructions.Add(OpCode.GET_VAR);
                Code.Instructions.AddInt(indexer);
                Code.Instructions.Add(OpCode.SMALLER);

                Code.Instructions.Add(OpCode.JMP_FALSE); // Jump to the end if expression is false
                var endPos = Code.Instructions.FillableInt(); // Set the end position
                Code.Instructions.StartBlock(); // Start a block for context dependent variables 
                EnterExpression(context.expression());
                Code.Instructions.Add(OpCode.GET_VAR);
                Code.Instructions.AddInt(indexer);
                Code.Instructions.Add(OpCode.ARRAY_GET);

                Code.Instructions.Add(OpCode.SET_VAR);
                Code.Instructions.AddInt(Code.Instructions.AddString(context.var.Text));
                foreach (var stmt in context.statement())
                    EnterStatement(stmt);

                Code.Instructions.EndBlock(); // Close the current block

                Code.Instructions.Add(OpCode.GET_VAR);
                Code.Instructions.AddInt(indexer);
                Code.Instructions.Add(OpCode.ADD_I1);

                Code.Instructions.Add(OpCode.SET_VAR);
                Code.Instructions.AddInt(indexer);

                Code.Instructions.Add(OpCode.JMP);
                Code.Instructions.AddInt(start);

                Code.Instructions.FillInt(endPos, Code.Instructions.Count);
            }
            else
            {
                var start = Code.Instructions.Count;
                EnterExpression(context.expression());

                Code.Instructions.Add(OpCode.JMP_FALSE); // Jump to the end if expression is false
                var endPos = Code.Instructions.FillableInt(); // Set the end position
                Code.Instructions.Add(OpCode.START_LOOP); // Start a block for context dependent variables 
                var endPos2 = Code.Instructions.FillableInt(); // Set the end position

                foreach (var stmt in context.statement())
                    EnterStatement(stmt);

                Code.Instructions.EndBlock(); // Close the current block

                Code.Instructions.Add(OpCode.JMP);
                Code.Instructions.AddInt(start);

                Code.Instructions.FillInt(endPos, Code.Instructions.Count);
                Code.Instructions.FillInt(endPos2, Code.Instructions.Count);
            }
        }

        public override void EnterArray(FlyParser.ArrayContext context)
        {
            if (context.var != null)
            {
                var hasEnd = context.spliceEnd != null;
                Code.Instructions.Add(OpCode.GET_VAR);
                var varName = context.var.Text;
                Code.Instructions.AddInt(Code.Instructions.AddString(varName));
                EnterExpression(context.spliceStart);
                if (hasEnd)
                    EnterExpression(context.spliceEnd);
                else
                {
                    Code.Instructions.Add(OpCode.GET_VAR);
                    Code.Instructions.AddInt(Code.Instructions.AddString(varName));
                    Code.Instructions.Add(OpCode.SUB_I1);
                }

                Code.Instructions.Add(OpCode.ARRAY_SPLICE);
            }
            else
            {
                var big = context.size != null;
                if (big)
                {
                    EnterExpression(context.addSize);
                    EnterExpression(context.size);
                }

                Code.Instructions.Add(big ? OpCode.ARRAY_ADD_BIG : OpCode.ARRAY_ADD);
                var expressions = context._items;
                for (int i = 0; i < expressions.Count; i++)
                {
                    Code.Instructions.Add(OpCode.CLONE);
                    EnterExpression(context.expression(i));
                    Code.Instructions.Add(OpCode.ARRAY_PUSH);
                }
            }
        }

        public override void EnterImportStatement(FlyParser.ImportStatementContext context)
        {
            var packages = context.package();
            foreach (var package in packages)
            {
                Code.Import(package.GetText());
                Imports.Add(package.GetText());
            }
        }

        public List<string> Imports = new List<string>();

        public override void EnterVarMultiAssignment(FlyParser.VarMultiAssignmentContext context)
        {
            var ids = context.arrOrVar();
            var values = context._values;

            // Check if valid multi assignment:
            // Either one value and multiple ids or the same amount of values and ids
            if (ids.Length == values.Count)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    EnterExpression(values[i]);
                    if (ids[i].index != null)
                    {
                        EnterExpression(ids[i].index);
                        Code.GetVar(ids[i].ID().GetText());
                        Code.ArraySet();
                    }
                    else
                    {
                        Code.SetVar(ids[i].ID().GetText());
                    }
                    Code.Instructions.Add(OpCode.POP);
                }
            }
            else if (values.Count == 1)
            {
                EnterExpression(values[0]);
                foreach (var id in ids)
                {
                    if (id.index != null)
                    {
                        EnterExpression(id.index);
                        Code.GetVar(id.ID().GetText());
                        Code.ArraySet();
                    }
                    else
                    {
                        Code.SetVar(id.ID().GetText());
                    }
                }
                Code.Instructions.Add(OpCode.POP);
            }
            else
            {
                throw new InvalidOperationException(
                    $"The multi var assignment has a invalid amount of variable names / expressions.");
            }
        }

        public object Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                formatter.Serialize(stream,
                    new VisitorContainer { Generator = Code, Imports = Imports });
                stream.Seek(0, SeekOrigin.Begin);
                var vc = (VisitorContainer)formatter.Deserialize(stream);
                return new Listener { Code = vc.Generator, Imports = vc.Imports };
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }

        [Serializable]
        private class VisitorContainer
        {
            public CodeGenerator Generator;
            public List<string> Imports;
        }
    }
}