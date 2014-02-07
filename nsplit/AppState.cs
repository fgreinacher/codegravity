// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using nsplit.Api;

#endregion

namespace nsplit
{
    internal static class AppState
    {
        public static AnalyzerTask Task { get; set; }
        public static Storage Storage { get; set; }
    }
}