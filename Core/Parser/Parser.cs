using System;
using PixelWallE.Core.Lexer;
using System.Collections.Generic;
using Avalonia.Media;
using PixelWallE.Core.Parser.AST.Expressions;
using PixelWallE.Core.Parser.AST.Statements;

namespace PixelWallE.Core.Parser
{
    public class ParseError : Exception { }
    public class Parser
    {
        private readonly List<Token> tokens;
        private int currentPosition;
        
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            currentPosition = 0;
        }
        private static readonly HashSet<TokenType> Function = new()
        {
            TokenType.GetActualX,
            TokenType.GetActualY,
            TokenType.GetCanvasSize,
            TokenType.GetColorCount,
            TokenType.IsBrushColor,
            TokenType.IsBrushSize,
            TokenType.IsCanvasColor
        };
        private bool IsFun(TokenType type) => Function.Contains(type);


        private Token Current => tokens[currentPosition];
        private Token Peek()
        {
            if (currentPosition + 1 < tokens.Count)
            {
                return tokens[currentPosition + 1];
            }
            return null;
        }
        private bool IsAtEnd => Current.Type == TokenType.EoF;
        
        private Token Advance()
        {
            if (!IsAtEnd) currentPosition++;
            return Previous();
        }
        
        private Token Previous() => tokens[currentPosition - 1];
        
        private bool Check(TokenType type)
        {
            if (IsAtEnd)
            {
                return false;
            }
            return Current.Type == type;
        }
        
        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        
        private Token Consume(TokenType type, string errorMessage)
        {
            if (Check(type)) return Advance();
            throw Error(Current, errorMessage);
        }
        private ParseError Error(Token token, string message)
        {
            MainWallE.SyntaxError(token, message);
            return new ParseError();
        }

        public List<Statement> Parse()
        {
            try
            {
                List<Statement> statements = new List<Statement>();
                while (!IsAtEnd) 
                {
                    statements.Add(ParseDeclaration());
                }
                return statements;
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        private Expression ParseExpression()
        {
            return ParseOr();
        }


        private Statement ParseDeclaration()
        {
            try
            {
                if (Check(TokenType.Identifier) && Peek().Type == TokenType.Arrow)
                {
                    return ParseVarDeclaration();
                }
                if (Check(TokenType.Identifier) && Peek().Type == TokenType.NewLine)
                {
                    return ParseLabel();
                }


                return ParseStatement();
            }
            catch (ParseError error)
            {
                Synchronize();
                return null;
            }
        }

        private Statement ParseVarDeclaration()
        {
            Token identifier = Consume(TokenType.Identifier, "Identificador esperado.");
            Expression initializer = null;
            if (Match(TokenType.Arrow))
            {
                initializer = ParseExpression();
            }

            Consume(TokenType.NewLine, "Debe haber un salto de línea después de la declaración de variable");
            return new VarDecl(identifier, initializer);
        }
        private Statement ParseLabel()
        {
            Token identifier = Consume(TokenType.Identifier, "Se esperaba nombre de etiqueta.");
            Consume(TokenType.NewLine, "Debe haber un salto de línea luego de la etiqueta.");
            return new LabelStatement(identifier.Lexeme);
        }


        private Statement ParseStatement()
        {
            if (Match(TokenType.GoTo)) return ParseGotoStatement();
            if (Match(TokenType.Spawn)) return ParseSpawn();
            if (Match(TokenType.Color)) return ParseColor();

            return ParseExprStatement();
        }

        private Statement ParseExprStatement()
        {
            Expression expression = ParseExpression();
            Consume(TokenType.NewLine, "Debe haber un salto de línea después de la expresión");
            return new ExprStatement(expression);
        }
        private Statement ParseGotoStatement()
        {
            Consume(TokenType.LeftBracket, "Falta '[' después de 'GoTo'");
            Token label = Consume(TokenType.Identifier, "Se esperaba nombre de la etiqueta en el GoTo.");
            Consume(TokenType.RightBracket, "Falta ']' después del nombre de la etiqueta.");

            Consume(TokenType.LeftParen, "Falta '(' después de la etiqueta.");
            Expression condition = ParseExpression();
            Consume(TokenType.RightParen, "Falta ')' después de la condición.");

            Consume(TokenType.NewLine, "Se esperaba salto de línea después de GoTo.");
    
            return new GotoStatement(label, condition);
        }
        private Statement ParseSpawn()
        {
            Token keyword = Previous();

            Consume(TokenType.LeftParen, "Falta '(' después de Spawn");

            Expression x = ParseExpression();
            Consume(TokenType.Comma, "Falta ',' entre coordenadas");
            Expression y = ParseExpression();

            Consume(TokenType.RightParen, "Falta ')' después de Spawn");

            return new SpawnStatement(keyword, x, y);
        }
        private Statement ParseColor()
        {
            Token keyword = Previous();
            Consume(TokenType.LeftParen, "Falta '(' después de Color.");

            Expression colorExpr = ParseExpression();

            Consume(TokenType.RightParen, "Falta ')' después del color.");
            return new ColorStatement(keyword, colorExpr);
        }



        private Expression ParseOr()
        {
            Expression expr = ParseAnd();

            while (Match(TokenType.Or))
            {
                Token op = Previous();
                Expression right = ParseAnd();
                expr = new BinaryExpression(expr, op, right);
            }

            return expr;
        }
        private Expression ParseAnd()
        {
            Expression expr = ParseEquality();

            while (Match(TokenType.And))
            {
                Token op = Previous();
                Expression right = ParseEquality();
                expr = new BinaryExpression(expr, op, right);
            }

            return expr;
        }


        private Expression ParseEquality()
        {
            Expression expr = ParseComparison();
    
            while (Match(TokenType.NotEqual, TokenType.Equal))
            {
                Token op = Previous();
                Expression right = ParseComparison();
                expr = new BinaryExpression(expr, op, right);
            }
    
            return expr;
        }

        private Expression ParseComparison()
        {
            Expression expr = ParseTerm();
            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                Token op = Previous();
                Expression right = ParseTerm();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }

        private Expression ParseTerm()
        {
            Expression expr = ParseFactor();
            while (Match(TokenType.Minus, TokenType.Plus))
            {
                Token op = Previous();
                Expression right = ParseFactor();
                expr = new BinaryExpression(expr, op, right);
            }
            return expr;
        }

        private Expression ParseFactor()
        {
            Expression expr = ParseUnary();
            while (Match(TokenType.Divide, TokenType.Multiply, TokenType.Power, TokenType.Modulo))
            {
                Token op = Previous();
                Expression right = ParseUnary();
                expr = new BinaryExpression(expr, op, right);
            }
    
            return expr;
        }

        private Expression ParseUnary()
        {
            if (Match(TokenType.Not, TokenType.Minus))
            {
                Token op = Previous();
                Expression right = ParseUnary();
                return new UnaryExpression(op, right);
            }

            return ParseCall();
        }

        private Expression ParseCall()
        {
            Expression expr = ParsePrimary();
            if (IsFun(Current.Type))
            {
                Token fun = Advance();

                Consume(TokenType.LeftParen, "Debe ir un '(' después del nombre de la función.");

                List<Expression> arguments = new();

                if (!Check(TokenType.RightParen))
                {
                    do
                    {
                        Expression arg = ParseExpression();
                        if (arg is CallExpression)
                        {
                            Error(Current, "No se permiten funciones anidadas como argumento.");
                        }

                        arguments.Add(arg);

                    } while (Match(TokenType.Comma));
                }

                Consume(TokenType.RightParen, "Debe ir un ')' después de los argumentos de la función.");

                expr = new CallExpression(fun, arguments);
            }

            return expr;
        }


        private Expression ParsePrimary()
        {
            if (Match(TokenType.Boolean))
            {
                if (Previous().Literal != null)
                {
                    return new LiteralExpression(Previous().Literal);
                }
            }

            if (Match(TokenType.String, TokenType.Number))
            {
                return new LiteralExpression(Previous().Literal);
            }

            if (Match(TokenType.Identifier))
            {
                return new VariableExpression(Previous());
            }

            if (Match(TokenType.LeftParen))
            {
                Expression expr = ParseExpression();
                Consume(TokenType.RightParen, "Expect ')' after expression.");
                return new GroupingExpression(expr);
            }
            throw Error(Current, "Expect expression.");
        }
        private void Synchronize()
        {
            Advance(); // Descarta el token problemático
    
            while (!IsAtEnd)
            {
                if (Previous().Type == TokenType.NewLine) return;
        
                // Buscamos puntos de sincronización (inicios de statement)
                switch (Current.Type)
                {
                    case TokenType.Spawn:
                    case TokenType.Color:
                    case TokenType.DrawLine:
                    case TokenType.GoTo:
                        return;
                }
        
                Advance();
            }
        }
    }
}