"use strict";
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
Object.defineProperty(exports, "__esModule", { value: true });
const commands_1 = require("@dolittle/commands");
const webassembly_commands_1 = require("@dolittle/webassembly.commands");
const queries_1 = require("@dolittle/queries");
const webassembly_queries_1 = require("@dolittle/webassembly.queries");
const readmodels_mongodb_webassembly_1 = require("@dolittle/readmodels.mongodb.webassembly");
const Processing_1 = require("@dolittle/runtime.events.webassembly.dev/Processing");
const Store_1 = require("@dolittle/runtime.events.webassembly.dev/Store");
const runtime_events_webassembly_dev_1 = require("@dolittle/runtime.events.webassembly.dev");
function configure(aurelia, config) {
    aurelia.container.registerInstance(commands_1.CommandCoordinator, new webassembly_commands_1.CommandCoordinator());
    aurelia.container.registerInstance(queries_1.QueryCoordinator, new webassembly_queries_1.QueryCoordinator());
    config = config || {};
    config.entryPoint = config.entryPoint || '';
    config.assemblies = config.assemblies || [];
    config.monoScript = config.monoScript || 'mono.js';
    config.offline = config.offline || false;
    config.debugging = config.debugging || false;
    if (config.entryPoint === '')
        throw new Error('Missing entrypoint in Dolittle WebAssembly plugin configuration');
    if (config.assemblies.length === 0)
        throw new Error('Missing assemblies in Dolittle WebAssembly plugin configuration');
    console.log(`Using '${config.entryPoint}' as entrypoint for WebAssembly `);
    runtime_events_webassembly_dev_1.storage.preload()
        .then(_ => Processing_1.EventProcessorOffsetRepository.preload())
        .then(_ => Store_1.EventStore.preload()).catch((error) => {
        console.error('Error preloading', error);
    }).then(_ => {
        // Now the rest of the system should be ready for booting
        window.Module = {};
        window.Module.onRuntimeInitialized = () => {
            MONO.mono_load_runtime_and_bcl('managed', 'managed', config.debugging ? 1 : 0, config.assemblies, () => {
                Module.mono_bindings_init('[WebAssembly.Bindings]WebAssembly.Runtime');
                BINDING.call_static_method(config.entryPoint, []);
            });
        };
        const monoScript = document.createElement('script');
        monoScript.async = true;
        monoScript.src = config.monoScript;
        document.body.appendChild(monoScript);
        if (config.offline === true) {
            navigator.serviceWorker.register('service-worker.js');
        }
        window._mongoDB = new readmodels_mongodb_webassembly_1.MongoDB();
        window._eventStore = {
            eventProcessorOffsetRepository: new Processing_1.EventProcessorOffsetRepository(),
            eventStore: new Store_1.EventStore()
        };
    });
}
exports.configure = configure;

//# sourceMappingURL=data:application/json;charset=utf8;base64,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QUMsT0FBTyxLQUFLLElBQUksRUFBRTtZQUN6QixTQUFTLENBQUMsYUFBYSxDQUFDLFFBQVEsQ0FBQyxtQkFBbUIsQ0FBQyxDQUFDO1NBQ3pEO1FBRUEsTUFBYyxDQUFDLFFBQVEsR0FBRyxJQUFJLHdDQUFPLEVBQUUsQ0FBQztRQUN4QyxNQUFjLENBQUMsV0FBVyxHQUFHO1lBQzFCLDhCQUE4QixFQUFFLElBQUksMkNBQThCLEVBQUU7WUFDcEUsVUFBVSxFQUFFLElBQUksa0JBQVUsRUFBRTtTQUMvQixDQUFDO0lBQ04sQ0FBQyxDQUFDLENBQUM7QUFDWCxDQUFDO0FBcERELDhCQW9EQyIsImZpbGUiOiJpbmRleC5qcyIsInNvdXJjZXNDb250ZW50IjpbIi8vIENvcHlyaWdodCAoYykgRG9saXR0bGUuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4vLyBMaWNlbnNlZCB1bmRlciB0aGUgTUlUIGxpY2Vuc2UuIFNlZSBMSUNFTlNFIGZpbGUgaW4gdGhlIHByb2plY3Qgcm9vdCBmb3IgZnVsbCBsaWNlbnNlIGluZm9ybWF0aW9uLlxuXG5pbXBvcnQgeyBBdXJlbGlhIH0gZnJvbSAnYXVyZWxpYS1mcmFtZXdvcmsnO1xuaW1wb3J0IHsgQ29tbWFuZENvb3JkaW5hdG9yIH0gZnJvbSAnQGRvbGl0dGxlL2NvbW1hbmRzJztcbmltcG9ydCB7IENvbW1hbmRDb29yZGluYXRvciBhcyBXQVNNQ29tbWFuZENvb3JkaW5hdG9yIH0gZnJvbSAnQGRvbGl0dGxlL3dlYmFzc2VtYmx5LmNvbW1hbmRzJztcbmltcG9ydCB7IFF1ZXJ5Q29vcmRpbmF0b3IgfSBmcm9tICdAZG9saXR0bGUvcXVlcmllcyc7XG5pbXBvcnQgeyBRdWVyeUNvb3JkaW5hdG9yIGFzIFdBU01RdWVyeUNvb3JkaW5hdG9yIH0gZnJvbSAnQGRvbGl0dGxlL3dlYmFzc2VtYmx5LnF1ZXJpZXMnO1xuaW1wb3J0IHsgTW9uZ29EQiB9IGZyb20gJ0Bkb2xpdHRsZS9yZWFkbW9kZWxzLm1vbmdvZGIud2ViYXNzZW1ibHknO1xuaW1wb3J0IHsgRXZlbnRQcm9jZXNzb3JPZmZzZXRSZXBvc2l0b3J5IH0gZnJvbSAnQGRvbGl0dGxlL3J1bnRpbWUuZXZlbnRzLndlYmFzc2VtYmx5LmRldi9Qcm9jZXNzaW5nJztcbmltcG9ydCB7IEV2ZW50U3RvcmUgfSBmcm9tICdAZG9saXR0bGUvcnVudGltZS5ldmVudHMud2ViYXNzZW1ibHkuZGV2L1N0b3JlJztcbmltcG9ydCB7IHN0b3JhZ2UgfSBmcm9tICdAZG9saXR0bGUvcnVudGltZS5ldmVudHMud2ViYXNzZW1ibHkuZGV2JztcblxuaW1wb3J0IHsgSlNSdW50aW1lIH0gZnJvbSAnQGRvbGl0dGxlL3dlYmFzc2VtYmx5LmludGVyb3AnO1xuXG5kZWNsYXJlIHZhciBNb2R1bGU6IGFueTtcbmRlY2xhcmUgdmFyIE1PTk86IGFueTtcbmRlY2xhcmUgdmFyIEJJTkRJTkc6IGFueTtcblxuZXhwb3J0IGZ1bmN0aW9uIGNvbmZpZ3VyZShhdXJlbGlhOiBBdXJlbGlhLCBjb25maWc6IGFueSkge1xuICAgIGF1cmVsaWEuY29udGFpbmVyLnJlZ2lzdGVySW5zdGFuY2UoQ29tbWFuZENvb3JkaW5hdG9yLCBuZXcgV0FTTUNvbW1hbmRDb29yZGluYXRvcigpKTtcbiAgICBhdXJlbGlhLmNvbnRhaW5lci5yZWdpc3Rlckluc3RhbmNlKFF1ZXJ5Q29vcmRpbmF0b3IsIG5ldyBXQVNNUXVlcnlDb29yZGluYXRvcigpKTtcblxuICAgIGNvbmZpZyA9IGNvbmZpZyB8fCB7fTtcbiAgICBjb25maWcuZW50cnlQb2ludCA9IGNvbmZpZy5lbnRyeVBvaW50IHx8ICcnO1xuICAgIGNvbmZpZy5hc3NlbWJsaWVzID0gY29uZmlnLmFzc2VtYmxpZXMgfHwgW107XG4gICAgY29uZmlnLm1vbm9TY3JpcHQgPSBjb25maWcubW9ub1NjcmlwdCB8fCAnbW9uby5qcyc7XG4gICAgY29uZmlnLm9mZmxpbmUgPSBjb25maWcub2ZmbGluZSB8fCBmYWxzZTtcbiAgICBjb25maWcuZGVidWdnaW5nID0gY29uZmlnLmRlYnVnZ2luZyB8fCBmYWxzZTtcblxuICAgIGlmIChjb25maWcuZW50cnlQb2ludCA9PT0gJycpIHRocm93IG5ldyBFcnJvcignTWlzc2luZyBlbnRyeXBvaW50IGluIERvbGl0dGxlIFdlYkFzc2VtYmx5IHBsdWdpbiBjb25maWd1cmF0aW9uJyk7XG4gICAgaWYgKGNvbmZpZy5hc3NlbWJsaWVzLmxlbmd0aCA9PT0gMCkgdGhyb3cgbmV3IEVycm9yKCdNaXNzaW5nIGFzc2VtYmxpZXMgaW4gRG9saXR0bGUgV2ViQXNzZW1ibHkgcGx1Z2luIGNvbmZpZ3VyYXRpb24nKTtcblxuICAgIGNvbnNvbGUubG9nKGBVc2luZyAnJHtjb25maWcuZW50cnlQb2ludH0nIGFzIGVudHJ5cG9pbnQgZm9yIFdlYkFzc2VtYmx5IGApO1xuXG4gICAgc3RvcmFnZS5wcmVsb2FkKClcbiAgICAgICAgLnRoZW4oXyA9PiBFdmVudFByb2Nlc3Nvck9mZnNldFJlcG9zaXRvcnkucHJlbG9hZCgpKVxuICAgICAgICAudGhlbihfID0+IEV2ZW50U3RvcmUucHJlbG9hZCgpKS5jYXRjaCgoZXJyb3I6IGFueSkgPT4ge1xuICAgICAgICAgICAgY29uc29sZS5lcnJvcignRXJyb3IgcHJlbG9hZGluZycsIGVycm9yKTtcbiAgICAgICAgfSkudGhlbihfID0+IHtcbiAgICAgICAgICAgIC8vIE5vdyB0aGUgcmVzdCBvZiB0aGUgc3lzdGVtIHNob3VsZCBiZSByZWFkeSBmb3IgYm9vdGluZ1xuICAgICAgICAgICAgKHdpbmRvdyBhcyBhbnkpLk1vZHVsZSA9IHt9O1xuXG4gICAgICAgICAgICAod2luZG93IGFzIGFueSkuTW9kdWxlLm9uUnVudGltZUluaXRpYWxpemVkID0gKCkgPT4ge1xuICAgICAgICAgICAgICAgIE1PTk8ubW9ub19sb2FkX3J1bnRpbWVfYW5kX2JjbChcbiAgICAgICAgICAgICAgICAgICAgJ21hbmFnZWQnLFxuICAgICAgICAgICAgICAgICAgICAnbWFuYWdlZCcsXG4gICAgICAgICAgICAgICAgICAgIGNvbmZpZy5kZWJ1Z2dpbmcgPyAxIDogMCxcbiAgICAgICAgICAgICAgICAgICAgY29uZmlnLmFzc2VtYmxpZXMsXG4gICAgICAgICAgICAgICAgICAgICgpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgIE1vZHVsZS5tb25vX2JpbmRpbmdzX2luaXQoJ1tXZWJBc3NlbWJseS5CaW5kaW5nc11XZWJBc3NlbWJseS5SdW50aW1lJyk7XG4gICAgICAgICAgICAgICAgICAgICAgICBCSU5ESU5HLmNhbGxfc3RhdGljX21ldGhvZChjb25maWcuZW50cnlQb2ludCwgW10pO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgIH07XG5cbiAgICAgICAgICAgIGNvbnN0IG1vbm9TY3JpcHQgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdzY3JpcHQnKTtcbiAgICAgICAgICAgIG1vbm9TY3JpcHQuYXN5bmMgPSB0cnVlO1xuICAgICAgICAgICAgbW9ub1NjcmlwdC5zcmMgPSBjb25maWcubW9ub1NjcmlwdDtcbiAgICAgICAgICAgIGRvY3VtZW50LmJvZHkuYXBwZW5kQ2hpbGQobW9ub1NjcmlwdCk7XG5cbiAgICAgICAgICAgIGlmIChjb25maWcub2ZmbGluZSA9PT0gdHJ1ZSkge1xuICAgICAgICAgICAgICAgIG5hdmlnYXRvci5zZXJ2aWNlV29ya2VyLnJlZ2lzdGVyKCdzZXJ2aWNlLXdvcmtlci5qcycpO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAod2luZG93IGFzIGFueSkuX21vbmdvREIgPSBuZXcgTW9uZ29EQigpO1xuICAgICAgICAgICAgKHdpbmRvdyBhcyBhbnkpLl9ldmVudFN0b3JlID0ge1xuICAgICAgICAgICAgICAgIGV2ZW50UHJvY2Vzc29yT2Zmc2V0UmVwb3NpdG9yeTogbmV3IEV2ZW50UHJvY2Vzc29yT2Zmc2V0UmVwb3NpdG9yeSgpLFxuICAgICAgICAgICAgICAgIGV2ZW50U3RvcmU6IG5ldyBFdmVudFN0b3JlKClcbiAgICAgICAgICAgIH07XG4gICAgICAgIH0pO1xufVxuIl19
