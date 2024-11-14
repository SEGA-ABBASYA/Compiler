using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    //Reserved words and operators token names
    Main , Int, Float, String, Read, Write, Repeat, Until, If, Elseif, Else, Then, Return, Endl,
    Colon, And, Or, Semicolon, Comma, LParanthesis, RParanthesis, AssignmentOp, NotEqualOp, ConditionEqualOp, LessThanOp, GreaterThanOp,
    PlusOp, MinusOp, MultiplyOp, DivideOp, Idenifier, IntNumber, FloatNumber, Number, LPracket, RPracket
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
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if ((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z')) //if you read a character فوق البيعه
                {
                    while (true)
                    {
                        j++;
                        if (j == SourceCode.Length)
                        {
                            i = j;
                            break;
                        }
                        CurrentChar = SourceCode[i];
                        if (((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z')) || (CurrentChar >= '0' && CurrentChar <= '9'))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else
                        {
                            i = j - 1;
                            break;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar >= '0' && CurrentChar <= '9') // فوق البيعه
                {
                    while (true)
                    {
                        j++;
                        if (j == SourceCode.Length)
                        {
                            i = j;
                            break;
                        }
                        CurrentChar = SourceCode[i];
                        if (CurrentChar == ' ' || CurrentChar == '+' || CurrentChar == ':' || CurrentChar == ':' || CurrentChar == '=' || CurrentChar == '>' || CurrentChar == '<' || CurrentChar == '*' ||
                            CurrentChar == '-' || CurrentChar == '/' || CurrentChar == ';' || CurrentChar == ')' || CurrentChar == ',' || CurrentChar == '\n' || CurrentChar == '\r')
                        {
                            i = j - 1;
                            break;
                        }
                        CurrentLexeme += CurrentChar.ToString();
                    }
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '{') // we still in arugements on it 
                {
                    while(CurrentChar != '}')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar == '&') // boolean Operator "&&"
                {
                    bool flag= false;
                    j++;
                    if(j == SourceCode.Length)
                    {
                        i = j;
                        flag = true;
                    }
                    if (!flag)
                    {
                        CurrentChar = SourceCode[j];
                        if(CurrentChar == '&')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        FindTokenClass(CurrentLexeme);
                        i = j;
                    }

                }
                else if(CurrentChar == '|')// Or Operator "&&"
                {
                    bool flag = false;
                    j++;
                    if (j == SourceCode.Length)
                    {
                        FindTokenClass((CurrentLexeme));
                        i = j;
                        flag = true;
                    }
                    if (!flag)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '|')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        FindTokenClass(CurrentLexeme);
                        i = j;
                    }
                }
                else if(CurrentChar == '<') // codition Operator "<= <>"
                {
                    bool flag = false;
                    j++;
                    if (j == SourceCode.Length)
                    {
                        i = j;
                        flag = true;
                    }
                    if (!flag)
                    {
                        CurrentChar = SourceCode[j];
                        if(CurrentChar == '=')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else if (CurrentChar == '>')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        FindTokenClass(CurrentLexeme);
                        i = j;
                    }
                }
                else if(CurrentChar == ':') // assigment Operator ":="
                {
                    bool flag = false;
                    j++;
                    if (j == SourceCode.Length)
                    {
                        i = j;
                        flag = true;
                    }
                    if (!flag)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '=')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        FindTokenClass(CurrentLexeme);
                        i = j;
                    }
                }
                else
                {
                    // error handle
                }

                TINY_Compiler.TokenStream = Tokens;
            }
            void FindTokenClass(string Lex)
            {
                Token_Class TC;
                Token Tok = new Token();
                Tok.lex = Lex;
                //Is it a reserved word?


                //Is it an identifier?


                //Is it a Constant?

                //Is it an operator?

                //Is it an undefined? (Error List)
            }



            bool isIdentifier(string lex)
            {
                bool isValid = true;
                // Check if the lex is an identifier or not.

                return isValid;
            }
            bool isConstant(string lex)
            {
                bool isValid = true;
                // Check if the lex is a constant (Number) or not.

                return isValid;
            }
        }
    }
}
