﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolittle.Collections;
using Newtonsoft.Json;

namespace Dolittle.Interaction.WebAssembly.Packager
{


    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new Configuration(args);
            var paths = new AssemblyPaths(configuration);
            var managedFilesFileCopier = new FileCopier(configuration.ManagedOutputPath);
            var staticFilesFileCopier = new FileCopier(configuration.OutputPath);
            var assemblies = new Assemblies(configuration, paths);
            var artifactsEmbedder = new ArtifactsEmbedder(configuration, assemblies);

            managedFilesFileCopier.Copy(assemblies.AllImportedAssemblyPaths);
            if( !configuration.IsRelease ) managedFilesFileCopier.Copy(assemblies.AllImportedAssemblyDebugSymbolPaths);

            var configurationPath = configuration.IsRelease?"release":"debug";

            staticFilesFileCopier.Copy(new[] {
                Path.Combine(paths.Sdk, configurationPath, "mono.js"),
                Path.Combine(paths.Sdk, configurationPath, "mono.wasm")
            });

            if( !configuration.IsRelease ) 
            {
                staticFilesFileCopier.Copy(new[] {
                    Path.Combine(paths.Sdk, "debug", "mono.wasm.map"),
                    Path.Combine(paths.Sdk, "debug", "mono.wast")
                });
            }

            var managedFiles = new List<string>();
            managedFiles.AddRange(assemblies.AllImportedAssemblyPaths);
            if( !configuration.IsRelease ) managedFiles.AddRange(assemblies.AllImportedAssemblyDebugSymbolPaths);

            var librariesFilePath = Path.Combine(configuration.OutputPath, "libraries.json");
            File.WriteAllText(librariesFilePath, JsonConvert.SerializeObject(assemblies.Libraries, Formatting.Indented));

            var assembliesFilePath = Path.Combine(configuration.OutputPath, "assemblies.json");
            var fileList = string.Join(",\n\t\t", managedFiles.Select(_ => $"\"{Path.GetFileName(_)}\"").ToArray());
            File.WriteAllText(assembliesFilePath, $"[\n\t\t{fileList}\n]");

            var monoConfigPath = Path.Combine(configuration.OutputPath, "mono-config.js");
            
            File.WriteAllText(monoConfigPath,
                $"config = {{\n\tvfs_prefix: 'managed',\n\tdeploy_prefix: 'managed',\n\tenable_debugging: 0, \n\tfile_list: [\n\t\t{fileList}\n\t ],\n\tadd_bindings: function() {{ \n\t\tModule.mono_bindings_init ('[WebAssembly.Bindings]WebAssembly.Runtime');\n\t}}\n}}"
            );

            var monoJsSource = Path.Combine(paths.Sdk, configurationPath, "mono.js");
            var monoJsDestination = Path.Combine(configuration.OutputPath, "mono.js");
            var monoJs = File.ReadAllText(monoJsSource);
            monoJs = monoJs.Replace("this.mono_wasm_runtime_is_ready=true;debugger","this.mono_wasm_runtime_is_ready=true;");
            monoJs = monoJs.Replace(
"  			this.mono_wasm_runtime_is_ready = true;\n"+
"  			debugger;\n",
"  			this.mono_wasm_runtime_is_ready = true;\n");

            File.WriteAllText(monoJsDestination, monoJs);

            artifactsEmbedder.Perform();

            assemblies.SkippedImports.ForEach(_ => Console.WriteLine($"  Skipped imports for '{_.File}' with reason '{_.Reason}'"));
        }
    }
}
