/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolittle.Build;

namespace Dolittle.Interaction.WebAssembly.Build
{
    /// <summary>
    /// Represents a system that is capable of copying files
    /// </summary>
    public class FileCopier
    {
        readonly string _destinationPath;
        private readonly IBuildMessages _buildMessages;

        /// <summary>
        /// Initializes a new instance of <see cref="FileCopier"/>
        /// </summary>
        /// <param name="buildMessages"><see cref="IBuildMessages"/> for build messages</param>
        /// <param name="destinationPath">Destination path the copier is for</param>
        public FileCopier(IBuildMessages buildMessages, string destinationPath)
        {
            _destinationPath = destinationPath;
            _buildMessages = buildMessages;
        }


        /// <summary>
        /// Copy files
        /// </summary>
        /// <param name="files">Params of enumerable of files</param>
        public void Copy(params IEnumerable<string>[] files)
        {
            var count = 0;

            foreach( var file in files.SelectMany(_ => _).Distinct() ) 
            {
                var filename = Path.GetFileName(file);
                var destination = Path.Combine(
                    _destinationPath,
                    filename
                );

                //Console.WriteLine($"Copy '{filename}' (Source: {Path.GetDirectoryName(file)}) to '{destination}'");
                File.Copy(file, destination, true);
                count++;
            }

            _buildMessages.Information($"Copied {count} to output folder '{_destinationPath}'");
        }
    }
}
