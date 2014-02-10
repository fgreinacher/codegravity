// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

#endregion

namespace nsplit.Api
{
    public class Storage
    {
        private const string FilePattern = "{0}{1}.json";
        private readonly string m_RootPath;
        private readonly HashSet<char> m_IvalidChars;


        public Storage(string rootPath)
        {
            m_RootPath = rootPath;
            m_IvalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());
        }

        public void Save(GraphDto data)
        {
            Save(data.Name, data);
        }

        public void Save(string name, GraphDto data)
        {
            if (!Directory.Exists(m_RootPath)) Directory.CreateDirectory(m_RootPath);

            string path = GetUniqueFilePath(name);
            using (var writer = File.CreateText(path))
            {
                JsonSerializer.Create().Serialize(writer, data);
            }
        }

        public GraphDto Load(string name)
        {
            string path = GetFilePath(name);
            using (var reader = File.OpenText(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return JsonSerializer.Create().Deserialize<GraphDto>(jsonReader);
            }
        }

        public IEnumerable<string> GetNames()
        {
            var searchPattern = string.Format(FilePattern, "*", string.Empty);
            return Directory
                    .GetFiles(m_RootPath, searchPattern)
                    .Select(Path.GetFileNameWithoutExtension);
        }

        private string GetUniqueFilePath(string name)
        {
            var normalizedName = NormalizeName(name);
            string suffix = string.Empty;
            int i = 1;
            do
            {
                string fileName = CombinePath(normalizedName, suffix);
                if (!File.Exists(fileName)) return fileName;
                suffix = string.Format("_{0}", i++);
            } while (true);
        }

        private string GetFilePath(string name)
        {
            var normalizedName = NormalizeName(name);
            return CombinePath(normalizedName, string.Empty);
        }

        private string CombinePath(string normalizedName, string suffix)
        {
            return Path.Combine(m_RootPath, string.Format(FilePattern, normalizedName, suffix));
        }

        private string NormalizeName(string name)
        {
            return new string(
                name
                .Where(ch=>!m_IvalidChars.Contains(ch))
                .ToArray());
        }

        public const string DefaultDirectoryName = "_DependencyForceGraphs";
        public static string GetDefaultPath()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            return Path.Combine(exePath, DefaultDirectoryName);
        }

        public void Save(object graph)
        {
            var graphDto = AutoMapper.Mapper.DynamicMap<GraphDto>(graph);
            Save(graphDto);
        }
    }
}