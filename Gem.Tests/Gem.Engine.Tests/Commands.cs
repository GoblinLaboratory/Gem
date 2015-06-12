﻿using Gem.Console;
using Gem.Console.Commands;
using Gem.Infrastructure.Functional;
using System;
using System.Collections.Generic;

namespace Gem.Engine.Tests
{

    internal interface ICommandClass { }

    public class ClassWithCommand : ICommandClass
    {
        public double Number { get; set; }

        [Command(command: "setnumber",
                 description: "provide a number as an argument")]
        private Result<object> FirstCommandCallback(ICommandHost host,
                                                    string command,
                                                    IList<string> arguments,
                                                    object executionResult)
        {
            if (arguments.Count == 1)
            {
                Number = Double.Parse(arguments[0]);
                return Result.Successful(Number);
            }
            else if (executionResult != null)
            {
                Number += (double)executionResult;
                return Result.Successful(Number);
            }
            return Result.Fail<object>("Wrong number of arguments");
        }

        [Command(command: "write",
                 description: "Writes the specified argument to the standard output stream")]
        public Result<object> ConsoleWriteCallback(ICommandHost host,
                                                   string command,
                                                   IList<string> arguments,
                                                   object executionResult)
        {
            if (arguments.Count == 1)
            {
                System.Console.WriteLine(arguments[0]);
                return Result.Successful(null);
            }
            else if (executionResult != null)
            {
                System.Console.WriteLine(executionResult);
            }
            return Result.Failed("Wrong number of arguments");
        }

        [Subcommand(parentCommand: "write",
                    subCommand: "color",
                    description: "[red|blue|green]")]
        private Result<object> SubCommandCallback(ICommandHost host,
                                            string command,
                                            IList<string> arguments,
                                            object executionResult)
        {
            return Result.Successful(null);
        }
    }

    public class ClassWithSubCommand
    {
        [Subcommand(parentCommand: "write",
        subCommand: "format",
        description: "[something]")]
        private Result<object> SubCommandCallback(ICommandHost host,
                                            string command,
                                            IList<string> arguments,
                                            object executionResult)
        {
            return Result.Successful(null);
        }
    }


    public class Calculator
    {

        #region Commands

        [Command(command: "calculate",
                 description: "Starts a calculation")]
        private Result<object> Calculate(ICommandHost host,
                                                   string command,
                                                   IList<string> arguments,
                                                   object executionResult)
        {
            if (arguments.Count == 1)
            {
                return ParseString(arguments[0]);
            }
            else if (executionResult != null)
            {
                double result;
                try
                {
                    result = Convert.ToDouble(executionResult);
                    return Result.Successful(result);
                }
                catch (Exception ex)
                {
                    Result.Failed("Invalid argument " + ex.Message);
                }
            }
            return Result.Failed("Wrong number of arguments");
        }

      
        [Subcommand(parentCommand: "calculate",
                    subCommand: "plus",
                    description: "Add a number to the previous")]
        private Result<object> Add(ICommandHost host,
                                            string command,
                                            IList<string> arguments,
                                            object executionResult)
        {
            return ParseCommand(arguments, executionResult, (res, arg) => res + arg);
        }

        [Subcommand(parentCommand: "calculate",
                    subCommand: "times",
                    description: "Multiplication")]
        private Result<object> Multiply(ICommandHost host,
                                            string command,
                                            IList<string> arguments,
                                            object executionResult)
        {
            return ParseCommand(arguments, executionResult, (res, arg) => res * arg);
        }

        [Subcommand(parentCommand: "calculate",
                    subCommand: "divide",
                    description: "Division")]
        private Result<object> Divide(ICommandHost host,
                                            string command,
                                            IList<string> arguments,
                                            object executionResult)
        {
            //check if argument==0
            return ParseCommand(arguments, executionResult, (res, arg) => res / arg);
        }

        [Subcommand(parentCommand: "calculate",
            subCommand: "minus",
            description: "Subtract a number from the previous")]
        private Result<object> Subtract(ICommandHost host,
                                            string command,
                                            IList<string> arguments,
                                            object executionResult)
        {
            return ParseCommand(arguments, executionResult, (res, arg) => res - arg);
        }

        #endregion

        #region Helpers

        private Result<object> ParseString(string number)
        {
            double result;

            return
             (Double.TryParse(number, out result)) ?
             Result.Successful(result) :
             Result.Failed("Invalid argument");
        }

        private Result<object> ParseCommand(IList<string> arguments,
                                            object executionResult, Func<double, double, double> calculation)
        {
            double argument = 0;
            if (arguments.Count == 1)
            {
                if (!Double.TryParse(arguments[0], out argument))
                    return Result.Failed("Wrong number of arguments");
            }
            if (executionResult != null)
            {
                double result;
                try
                {
                    result = Convert.ToDouble(executionResult);
                    return Result.Successful(calculation(result, argument));
                }
                catch (Exception ex)
                {
                    Result.Failed("Invalid argument " + ex.Message);
                }
            }
            return Result.Failed("Wrong number of arguments");
        }

        #endregion
    }

}
