/**************************************************************************
Copyright (C) 2023 Rekkonnect

This file is part of CompactView.

CompactView is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

CompactView is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with CompactView.  If not, see <http://www.gnu.org/licenses/>.

CompactView web site <http://sourceforge.net/p/compactview/>.
**************************************************************************/
using System.Collections.Generic;

namespace CompactView.Lexing
{
    public partial class SqlLexer
    {
        public static SqlLexer Shared { get; } = new SqlLexer();

        private List<Token> tokens = new List<Token>();

        private string sourceSql;
        private string upperSql;

        // We specifically introduce those fields to indicate the current state of the token
        // that is being parsed. Trailing whitespace trivia is included in the returning token
        // themselves, upon consuming.

        private bool inSingleLineComment;
        private bool inMultiLineComment;
        private bool inString;
        private bool inNumber;
        private bool inIdentifier;

        private bool InComment => inSingleLineComment || inMultiLineComment;

        public Token LastToken
        {
            get
            {
                if (tokens.Count == 0)
                    return default;

                return tokens[tokens.Count - 1];
            }
        }

        private int squareBracketNest;

        private int currentTokenStart = 0;
        private TokenKind currentTokenKind;

        private void Reset()
        {
            tokens = new List<Token>();

            inSingleLineComment = false;
            inMultiLineComment = false;

            inNumber = false;
            inString = false;
            inIdentifier = false;

            squareBracketNest = 0;
            currentTokenStart = 0;
        }

        public List<Token> Tokenize(string sql)
        {
            Reset();

            sourceSql = sql;
            upperSql = sql.ToUpperInvariant();

            char[] chars = upperSql.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                char currentChar = chars[i];

                // Generally, the terminators do not immediately consume the token
                // This is because we want to eat up all the following whitespace
                // In order to avoid the need for classifying whitespace as trivia
                // and specializing the lexer further

                // Therefore the only places where the token is consumed is on the
                // start of a new token kind, from definite characters that specify
                // the beginning of a token.

                // Only handle potential comment terminators
                switch (currentChar)
                {
                    case '\n':
                        HandleWhitespace();
                        continue;

                    case '*':
                        if (inMultiLineComment)
                        {
                            if (i + 1 < chars.Length)
                            {
                                bool isNextClosing = chars[i + 1] == '/';
                                if (isNextClosing)
                                {
                                    inMultiLineComment = false;
                                    i++;
                                }
                            }

                            // Only handle the * character if we are in a comment
                            continue;
                        }

                        break;
                }

                // Do not proceed to evaluate the character if we are in a comment
                if (InComment)
                    continue;

                // Similarly, evalutae strings with higher precedence
                switch (currentChar)
                {
                    case '\'':
                        if (!inString)
                        {
                            ConsumeToken(TokenKind.String, i);
                        }

                        inString = !inString;
                        continue;
                }

                // If we are still in the string, avoid processing the character further
                if (inString)
                    continue;

                switch (currentChar)
                {
                    case '-':
                        if (i + 1 < chars.Length)
                        {
                            bool startsComment = chars[i + 1] == '-';
                            if (startsComment)
                            {
                                inSingleLineComment = true;
                                ConsumeToken(TokenKind.SingleLineComment, i);
                                i++;
                            }
                        }

                        continue;

                    case '/':
                        if (i + 1 < chars.Length)
                        {
                            bool startsComment = chars[i + 1] == '*';
                            if (startsComment)
                            {
                                inMultiLineComment = true;
                                ConsumeToken(TokenKind.MultiLineComment, i);
                            }
                        }

                        continue;

                    case '[':
                        // Error recovery
                        ConsumeIfRunningLiteral(TokenKind.Basic, i);
                        // END Error recovery

                        if (squareBracketNest == 0)
                        {
                            ConsumeToken(TokenKind.BracketedIdentifier, i);
                            inIdentifier = true;
                        }

                        squareBracketNest++;
                        continue;

                    case ']':
                        // Error recovery
                        ConsumeIfRunningLiteral(TokenKind.Basic, i);
                        // END Error recovery

                        if (squareBracketNest > 0)
                        {
                            squareBracketNest--;
                        }
                        if (squareBracketNest == 0)
                        {
                            inIdentifier = false;
                        }

                        continue;

                    case '.':
                        if (currentTokenKind == TokenKind.Number)
                        {
                            continue;
                        }

                        bool canStart = CanStartDottedNumberToken();

                        if (canStart)
                        {
                            HandleNumberCharacter(i);
                        }
                        else
                        {
                            // If we are within a bracketed identifier,
                            // the dot can be a valid character inside it
                            if (squareBracketNest == 0)
                            {
                                inIdentifier = false;
                                ConsumeToken(TokenKind.Basic, i);
                            }
                        }

                        continue;

                    case ' ':
                    case '\t':
                        HandleWhitespace();
                        continue;

                    default:
                        if (currentChar >= '0' && currentChar <= '9')
                        {
                            HandleNumberCharacter(i);
                            continue;
                        }

                        inNumber = false;

                        bool identifierCharacter =
                            char.IsLetter(currentChar) || currentChar == '_';

                        bool shouldConsume = false;

                        var nextTokenKind = TokenKind.Basic;
                        if (identifierCharacter)
                        {
                            nextTokenKind = TokenKind.Identifier;

                            if (!inIdentifier)
                            {
                                shouldConsume = true;
                            }
                        }
                        else
                        {
                            if (currentTokenKind != TokenKind.Basic)
                            {
                                shouldConsume = true;
                            }
                        }

                        if (shouldConsume)
                        {
                            ConsumeToken(nextTokenKind, i);
                        }
                        inIdentifier = identifierCharacter;

                        /*
                        // Must be some other token, like an operator or a random invalid symbol
                        switch (currentTokenKind)
                        {
                            case TokenKind.BracketedIdentifier:
                            case TokenKind.Identifier:
                            case TokenKind.Number:
                                // We should never encounter the bracketed identifier here
                                bool insideValidBracketed = inIdentifier
                                    && currentTokenKind == TokenKind.BracketedIdentifier;

                                if (insideValidBracketed)
                                {
                                    // This is an unexpected scenario in a valid SQL statement
                                    // but it can trigger in very corrupted scenarios
                                    continue;
                                }

                                break;
                        }
                        */

                        continue;
                }
            }

            ConsumeToken(TokenKind.Undetermined, chars.Length);

            return tokens;
        }

        private void HandleWhitespace()
        {
            inSingleLineComment = false;
            inNumber = false;
            inIdentifier = false;
        }

        private void HandleNumberCharacter(int currentIndex)
        {
            // If we are already binding an identifier,
            // the number is part of the identifier
            // The dot (.) always terminates the LHS of the expression,
            // with the left part being an individual identifier token acting
            // as the qualifier
            if (inIdentifier)
                return;

            if (inNumber)
                return;

            ConsumeToken(TokenKind.Number, currentIndex);
            inNumber = true;
        }

        private bool CanStartDottedNumberToken()
        {
            return CanStartDottedNumberToken(tokens.Count);
        }
        private bool CanStartDottedNumberToken(int tokenIndex)
        {
            if (tokenIndex < 0)
                return false;

            var tokenKind = currentTokenKind;

            if (tokenIndex < tokens.Count)
            {
                var token = tokens[tokenIndex];
                tokenKind = token.Kind;
            }

            switch (tokenKind)
            {
                case TokenKind.Basic:
                case TokenKind.Undetermined:
                    return true;

                case TokenKind.MultiLineComment:
                case TokenKind.SingleLineComment:
                    // keep looking up previous tokens
                    return CanStartDottedNumberToken(tokenIndex - 1);

                default:
                    return false;
            }
        }

        private void ConsumeIfRunningLiteral(TokenKind nextTokenKind, int nextTokenStart)
        {
            switch (currentTokenKind)
            {
                case TokenKind.Number:
                case TokenKind.String:
                    ConsumeToken(nextTokenKind, nextTokenStart);
                    break;
            }
        }

        private void ConsumeToken(TokenKind nextTokenKind, int nextTokenStart)
        {
            if (currentTokenKind == TokenKind.Undetermined)
            {
                currentTokenKind = nextTokenKind;
                return;
            }

            var tokenKind = EvaluateTokenKindForIdentifier(nextTokenStart);
            currentTokenKind = tokenKind;

            var stringSlice = GetCurrentStringSlice(nextTokenStart, sourceSql);

            var token = new Token(currentTokenKind, stringSlice);
            tokens.Add(token);

            currentTokenStart = nextTokenStart;
            currentTokenKind = nextTokenKind;
        }

        private TokenKind EvaluateTokenKindForIdentifier(int nextTokenStart)
        {
            if (currentTokenKind != TokenKind.Identifier)
                return currentTokenKind;

            var stringSlice = GetCurrentStringSlice(nextTokenStart, upperSql);
            var tokenString = stringSlice.Trim().ToString();

            if (KnownWords.Keywords.Contains(tokenString))
            {
                return TokenKind.Keyword;
            }
            if (KnownWords.Types.Contains(tokenString))
            {
                return TokenKind.Type;
            }
            if (KnownWords.Functions.Contains(tokenString))
            {
                return TokenKind.KnownFunction;
            }

            return TokenKind.Identifier;
        }

        private StringSlice GetCurrentStringSlice(int nextTokenStart, string source)
        {
            int length = nextTokenStart - currentTokenStart;
            return new StringSlice(source, currentTokenStart, length);
        }
    }
}
