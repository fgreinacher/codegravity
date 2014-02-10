// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using Gma.CodeVisuals.WebApi.DependencyForceGraph;

#endregion

namespace Gma.CodeVisuals.Generator
{
    internal class Options
    {
        [ValueList(typeof (List<string>))]
        public IList<string> Assemblies { get; set; }

        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [OptionList('p', "path", Separator = ';', HelpText = "Specify search path, separated by a semicolon.")]
        public IList<string> Path { get; set; }

        [Option('o', "output", DefaultValue = Storage.DefaultDirectoryName, HelpText = "Oputput folder for graph files.")]
        public string Output { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}