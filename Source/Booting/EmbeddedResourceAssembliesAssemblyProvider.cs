/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dolittle.Assemblies;
using Dolittle.Collections;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Interaction.WebAssembly.Booting
{

    /// <summary>
    /// EqualityComparer for <see cref="Library"/>
    /// </summary>
    public class LibraryComparer : IEqualityComparer<Library>
    {
        /// <inheritdoc/>
        public bool Equals(Library x, Library y)
        {
            return x.Name.Equals(y.Name);
        }

        /// <inheritdoc/>
        public int GetHashCode(Library obj)
        {
            return obj.GetHashCode();
        }
    }


    /// <summary>
    /// Represents a <see cref="ICanProvideAssemblies">assembly provider</see> that will provide assemblies based on
    /// an embedded JSON file called 'assemblies.json' that holds an array of strings
    /// </summary>
    public class EmbeddedResourceAssembliesAssemblyProvider : ICanProvideAssemblies
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EmbeddedResourceAssembliesAssemblyProvider"/>
        /// </summary>
        public EmbeddedResourceAssembliesAssemblyProvider(Assembly assembly)
        {
            var name = $"{assembly.GetName().Name}.libraries.json";
            System.Console.WriteLine($"Using assemblies defined in resource : {name}");
            var manifestResourceNames = assembly.GetManifestResourceNames();

            if( !manifestResourceNames.Any(_ => _.Trim() == name) ) 
            {
                Libraries = new Library[0];
                return;
            }

            var resource = assembly.GetManifestResourceStream(name);
            using( var reader = new StreamReader(resource) ) 
            {
                var json = reader.ReadToEnd();

                System.Console.WriteLine($"Libraries : {json}");


                var libraries = JsonConvert.DeserializeObject<dynamic[]>(json);
                libraries.ForEach(_ => {
                    System.Console.WriteLine($"{_.Type.Value}");

                });

                var comparer = new LibraryComparer();

                Libraries = libraries
                    .Select(_ => new Library(
                        _.Type.Value, 
                        _.Name.Value, 
                        _.Version.Value, 
                        string.Empty,
                        new Dependency[0], false))
                    .Distinct(comparer).ToArray();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Library> Libraries { get; }


        /// <inheritdoc/>
        public Assembly GetFrom(Library library)
        {
            return Assembly.Load(library.Name);
        }
    }
}