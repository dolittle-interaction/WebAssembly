// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Runtime.Commands;

namespace Dolittle.Interaction.WebAssembly.Commands
{
    /// <summary>
    /// Helper class for deserializing JSON command requests from the interaction layer.
    /// </summary>
    public class CommandRequestWithDefaultGeneration : CommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRequestWithDefaultGeneration"/> class.
        /// </summary>
        /// <param name="correlationId"><see cref="Dolittle.Runtime.Commands.CommandRequest.CorrelationId"/> for the request.</param>
        /// <param name="type"><see cref="Dolittle.Artifacts.ArtifactId"/> for the request.</param>
        /// <param name="content">Content of the command.</param>
        public CommandRequestWithDefaultGeneration(CorrelationId correlationId, ArtifactId type, IDictionary<string, object> content)
            : base(correlationId, type, ArtifactGeneration.First, content)
        {
        }
    }
}