using System;

namespace simulace
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Simulation simulation = new Simulation();
            simulation.Start();
        }

        class Simulation
        {
            private PriorityQueue<Event> EventQueue { get; set; }
            private Queue<Car> Arsenal { get; set; }
            private int Time { get; set; }
            public void Start()
            {
                EventQueue = new PriorityQueue<Event>();
                Arsenal = new Queue<Car>();
                Time = 0;

                GetInput();
                MainLoop();
            }

            private void MainLoop()
            {
                while (true)
                {
                    ProcessEvent(EventQueue.Dequeue());
                }
            }

            private void ProcessEvent(Event e)
            {

            }

            private void GetInput()
            {
                Console.WriteLine("Počet ruzných aut a hmotnost pisku:");
                int[] input = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);

                Console.WriteLine($"Na dalších {input[0]} řádků napiš jaké auta chceš ve formátu: count loadingTime unloadingTime travelTime capacity");

                for (int i = 0; i < input[0]; i++)
                {
                    int[] inpute = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);

                    for (int j = 0; j < inpute[0]; j++)
                    {
                        Car newCar = new Car(int.Parse(i.ToString() + j.ToString()), inpute[1], inpute[2], inpute[3], inpute[4]);
                        Arsenal.Enqueue(newCar);
                    }
                }

            }
        }

        enum Status { START, FINISH}
        enum EventType { LOAD, UNLOAD, TRAVEL, TRAVELBACK}

        class Car
        {
            public int ID { get; private set; }
            public int LoadingTime { get; private set; }
            public int UnloadingTime { get; private set; }
            public int TravelTime { get; private set; }
            public int Capacity { get; private set; }

            public Car(int iD, int lT, int uT, int tT, int c)
            {
                ID = iD; LoadingTime = lT; UnloadingTime = uT;
                TravelTime = tT; Capacity = c;
            }

        }

        class Event : IComparable<Event>
        {
            public int Time { get; private set; }
            public Car car { get; private set; }
            public Status status { get; private set; }
            public EventType eventType { get; private set; }

            public Event(Car car, Status status, EventType eventType)
            {
                this.car = car; this.status = status; this.eventType = eventType;
            }

            public int CompareTo(Event secondEvent)
            {
                return Math.Sign(this.Time - secondEvent.Time);
            }
        }
    }
}
