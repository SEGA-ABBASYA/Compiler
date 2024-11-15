using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    //Reserved words and operators token names
    Main, Int, Float, String, Read, Write, Repeat, Until, If, Elseif, Else, Then, Return, Endl,
    And, Or, Semicolon, Comma, LParanthesis, RParanthesis, AssignmentOp, NotEqualOp, ConditionEqualOp, LessThanOp, GreaterThanOp,
    PlusOp, MinusOp, MultiplyOp, DivideOp, Identifier, Constant, LPracket, RPracket, End
}
namespace Tiny_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            //ReservedWords.Add("IF", Token_Class.If);
            //Operators.Add(".", Token_Class.Dot); and so on
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);

            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LPracket);
            Operators.Add("}", Token_Class.RPracket);
            Operators.Add("=", Token_Class.ConditionEqualOp);
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);


        }

        public void StartScanning(string SourceCode)
        {
            
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = "";

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if ((CurrentChar >= 'A' && CurrentChar <= 'z')) //if you read a character
                {
                    while (CurrentChar >= 'A' && CurrentChar <= 'z' || CurrentChar >= '0' && CurrentChar <= '9')
                    {
                        j++;
                        CurrentLexeme += CurrentChar;
                        
                        if (j == SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    while (CurrentChar >= '0' && CurrentChar <= '9' || CurrentChar == '.')
                    {
                        CurrentLexeme += CurrentChar;
                        j++;
                        if (j == SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                
                }
                else if (CurrentChar == '&')
                {
                    Console.WriteLine("&");
                    j++;
                    CurrentLexeme += CurrentChar;
                    if (SourceCode.Length == 1)
                    {
                        FindTokenClass(CurrentLexeme);
                        break;
                    }
                    if (j == SourceCode.Length)
                    {
                        Console.WriteLine("break");
                        break;
                    }
                    
                    CurrentChar = SourceCode[j];
                    if (CurrentChar == '&')
                    {
                        CurrentLexeme += CurrentChar;
                    }
                    Console.WriteLine("Find");
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar == '|')
                {
                    CurrentLexeme += CurrentChar;
                    if (SourceCode.Length == 1)
                    {
                        FindTokenClass(CurrentLexeme);
                        break;
                    }
                    if (j == SourceCode.Length - 1)
                        break;
                    j++;
                    CurrentChar = SourceCode[j];
                    if (CurrentChar == '|')
                    {
                        CurrentLexeme += CurrentChar;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar == ':')
                {
                    CurrentLexeme += CurrentChar;
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '=')
                    {
                        j++;
                        CurrentLexeme += '=';
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == '<')
                {
                    CurrentLexeme += CurrentChar;
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '>')
                    {
                        j++;
                        CurrentLexeme += '>';
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == '{') // we still in arugements on it 
                {

                }
                
                else
                {
                    CurrentLexeme += CurrentChar;
                    FindTokenClass(CurrentLexeme);
                    i = j;

                }

                TINY_Compiler.TokenStream = Tokens;
            }
            void FindTokenClass(string Lex)
            {
                Console.WriteLine(Lex);
                Token_Class TC;
                Token Tok = new Token();
                Tok.lex = Lex;
             
                //Is it a reserved word?
                if (ReservedWords.ContainsKey(Tok.lex))
                {
                    Console.WriteLine("Res");
                    Tok.token_type = ReservedWords[Lex];
                    Tokens.Add(Tok);

                }

                //Is it an identifier?
                else  if(isIdentifier(Lex)){
                    Console.WriteLine("Id");
                    Tok.token_type=Token_Class.Identifier;
                    Tokens.Add(Tok);
                }


                //Is it a Constant?
                else if(isConstant(Lex)){
                    Console.WriteLine("Con");
                    Tok.token_type=Token_Class.Constant;
                    Tokens.Add(Tok);
                }

                //Is it an operator?
                
                else if (Operators.ContainsKey(Tok.lex))
                {
                    Console.WriteLine("Op");
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);

                }
                //Is it an undefined? (Error List)
                else{
                    Console.WriteLine("ELSE");
                     Errors.Error_List.Add(Lex);
                }
            }



            bool isIdentifier(string lex)
            {
                // Check if the lex is an identifier or not.
                var reg = new Regex(@"^[A-Za-z][A-Za-z0-9]*$");
                return reg.IsMatch(lex);
            }
            bool isConstant(string lex)
            {
                // Check if the lex is a constant (Number) or not.
                var reg = new Regex(@"[+|-]?[0-9]+(\.[0-9]+)?");
                return reg.IsMatch(lex);
            }
        }
    }
}
