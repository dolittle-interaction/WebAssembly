﻿using System;
using Dolittle.Booting;
using Dolittle.Interaction.WebAssembly.Interop;
using Dolittle.Logging;
using Dolittle.Execution;
using Dolittle.Tenancy;
using Dolittle.Types;
using Dolittle.ResourceTypes;
using Dolittle.Collections;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            var before = DateTime.Now;

            var bootResult = Bootloader.Configure(_ => _
                .WithEntryAssembly(typeof(Program).Assembly)
                .WithAssembliesSpecifiedIn(typeof(Program).Assembly)
                .SynchronousScheduling()
                .UseLogAppender(new CustomLogAppender())
            ).Start();

            var container = bootResult.Container;
            var logger = container.Get<ILogger>();
            logger.Information("We're running");

            var executionContextManager = container.Get<IExecutionContextManager>();
            executionContextManager.CurrentFor(TenantId.Development);

            var interop = container.Get<IJSRuntime>();
            interop.Invoke("window._dolittleLoaded");
        }
    }
}