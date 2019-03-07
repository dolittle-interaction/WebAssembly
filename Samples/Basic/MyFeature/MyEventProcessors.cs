using System;
using Dolittle.Events.Processing;
using Dolittle.Logging;
using Dolittle.ReadModels;
using Dolittle.Runtime.Events;
using Dolittle.Serialization.Json;
using WebAssembly;

namespace Basic.MyFeature
{
    public class MyEventProcessors : ICanProcessEvents
    {
        readonly ILogger _logger;
        readonly IAsyncReadModelRepositoryFor<Animal> _repository;

        readonly ISerializer _serializer;

        public MyEventProcessors(ILogger logger, IAsyncReadModelRepositoryFor<Animal> repository, ISerializer serializer)
        {
            _logger = logger;
            _repository = repository;
            _serializer = serializer;
        }

        [EventProcessor("0b519add-ae77-40a9-bc9b-30f014e8a186")]
        public void Process(MyEvent @event)
        {
            _logger.Information("Event processed");

            try
            { 
                /*
                var existing = _repository.GetById(Guid.Parse("358b9a07-51fa-4da1-af6d-ffff04a08a00"));
                var existingAsJson = _serializer.ToJson(existing);
                _logger.Information($"Existing document : {existingAsJson}");
                */

                var document = new Animal
                {
                    Id = Guid.NewGuid(),
                    Species = "Dog",
                    Name = Guid.NewGuid().ToString()
                };

                _repository.Insert(document).ContinueWith(t =>
                {
                    _logger.Information("Inserted document");

                    _repository.GetById(document.Id).ContinueWith(task =>
                    {
                        var existing = task.Result;
                        var existingAsJson = _serializer.ToJson(existing);
                        _logger.Information($"Existing document : {existingAsJson}");

                    });
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Couldn't insert document ");

            }
        }

        [EventProcessor("473c597b-7d3c-4e93-829b-46ef485dd980")]
        public void Process(AnimalDeleted @event, EventSourceId eventSourceId)
        {
            Console.WriteLine("Delete it");
            _repository.GetById(eventSourceId).ContinueWith(t =>
            {
                Console.WriteLine("Animal fetched");
                var animal = t.Result;
                _repository.Delete(animal).ContinueWith(_ =>
                {
                    Console.WriteLine("Deleted");
                    _logger.Information("Deleted");
                });

            });
        }
    }
}