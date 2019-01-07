/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dolittle.Assemblies;
using Mono.Cecil;

namespace Dolittle.Interaction.WebAssembly.Packager
{
    /// <summary>
    /// Represents a system that knows how to discover assemblies
    /// </summary>
    public class Assemblies
    {
        readonly List<Assembly> _rootAssemblies = new List<Assembly>();
        readonly Configuration _configuration;
        readonly AssemblyPaths _assemblyPaths;
        IEnumerable<string> _rootAssemblyPaths;
        IEnumerable<string> _allImportedAssemblyPaths;
        IEnumerable<string> _allImportedAssemblyDebugSymbolPaths;

        /// <summary>
        /// Initializes a new instance of <see cref="Assemblies"/>
        /// </summary>
        /// <param name="configuration">Current <see cref="Configuration"/></param>
        /// <param name="assemblyPaths">Paths for assemblies</param>
        public Assemblies(Configuration configuration, AssemblyPaths assemblyPaths)
        {
            _configuration = configuration;
            _assemblyPaths = assemblyPaths;
            PopulateRootAssemblies();
            ImportAllAssemblies();
        }

        void ImportAllAssemblies()
        {
            var importedFiles = new HashSet<string>();
            foreach (var rootAssemblyPath in _rootAssemblyPaths)
            {
                Import(rootAssemblyPath, importedFiles);
            }

            Import("WebAssembly.Bindings.dll", importedFiles);
            Import("netstandard.dll", importedFiles);

            _allImportedAssemblyPaths = importedFiles.Where(_ => Path.GetExtension(_).ToLower() == ".dll").Distinct();
            _allImportedAssemblyDebugSymbolPaths = importedFiles.Where(_ => Path.GetExtension(_).ToLower() == ".pdb").Distinct();
        }

        /// <summary>
        /// Gets all the root assemblies
        /// </summary>
        public IEnumerable<Assembly> RootAssemblies => _rootAssemblies;

        /// <summary>
        /// Gets all the paths to all the root assemblies
        /// </summary>
        public IEnumerable<string> RootAssemblyPaths => _rootAssemblyPaths;

        /// <summary>
        /// Gets the paths to all the imported assemblies (recursively imported)
        /// </summary>
        public IEnumerable<string> AllImportedAssemblyPaths => _allImportedAssemblyPaths;

        /// <summary>
        /// Gets the paths to all the imported assemblies debug symbols (recursively imported)
        /// </summary>
        public IEnumerable<string> AllImportedAssemblyDebugSymbolPaths => _allImportedAssemblyDebugSymbolPaths;

        void Import(string file, HashSet<string> files)
        {
            var bestMatchedFile = _assemblyPaths.FindBestMatchFor(file);
            try
            { 
                var filesToImport = GetFilesFor(_assemblyPaths, bestMatchedFile);
                foreach (var fileToAdd in filesToImport)
                {
                    if (!files.Add(fileToAdd)) return;
                }

                var module = ModuleDefinition.ReadModule(bestMatchedFile);
                foreach (var assemblyReference in module.AssemblyReferences)
                {
                    var referenceFile = $"{assemblyReference.Name}.dll";
                    Import(referenceFile, files);
                }
            } 
            catch
            {
                Console.WriteLine($"Skipping references for {file} - {bestMatchedFile}");
            }
        }

        IEnumerable<string> GetFilesFor(AssemblyPaths paths, string path)
        {
            var files = new List<string>();
            path = paths.FindBestMatchFor(path);
            if (File.Exists(path))
            {
                files.Add(path);

                var pdbFile = Path.ChangeExtension(path, ".pdb");
                if (File.Exists(pdbFile)) files.Add(pdbFile);
            }
            return files;
        }

        void PopulateRootAssemblies()
        {
            var rootAssembly = Assembly.LoadFrom(_configuration.EntryAssemblyPath);
            _rootAssemblies.Add(rootAssembly);

            var assemblyContext = new AssemblyContext(rootAssembly);
            _rootAssemblies.AddRange(assemblyContext.GetReferencedAssemblies());

            _rootAssemblyPaths = _rootAssemblies.Select(_ =>
            {
                var path = string.Empty;
                if (_.CodeBase == null) path = _assemblyPaths.FindBestMatchFor($"{_.GetName().Name}.dll");
                else
                {
                    var uri = new Uri(_.CodeBase);
                    path = _assemblyPaths.FindBestMatchFor(uri.AbsolutePath);
                }
                return path;
            });
        }

    }
}