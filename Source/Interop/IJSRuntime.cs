// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Interaction.WebAssembly.Interop
{
    /// <summary>
    /// Represents the JavaScript runtime.
    /// </summary>
    public interface IJSRuntime
    {
        /// <summary>
        /// Invoke a JavaScript function.
        /// </summary>
        /// <param name="identifier">Fully qualified identifier of the function to call.</param>
        /// <param name="arguments">Arguments to pass.</param>
        void Invoke(string identifier, params object[] arguments);

        /// <summary>
        /// Invoke an asynchronous function, typically based on async/await or promise/continuations.
        /// </summary>
        /// <typeparam name="T">Expected return type.</typeparam>
        /// <param name="identifier">Fully qualified identifier of the function to call.</param>
        /// <param name="arguments">Arguments to pass.</param>
        /// <returns>Awaitable task.</returns>
        Task<T> Invoke<T>(string identifier, params object[] arguments);

        /// <summary>
        /// Indicate an invocation was successful with a given result.
        /// </summary>
        /// <param name="invocationId">Id of the task representing the invocation.</param>
        /// <param name="resultAsJson">Result in the form of a JSON.</param>
        void Succeeded(Guid invocationId, string resultAsJson);

        /// <summary>
        /// Indicate an invocation was successful with a given result.
        /// </summary>
        /// <param name="invocationId">Id of the task representing the invocation.</param>
        /// <param name="exception">The exception / error that occurred.</param>
        void Failed(Guid invocationId, string exception);
    }
}