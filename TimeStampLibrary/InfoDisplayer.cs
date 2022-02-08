// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using PluginContracts;
using System;
using System.Linq;
using System.Reflection;
//using Microsoft.Data.Sqlite;

namespace TimestampedPlugin {
    public class InfoDisplayer : IPlugin {

        public string GetName() => "InfoDisplayer";

        public void Print() {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Hello World");
            Console.ResetColor();

        }
    }
}