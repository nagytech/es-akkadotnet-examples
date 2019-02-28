using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence;
using static event_source_examples.Program.PersistentBasketActor;

namespace event_source_examples
{
    public class Program
    {
        public class PersistentBasketActor : ReceivePersistentActor
        {
            int i = 0;

            public string BasketId { get; }
            public string CustomerId { get; }

            public Dictionary<string, decimal> Cart { get; }
                = new Dictionary<string, decimal>();

            public override string PersistenceId => $"{CustomerId}::{BasketId}";

            public PersistentBasketActor(string customerId, string basketId)
            {
                CustomerId = customerId;
                BasketId = basketId;

                Command<GetCartCommand>(_ => Sender.Tell(Cart));
                Command<AlterItemQuantityCommand>(msg =>
                {
                    if (msg.Delta == 0) return;

                    var @event = DeriveEvent(msg.ItemId, msg.Delta);
                    UpdateState(@event);
                    Persist(
                        @event,
                        (e) => Console.WriteLine($"Altered number of [{msg.ItemId}] in cart by [{msg.Delta}].")
                    );
                    Sender.Tell(Done.Instance);
                });
                Recover<object>(e => UpdateState(e));
            }


            private void UpdateState(object @event)
            {
                switch (@event)
                {
                    case ItemAddedEvent addedEvent:
                        Cart.Add(addedEvent.ItemId, addedEvent.Quantity);
                        break;
                    case ItemQuantityIncreasedEvent increasedEvent:
                        Cart[increasedEvent.ItemId] += increasedEvent.IncreaseBy;
                        break;
                    case ItemQuantityDecreasedEvent decreasedEvent:
                        Cart[decreasedEvent.ItemId] += decreasedEvent.DecreaseBy;
                        break;
                    case ItemRemovedEvent removedEvent:
                        Cart.Remove(removedEvent.ItemId);
                        break;
                    default:
                        // Unhandled event.
                        return;
                }
            }

            private ItemQuantityAlteredEvent DeriveEvent(string itemId, decimal delta)
            {
                if (delta == 0) return null;
                if (Cart.TryGetValue(itemId, out var initial))
                {
                    if (delta > 0)
                        return new ItemQuantityIncreasedEvent(itemId, delta);
                    return new ItemRemovedEvent(itemId);
                }
                else
                {
                    if (delta > 0)
                        return new ItemAddedEvent(itemId, delta);
                    throw new InvalidOperationException(
                        "Cannot decrement item which is not in the cart"
                    );
                }
            }
        }

        static async Task Main(string[] args)
        {
            var config = Akka.Configuration.ConfigurationFactory.ParseString(
                File.ReadAllText("akka.conf")
            );
            var system = ActorSystem.Create("hello-world-system", config);
            var ctActor = system.ActorOf(
                Props.Create(() => new PersistentBasketActor("C123456", "B00001"))
            );

            Random r = new Random((int)DateTime.Now.Ticks);
            var tasks = Enumerable
                .Range(0, 10)
                .Select(i => ctActor.Ask(
                    new AlterItemQuantityCommand(i.ToString(), r.Next(1, 5))
                ))
                .ToArray();

            await ctActor.Ask(
                new AlterItemQuantityCommand(r.Next(0, 10).ToString(), -100)
            );

            await Task.WhenAll(tasks);

            Thread.Sleep(50); // Wait for actor to finish writing.

            Console.WriteLine("Cart contents\n============");
            var res = await ctActor.Ask(new GetCartCommand());
            var cart = res as Dictionary<string, decimal>;
            foreach (var c in cart.OrderBy(x => x.Key))
                Console.WriteLine($"{c.Key} -> {c.Value}");

            Console.ReadLine();
        }
    }
}
