using System;
using System.Threading.Tasks;
using Akka.Actor;
using static event_source_examples.HelloWorldActor;

namespace event_source_examples
{
    class HelloWorldActor : UntypedActor
    {
        public class Hello
        {
            public string Name { get; }
            public Hello(string name) => Name = name;
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Hello hello:
                    Sender.Tell($"Hey, {hello.Name}!");
                    break;
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var system = ActorSystem.Create("hello-world-system");
            var hwActor = system.ActorOf<HelloWorldActor>();
            var result = await hwActor.Ask(new Hello("World"));
            Console.WriteLine(result); // Outputs "Hey, World!"
            Console.ReadLine();
        }
    }
}
