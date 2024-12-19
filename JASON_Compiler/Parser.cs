using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            Console.WriteLine("I got here");
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");

            if (TokenStream[InputPointer + 1].token_type != Token_Class.Main)
                program.Children.Add(Function_Statements());
            program.Children.Add(Main_Function());

            return program;
        }
        Node Main_Function()
        {
            Node main_function = new Node("Main_Function");
            main_function.Children.Add(DataType());
            main_function.Children.Add(match(Token_Class.Main));
            main_function.Children.Add(match(Token_Class.LParanthesis));
            main_function.Children.Add(match(Token_Class.RParanthesis));
            main_function.Children.Add(Function_Body());

            return main_function;
        }
        Node Function_Statements()
        {

            Node function_statements = new Node("Function_Statements");
            if (TokenStream[InputPointer + 1].token_type != Token_Class.Main && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.ReservedString))
            {
                function_statements.Children.Add(Function_Statement());
                function_statements.Children.Add(Function_Statements());
            }

            return function_statements;
        }
        Node Function_Statement()
        {

            Node function_statement = new Node("Function_Statement");
            function_statement.Children.Add(Function_Declaration());
            function_statement.Children.Add(Function_Body());
            return function_statement;
        }

        Node Function_Declaration()
        {
            Node function_declaration = new Node("Function_Declaration");
            function_declaration.Children.Add(DataType());
            function_declaration.Children.Add(Function_Name());
            function_declaration.Children.Add(match(Token_Class.LParanthesis));
            if (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.ReservedString)
            {
                function_declaration.Children.Add(Parameters());
            }
            function_declaration.Children.Add(match(Token_Class.RParanthesis));

            return function_declaration;
        }
        Node Function_Name()
        {
            Node function_name = new Node("Function_Name");
            function_name.Children.Add(match(Token_Class.Identifier));
            return function_name;
        }
        Node Function_Body()
        {
            Node function_body = new Node("Function_Body");
            function_body.Children.Add(match(Token_Class.LPracket));
            function_body.Children.Add(Statements());
            function_body.Children.Add(Return_Statement());
            function_body.Children.Add(match(Token_Class.RPracket));


            return function_body;
        }

        Node Statements()
        {
            Node statements = new Node("Statements");
            Token_Class currToken = TokenStream[InputPointer].token_type;
            if (currToken == Token_Class.Identifier || currToken == Token_Class.Read || currToken == Token_Class.Write || currToken == Token_Class.If || currToken == Token_Class.Repeat
                || currToken == Token_Class.Int || currToken == Token_Class.Float || currToken == Token_Class.ReservedString)
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements());
            }
                
            return statements;
        }
        Node Statement()
        {
            Node statement = new Node("Statement");
            switch (TokenStream[InputPointer].token_type)
            {
                case Token_Class.Identifier:
                    statement.Children.Add(Assignment_Statement());
                    statement.Children.Add(match(Token_Class.Semicolon));
                    break;

                case Token_Class.Read:
                    statement.Children.Add(Read_Statement());
                    break;

                case Token_Class.Write:
                    statement.Children.Add(Write_Statement());
                    break;

                case Token_Class.If:
                    statement.Children.Add(If_Statement());
                    break;

                case Token_Class.Repeat:
                    statement.Children.Add(Repeat_Statement());
                    break;

                case Token_Class.Int:
                case Token_Class.ReservedString:
                case Token_Class.Float:
                    statement.Children.Add(Declaration_Statement());
                    break;


            }
            return statement;
        }
        Node Term()
        {
            Node term = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));
                return term;
            }
                
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                if(InputPointer + 1 < TokenStream.Count)
                {
                    if (TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                    {
                        term.Children.Add(Function_Call());
                        return term;
                    }
                }
                term.Children.Add(match(Token_Class.Identifier));
                return term;
            }
            
            return term;
        }
        Node Parameters()
        {
            Node parameters = new Node("Parameters");


            parameters.Children.Add(Parameter());
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                parameters.Children.Add(Parameters2());


            return parameters;
        }
        Node Parameters2()
        {
            Node parameters2 = new Node("Parameters2");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parameters2.Children.Add(match(Token_Class.Comma));
                parameters2.Children.Add(Parameter());
                parameters2.Children.Add(Parameters2());
            }
            return parameters2;
        }
        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(DataType());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }
        //**********************************************************
        Node Function_Call()
        {
            Node function_call = new Node("Function_Call");
            function_call.Children.Add(match(Token_Class.Identifier));
            function_call.Children.Add(match(Token_Class.LParanthesis));
            function_call.Children.Add(Function_Call2());

            return function_call;
        }
        Node Function_Call2()
        {
            Node function_call2 = new Node("Function_Call2");
            if (TokenStream[InputPointer].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                function_call2.Children.Add(IdList());
            }
            function_call2.Children.Add(match(Token_Class.RParanthesis));

            return function_call2;
        }
        Node IdList()
        {
            Node idList = new Node("IdList");
            idList.Children.Add(Term());
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                idList.Children.Add(IdList2());

            return idList;
        }
        Node IdList2()
        {
            Node idList2 = new Node("IdList2");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                idList2.Children.Add(match(Token_Class.Comma));
                idList2.Children.Add(Term());
                idList2.Children.Add(IdList2());
            }

            return idList2;
        }

        Node Declaration_Statement()
        {
            Node declaration_statement = new Node("Declaration_Statement");
            declaration_statement.Children.Add(DataType());
            declaration_statement.Children.Add(Ids());
            declaration_statement.Children.Add(match(Token_Class.Semicolon));

            return declaration_statement;
        }
        Node Ids()
        {
            Node ids = new Node("Ids");
            ids.Children.Add(IdDecl());
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                ids.Children.Add(Ids2());

            return ids;
        }
        Node Ids2()
        {
            Node ids2 = new Node("Ids2");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                ids2.Children.Add(match(Token_Class.Comma));
                ids2.Children.Add(IdDecl());
                ids2.Children.Add(Ids2());
            }

            return ids2;
        }
        Node IdDecl()
        {
            
            Node idDecl = new Node("IdDecl");
            if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
                idDecl.Children.Add(Assignment_Statement());
            else
                idDecl.Children.Add(match(Token_Class.Identifier));


            return idDecl;
        }

        //ABDALLAH STARTS HERE

        // Implement your logic here
        Node Assignment_Statement()
        {
            Node assignment_Statement = new Node("Assignment_Statement");
            assignment_Statement.Children.Add(match(Token_Class.Identifier));
            assignment_Statement.Children.Add(match(Token_Class.AssignmentOp));
            assignment_Statement.Children.Add(Expression());
            return assignment_Statement;

        }
        Node Expression()
        {
            Node expression = new Node("Expression");
            if (Token_Class.String == TokenStream[InputPointer].token_type)
            {
                expression.Children.Add(match(Token_Class.String));
                return expression;
            }
            if (Token_Class.LParanthesis == TokenStream[InputPointer].token_type)
            {
                expression.Children.Add(Equations());
                return expression;
            }

            if (InputPointer + 1 < TokenStream.Count)
            {
                if (Token_Class.PlusOp == TokenStream[InputPointer + 1].token_type ||
                Token_Class.MinusOp == TokenStream[InputPointer + 1].token_type ||
                Token_Class.MultiplyOp == TokenStream[InputPointer + 1].token_type ||
                Token_Class.DivideOp == TokenStream[InputPointer + 1].token_type)
                {
                    expression.Children.Add(Equations());
                    return expression;
                }
            }
            expression.Children.Add(Term());
            return expression;
        }
        Node Equations()
        {
            Node equations = new Node("Equations");
            equations.Children.Add(Term_Eq());
            equations.Children.Add(Equation());
            return equations;
        }
        Node Term_Eq()
        {
            Node term_Eq = new Node("Term_Eq");
            if (Token_Class.LParanthesis == TokenStream[InputPointer].token_type)
            {
                term_Eq.Children.Add(match(Token_Class.LParanthesis));
                term_Eq.Children.Add(Equations());
                term_Eq.Children.Add(match(Token_Class.RParanthesis));
            }else
            {
                term_Eq.Children.Add(Term());
            }
            return term_Eq;
        }
        Node Equation()
        {
            Node equation = new Node("Equation");
            if(Token_Class.PlusOp == TokenStream[InputPointer].token_type||
                Token_Class.MinusOp == TokenStream[InputPointer].token_type||
                Token_Class.MultiplyOp == TokenStream[InputPointer].token_type||
                Token_Class.DivideOp == TokenStream[InputPointer].token_type)
            {
                equation.Children.Add(Arithmatic_Operator());
                equation.Children.Add(Equations());
            }
            return equation;
        }
        Node Write_Statement()
        {
            Node write_statement = new Node("Write_Statement");
            write_statement.Children.Add(match(Token_Class.Write));
            write_statement.Children.Add(Write_Statement1());
            return write_statement;
        }
        Node Write_Statement1()
        {
            Node write_statement1 = new Node("Write_Statement1");
            if(Token_Class.Endl == TokenStream[InputPointer].token_type)
            {
                write_statement1.Children.Add(match(Token_Class.Endl));
            }else
            {
                write_statement1.Children.Add(Expression());      
            }
            write_statement1.Children.Add(match(Token_Class.Semicolon));
            return write_statement1;
        }
        Node Read_Statement()
        {
            Node read_statement = new Node("Read_Statement");
            read_statement.Children.Add(match(Token_Class.Read));
            read_statement.Children.Add(match(Token_Class.Identifier));
            read_statement.Children.Add(match(Token_Class.Semicolon));
            return read_statement;
        }
        Node Return_Statement()
        {
            Node return_statement = new Node("Return_Statement");
            return_statement.Children.Add(match(Token_Class.Return));
            return_statement.Children.Add(Expression());
            return_statement.Children.Add(match(Token_Class.Semicolon));
            return return_statement;
        }
        Node If_Statement()
        {
            Node if_statement = new Node("If_Statement");
            if_statement.Children.Add(match(Token_Class.If));
            if_statement.Children.Add(Condition_Statement());
            if_statement.Children.Add(match(Token_Class.Then));
            if_statement.Children.Add(Statements());
            if_statement.Children.Add(Else());
            return if_statement;
        }
        Node Else()
        {
            Node else_word = new Node("Else");
            Token_Class curr_type = TokenStream[InputPointer].token_type;
            if (curr_type == Token_Class.Elseif)
            {
                else_word.Children.Add(Else_If_Statement());
            }
            else if (curr_type == Token_Class.Else)
            {
                else_word.Children.Add(Else_Statement());
            }
            else if (curr_type == Token_Class.End)
            {
                else_word.Children.Add(match(Token_Class.End));
            }
            return else_word; 
        }
        Node Else_If_Statement()
        {
            Node else_if_statement = new Node("Else_If_Statement");
            else_if_statement.Children.Add(match(Token_Class.Elseif));
            else_if_statement.Children.Add(Condition_Statement());
            else_if_statement.Children.Add(match(Token_Class.Then));
            else_if_statement.Children.Add(Statements());
            else_if_statement.Children.Add(Else());
            return else_if_statement;
        }
        Node Else_Statement()
        {
            Node else_statements = new Node("Else_Statements");
            else_statements.Children.Add(match(Token_Class.Else));
            else_statements.Children.Add(Statements());
            else_statements.Children.Add(match(Token_Class.End));
            return else_statements;
        }
        Node Repeat_Statement()
        {
            Node repeat_statement = new Node("Repeat_Statement");
            repeat_statement.Children.Add(match(Token_Class.Repeat));
            repeat_statement.Children.Add(Statements());
            repeat_statement.Children.Add(match(Token_Class.Until));
            repeat_statement.Children.Add(Condition_Statement());
            return repeat_statement;
        }
        Node Condition_Statement()
        {
            Node condition_statement = new Node("Condition_Statement");
            condition_statement.Children.Add(Condition());
            if (Token_Class.And == TokenStream[InputPointer].token_type || Token_Class.Or == TokenStream[InputPointer].token_type)
            {
                condition_statement.Children.Add(Condition_Statement1());
            }
            
            return condition_statement;
        }

        Node Condition_Statement1()
        {
            Node condition_statement1 = new Node("Condition_Statement1");
            if (Token_Class.And == TokenStream[InputPointer].token_type || Token_Class.Or == TokenStream[InputPointer].token_type)
            {
                condition_statement1.Children.Add(Boolean_Operator());
                condition_statement1.Children.Add(Condition());
                condition_statement1.Children.Add(Condition_Statement1());
            }
                
            
            return condition_statement1;
        }

        Node Boolean_Operator()
        {
            Node boolean_operator = new Node("Boolean_Operator");
            if(Token_Class.And == TokenStream[InputPointer].token_type)
            {
                boolean_operator.Children.Add(match(Token_Class.And));
            }else if(Token_Class.Or == TokenStream[InputPointer].token_type)
            {
                boolean_operator.Children.Add(match(Token_Class.Or));
            }
            return boolean_operator;
        }

        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition; 
        }
        Node Condition_Operator()
        {
            Node condition_operator = new Node("Condition_Operator");
            if(Token_Class.NotEqualOp == TokenStream[InputPointer].token_type)
            {
                condition_operator.Children.Add(match(Token_Class.NotEqualOp));
            }else if (Token_Class.LessThanOp == TokenStream[InputPointer].token_type)
            {
                condition_operator.Children.Add(match(Token_Class.LessThanOp));
            }else if (Token_Class.GreaterThanOp == TokenStream[InputPointer].token_type)
            {
                condition_operator.Children.Add(match(Token_Class.GreaterThanOp));
            }else if (Token_Class.ConditionEqualOp == TokenStream[InputPointer].token_type)
            {
                condition_operator.Children.Add(match(Token_Class.ConditionEqualOp));
            }
            return condition_operator;
        }
        Node DataType()
        {
            Node dataType = new Node("DataType");
            Token_Class currToken = TokenStream[InputPointer].token_type;
            if(currToken == Token_Class.Int)
            {
                dataType.Children.Add(match(Token_Class.Int));
            }else if (currToken == Token_Class.Float)
            {
                dataType.Children.Add(match(Token_Class.Float));
            }else if (currToken == Token_Class.ReservedString)
            {
                dataType.Children.Add(match(Token_Class.ReservedString));
            }
            return dataType;
        }
        Node Arithmatic_Operator()
        {
            Node arithmatic_Operator = new Node("Arithmatic_Operator");
            if (Token_Class.PlusOp == TokenStream[InputPointer].token_type)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.PlusOp));
            }else if (Token_Class.MinusOp == TokenStream[InputPointer].token_type)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.MinusOp));
            }else if (Token_Class.MultiplyOp == TokenStream[InputPointer].token_type)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.MultiplyOp));
            }else if (Token_Class.DivideOp == TokenStream[InputPointer].token_type)
            {
                arithmatic_Operator.Children.Add(match(Token_Class.DivideOp));
            }
            return  arithmatic_Operator; 
        }







        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
