using System;
using PixelWallE.Core.Lexer;
using System.Collections.Generic;
using Avalonia.Media;
using PixelWallE.Core.Parser.AST.Expressions;

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
        private Token Current => tokens[currentPosition];
        private Token Peek()
        {
            if (currentPosition + 1 < tokens.Count)
            {
                return tokens[currentPosition + 1];
            }
            return null;
        }
        private bool IsAtEnd => Current.Type == TokenType.EOF;
        
        private Token Advance()
        {
            if (!IsAtEnd) currentPosition++;
            return Previous();
        }
        
        private Token Previous() => tokens[currentPosition - 1];
        
        private bool Check(TokenType type) => !IsAtEnd && Current.Type == type;
        
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
            ReportError(token, message);
            return new ParseError();
        }
        
        private void ReportError(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                ErrorReporter.Report(token.Line, " al final", message);
            }
            else
            {
                ErrorReporter.Report(token.Line, $" en '{token.Lexeme}'", message);
            }
        }

        Expression Parse()
        {
            try
            {
                return ParseExpression();
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        private Expression ParseExpression()
        {
            return ParseEquality();
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

            return ParsePrimary();
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