using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using static event_source_examples.Program.CountingActor;

namespace event_source_examples
{
    public class Program
    {
        public Program()
        {
        }

        public class CountingActor : ReceiveActor
        {
            int i = 0;
            public class Count
            {
                public int Id { get; }
                public Count(int id) => Id = id;
            }
            public CountingActor()
            {
                Receive<Count>(msg =>
                {
                    Console.WriteLine($"msg: {msg.Id} -> {(i++)}");
                });
            }
        }

        static void Main(string[] args)
        {
            var system = ActorSystem.Create("hello-world-system");
            var ctActor = system.ActorOf<CountingActor>();

            Parallel.For(0, 40, async i => await ctActor.Ask(new Count(i)));

            Console.ReadLine();
        }
    }
}
