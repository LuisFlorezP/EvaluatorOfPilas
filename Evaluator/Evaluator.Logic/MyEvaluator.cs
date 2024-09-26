using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;

namespace Evaluator.Logic
{
    public class MyEvaluator
    {
        public static double Evaluate(string infix)
        {
            var postfix = ToPostfix(infix);
            return Calculate(postfix);
        }

        private static double Calculate(string postfix)
        {
            var stack = new Stack<double>(100);
            for (int i = 0; i < postfix.Length; i++)
            {
                if (IsOperator(postfix[i]))
                {
                    double number2 = stack.Pop();
                    double number1 = stack.Pop();
                    double result = Calculate(number1, postfix[i], number2);
                    stack.Push(result);
                }
                else
                {
                    int index = i;
                    int length = 1;
                    i++;
                    while (true)
                    {
                        if (postfix[i] != 'N')
                        {
                            length++;
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    string figure = string.Empty;
                    for (int j = index; j < length + index; j++)
                    {
                        figure += postfix[j];
                    }
                    double number = ValidateSpots(figure);
                    stack.Push(number);
                }
            }
            return stack.Pop();
        }

        private static double ValidateSpots(string figure)
        {
            bool validation = false;
            int locate = 0;
            for (int i = 0; i < figure.Length; i++)
            {
                if (figure[i] == '.')
                {
                    validation = true;
                    locate = i;
                    break;
                }
            }
            if (validation)
            {
                return Convert.ToDouble(figure) / Math.Pow(10, (figure.Length - 1 - locate));
            }
            else
            {
                return Convert.ToDouble(figure);
            }
        }

        private static double Calculate(double number1, char @operator, double number2)
        {
            switch (@operator)
            {
                case '^': return Math.Pow(number1, number2);
                case '*': return number1 * number2;
                case '/': return number1 / number2;
                case '+': return number1 + number2;
                case '-': return number1 - number2;
                default: throw new Exception("Expression is not valid.");
            }
        }

        private static string ToPostfix(string infix)
        {
            var stack = new Stack<char>(100);
            var postfix = string.Empty;
            ValidateBrackets(infix);
            ValidatePossibleOperatorsExtremes(infix[0], infix[infix.Length - 1]);
            for (int i = 0; i < infix.Length; i++)
            {
                if (IsOperator(infix[i]))   
                {
                    bool changeOperator = false;
                    if (i != infix.Length - 1)
                    {
                        if (infix[i] == '(')
                        {
                            ValidatePossibleNextOperator(infix[i + 1]);
                        }
                        else
                        {
                            if (infix[i] == ')')
                            {
                                ValidatePossibleNextBracket(infix[i + 1]);
                            }
                            else
                            {
                                if (i != infix.Length - 2)
                                {
                                    ValidateOperators(infix[i + 1], infix[i + 2]);
                                    changeOperator = ValidateOperators(infix[i + 1]);
                                }
                            }
                        }
                    }
                    if (changeOperator == true)
                    {
                        i++;
                    }
                    if (stack.IsEmpty)
                    {
                        stack.Push(infix[i]);
                    }
                    else
                    {
                        if (infix[i] == ')')
                        {
                            do
                            {
                                postfix += stack.Pop();
                            } while (stack.GetItemInTop() != '(');
                            stack.Pop();
                        }
                        else
                        {
                            if (PriorityInExpression(infix[i]) > PriorityInStack(stack.GetItemInTop()))
                            {
                                stack.Push(infix[i]);
                            }
                            else
                            {
                                postfix += stack.Pop();
                                stack.Push(infix[i]);
                            }
                        }
                    }
                }
                else
                {
                    if (i != infix.Length - 1) 
                    {
                        if (infix[i] != '.')
                        {
                            int index = i;
                            int lenght = 1;
                            i++;
                            ValidateSpots(infix, i);
                            while (infix[i] != '(' && infix[i] != ')' && infix[i] != '^' && infix[i] != '/' && infix[i] != '*' && infix[i] != '+' && infix[i] != '-')
                            {
                                if (i == infix.Length - 1)
                                {
                                    lenght++;
                                    i++;
                                    break;
                                }
                                lenght++;
                                i++;
                            }
                            for (int j = index; j < lenght + index; j++)
                            {
                                postfix += infix[j];
                            }
                            if (postfix[postfix.Length - 1] == '.')
                            {
                                throw new Exception("Expression is not valid.");
                            }
                            postfix += 'N';
                            i--;
                        }
                        else
                        {
                            throw new Exception("Expression is not valid.");
                        }
                    }
                    else
                    {
                        if (infix[i] != '.')
                        {
                            postfix += infix[i];
                            postfix += 'N';
                        }
                        else
                        {
                            throw new Exception("Expression is not valid.");
                        }
                    }
                }
            }
            while (!stack.IsEmpty)
            {
                postfix += stack.Pop();
            }
            return postfix;
        }

        private static void ValidateBrackets(string infix)
        {
            int openBracket = 0, closedBracket = 0;
            for (int i = 0; i < infix.Length; i++)
            {
                if (infix[i] == '(')
                {
                    openBracket++;
                }
                else
                {
                    if (infix[i] == ')')
                    {
                        closedBracket++;
                    }
                }
            }
            if (openBracket != closedBracket)
            {
                throw new Exception("Expression is not valid.");
            }
        }

        private static void ValidatePossibleOperatorsExtremes(char item1, char item2)
        {
            if (item1 == '^' || item1 == '/' || item1 == '*' || item1 == '+' || item1 == '-')
            {
                throw new Exception("Expression is not valid.");
            }
            if (item2 == '^' || item2 == '/' || item2 == '*' || item2 == '+' || item2 == '-')
            {
                throw new Exception("Expression is not valid.");
            }
        }

        private static bool IsOperator(char item)
        {
            if (item == '(' || item == ')' || item == '^' || item == '/' || item == '*' || item == '+' || item == '-')
            {
                return true;
            }
            return false;
        }

        private static void ValidatePossibleNextOperator(char item)
        {
            if (item == '^' || item == '/' || item == '*' || item == '+' || item == '-')
            {
                throw new Exception("Expression is not valid.");
            }
        }

        private static void ValidatePossibleNextBracket(char item)
        {
            if (item != ')' && item != '^' && item != '/' && item != '*' && item != '+' && item != '-')
            {
                throw new Exception("Expression is not valid.");
            }
        }

        private static void ValidateOperators(char item1, char item2)
        {
            if (IsOperator(item1, item2))
            {
                throw new Exception("Expression is not valid.");
            }
        }

        private static bool IsOperator(char item1, char item2)
        {
            if (item1 == '^' || item1 == '/' || item1 == '*' || item1 == '+' || item1 == '-')
            {
                if (item2 == '^' || item2 == '/' || item2 == '*' || item2 == '+' || item2 == '-')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private static bool ValidateOperators(char item)
        {
            if (IsOperatorArithmetic(item))
            {
                return true;
            }
            return false;
        }

        private static bool IsOperatorArithmetic(char item)
        {
            if (item == '^' || item == '/' || item == '*' || item == '+' || item == '-')
            {
                return true;
            }
            return false;
        }

        private static int PriorityInExpression(char @operator)
        {
            switch (@operator)
            {
                case '^': return 4;
                case '*': return 2;
                case '/': return 2;
                case '+': return 1;
                case '-': return 1;
                case '(': return 5;
                default: throw new Exception("Expression is not valid.");
            }
        }

        private static int PriorityInStack(char @operator)
        {
            switch (@operator)
            {
                case '^': return 3;
                case '*': return 2;
                case '/': return 2;
                case '+': return 1;
                case '-': return 1;
                case '(': return 0;
                default: throw new Exception("Expression is not valid.");
            }
        }

        private static void ValidateSpots(string infix, int i)
        {
            int countSpot = 0;

            while (infix[i] != '(' && infix[i] != ')' && infix[i] != '^' && infix[i] != '/' && infix[i] != '*' && infix[i] != '+' && infix[i] != '-')
            {
                if (i == infix.Length - 1)
                {
                    if (infix[i] == '.')
                    {
                        throw new Exception("Expression is not valid.");
                    }
                    break;
                }
                else
                {
                    if (infix[i] == '.')
                    {
                        countSpot++;
                    }
                    i++;
                }
            }
            if (countSpot > 1)
            {
                throw new Exception("Expression is not valid.");
            }
        }
    }
}