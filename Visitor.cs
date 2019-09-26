using System;
using System.Collections.Generic;
using System.Text;
using FlyLang;

namespace OnTheFly
{
    public class Listener : FlyBaseListener
    {
        public Instructions Instructions;

        public Listener()
        {
            Instructions = new Instructions();
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
            if (context.expression() != null)
                EnterExpression(context.expression());
            if (context.varAssignment() != null)
                EnterVarAssignment(context.varAssignment());
            if (context.ifElse() != null)
                EnterIfElse(context.ifElse());
        }

        public override void EnterExpression(FlyParser.ExpressionContext context)
        {
            if (context.INT() != null)
                Instructions.AddInt(context.INT().GetText());
            if(context.NIL() != null)
                Instructions.Add(OpCode.LOAD_NIL);
            else if (context.STRING() != null)
            {
                Instructions.Add(OpCode.LOAD_STR);
                Instructions.Add(Instructions.AddString(context.STRING().GetText(), true));
            }
            else if (context.FLOAT() != null)
                Instructions.AddFloat(context.FLOAT().GetText());
            else if (context.BOOL() != null)
                Instructions.AddBool(context.BOOL().GetText());
            else if (context.ID() != null)
            {
                var i = Instructions.AddString(context.ID().GetText());
                Instructions.Add(OpCode.GET_VAR);
                Instructions.Add(i);
            }
            else if (context.array() != null)
            {
                EnterArray(context.array());
            }
            else if (context.unary != null)
            {
                EnterExpression(context.right);
                Instructions.Add(OpCode.INVERSE);
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
                }
            }
            else
                throw new Exception("Not a valid expression");
        }

        public override void EnterVarAssignment(FlyParser.VarAssignmentContext context)
        {
            EnterExpression(context.expression());
            Instructions.Add(OpCode.SET_VAR);
            Instructions.Add(Instructions.AddString(context.ID().GetText()));
        }

        public override void EnterIfElse(FlyParser.IfElseContext context)
        {
            var endPositions = new List<int>();

            EnterExpression(context.ifExpr);
            Instructions.Add(OpCode.JMP_NEQ);
            // Create an empty parameter which should be the end of the statement
            var ifEndPos = Instructions.Fillable();
            foreach (var statement in context._if)
            {
                EnterStatement(statement);
            }

            Instructions.Add(OpCode.JMP);
            endPositions.Add(Instructions.Fillable());

            Instructions.Fill(ifEndPos, Instructions.Count);
            var statementCount = 1;
            if (context._elifExpr.Count > 0)
            {
                for (var i = 0; i < context._elifExpr.Count; i++)
                {
                    var expr = context._elifExpr[i];
                    var block = context._elifSb[i];

                    EnterExpression(expr);
                    Instructions.Add(OpCode.JMP_NEQ);
                    var elifEndPos = Instructions.Fillable();
                    foreach (var stmt in block.statement())
                    {
                        EnterStatement(stmt);
                    }

                    Instructions.Add(OpCode.JMP);
                    endPositions.Add(Instructions.Fillable());
                    Instructions.Fill(elifEndPos, Instructions.Count);
                }
            }

            if (context._else.Count > 0)
            {
                foreach (var statement in context._else)
                {
                    EnterStatement(statement);
                }
            }

            endPositions.ForEach(x => Instructions.Fill(x, Instructions.Count));
        }
    }
}