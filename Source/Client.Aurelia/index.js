/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
import { CommandCoordinator } from '@dolittle/commands';
import { CommandCoordinator as WASMCommandCoordinator } from '@dolittle/webassembly.commands';
import { QueryCoordinator } from '@dolittle/queries';
import { QueryCoordinator as WASMQueryCoordinator } from '@dolittle/webassembly.queries';
import { MongoDB } from '@dolittle/readmodels.mongodb.webassembly';
import { EventProcessorOffsetRepository } from '@dolittle/runtime.events.webassembly.dev/Processing';
import { EventStore } from '@dolittle/runtime.events.webassembly.dev/Store';
import { storage } from '@dolittle/runtime.events.webassembly.dev';

import { JSRuntime } from '@dolittle/webassembly.interop';

export function configure(aurelia, config) {
    aurelia.container.registerInstance(CommandCoordinator, new WASMCommandCoordinator());
    aurelia.container.registerInstance(QueryCoordinator, new WASMQueryCoordinator());

    config = config || {};
    config.entryPoint = config.entryPoint || '';
    config.assemblies = config.assemblies || [];
    config.monoScript = config.monoScript || 'mono.js';
    config.offline = config.offline || false;
    config.debugging = config.debugging || false;

    if (config.entryPoint == '') throw "Missing entrypoint in Dolittle WebAssembly plugin configuration";
    if (config.assemblies.length == 0) throw "Missing assemblies in Dolittle WebAssembly plugin configuration";

    console.log(`Using '${config.entryPoint}' as entrypoint for WebAssembly `)

    storage.preload()
            .then(_ => EventProcessorOffsetRepository.preload())
            .then(_ => EventStore.preload()).catch(error => {
        console.error('Error preloading', error);
    }).then(_ => {
        // Now the rest of the system should be ready for booting
        window.Module = {};

        window.Module.onRuntimeInitialized = () => {
            MONO.mono_load_runtime_and_bcl(
                'managed',
                'managed',
                config.debugging?1:0,
                config.assemblies,
                () => {
                    Module.mono_bindings_init("[WebAssembly.Bindings]WebAssembly.Runtime");
                    BINDING.call_static_method(config.entryPoint, []);
                }
            );
        };

        let monoScript = document.createElement('script');
        monoScript.async = true;
        monoScript.src = config.monoScript;
        document.body.appendChild(monoScript);

        if (config.offline === true) {
            navigator.serviceWorker.register('service-worker.js');
        }

        window._mongoDB = new MongoDB();
        window._eventStore = {
            eventProcessorOffsetRepository: new EventProcessorOffsetRepository(),
            eventStore: new EventStore()
        }
    });
}