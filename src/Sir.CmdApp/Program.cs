﻿using Sir.Store;
using System;
using System.Diagnostics;
using System.IO;

namespace Sir.CmdApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sessionFactory = new SessionFactory(Directory.GetCurrentDirectory());
            var store = new Store.Writer(sessionFactory, new Tokenizer());
            var timer = new Stopwatch();
            string[] userInput = args.Length == 0 ? null : args;
            var methods = Inspector.GetMethods(typeof(App));
            var app = new App();

            while (true)
            {
                if (userInput == null)
                {
                    var line = Console.ReadLine();
                    userInput = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                }

                if (userInput.Length == 0 || userInput[0] == "q" || userInput[0] == "quit")
                {
                    break;
                }
                else
                {
                    var method = methods.Resolve(userInput);
                    if (method != null)
                    {
                        timer.Restart();
                        method.Invoke(app, new object[] { userInput, store });
                        timer.Stop();
                    }

                }

                Console.WriteLine("{0} ticks", timer.Elapsed.Ticks);
                Console.WriteLine();

                userInput = null;
            }
        }
    }
}
