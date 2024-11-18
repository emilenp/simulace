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
            private PriorityQueue<Event, int> EventQueue { get; set; }
            private List<Car> Arsenal { get; set; }
            private int Sand { get; set; }
            private bool isLoading { get; set; }
            public void Start()
            {
                EventQueue = new PriorityQueue<Event, int>();
                Arsenal = new List<Car>();
                isLoading = false;

                GetInput();
                MainLoop();
            }
            private void MainLoop()
            {
                Arsenal = Arsenal.OrderByDescending(c => c.Capacity).ThenBy(c => c.TravelTime).ThenBy(c => c.LoadingTime).ThenBy(c => c.UnloadingTime).ToList();
                EventQueue.Enqueue(new Event(Arsenal[0], EventType.LOAD, Status.START, 0), 0);
                Arsenal.RemoveAt(0);

                while (Sand > 0)
                {
                    ProcessEvent(EventQueue.Dequeue());
                }

                while (EventQueue.Count > 0)
                {
                    Console.WriteLine(EventQueue.Dequeue());
                }
            }
            private void ProcessEvent(Event e)
            {
                Console.WriteLine(e);

                if (e.EventType == EventType.LOAD && e.Status == Status.START)
                {
                    Sand -= e.Car.Capacity;
                    isLoading = true;
                    EventQueue.Enqueue(new Event(e.Car, EventType.LOAD, Status.FINISH, e.Time + e.Car.LoadingTime), e.Time + e.Car.LoadingTime);
                    EventQueue.Enqueue(new Event(e.Car, EventType.TRAVEL, Status.START, e.Time + e.Car.LoadingTime), e.Time + e.Car.LoadingTime);
                    EventQueue.Enqueue(new Event(e.Car, EventType.TRAVEL, Status.FINISH, e.Time + e.Car.LoadingTime + e.Car.TravelTime), e.Time + e.Car.LoadingTime + e.Car.TravelTime);
                    EventQueue.Enqueue(new Event(e.Car, EventType.UNLOAD, Status.START, e.Time + e.Car.LoadingTime + e.Car.TravelTime), e.Time + e.Car.LoadingTime + e.Car.TravelTime);
                    EventQueue.Enqueue(new Event(e.Car, EventType.UNLOAD, Status.FINISH, e.Time + e.Car.LoadingTime + e.Car.TravelTime + e.Car.UnloadingTime), e.Time + e.Car.LoadingTime + e.Car.TravelTime + e.Car.UnloadingTime);
                    EventQueue.Enqueue(new Event(e.Car, EventType.TRAVELBACK, Status.START, e.Time + e.Car.LoadingTime + e.Car.TravelTime + e.Car.UnloadingTime), e.Time + e.Car.LoadingTime + e.Car.TravelTime + e.Car.UnloadingTime);
                    EventQueue.Enqueue(new Event(e.Car, EventType.TRAVELBACK, Status.FINISH, e.Time + e.Car.LoadingTime + e.Car.TravelTime + e.Car.UnloadingTime + e.Car.TravelTime), e.Time + e.Car.LoadingTime + e.Car.TravelTime + e.Car.UnloadingTime + e.Car.TravelTime);
                }

                if (e.EventType == EventType.LOAD && e.Status == Status.FINISH)
                {
                    isLoading = false;
                }
                else if (isLoading == false && Arsenal.Count > 0)
                {
                    isLoading = true;
                    EventQueue.Enqueue(new Event(Arsenal[0], EventType.LOAD, Status.START, e.Time), e.Time);
                    Arsenal.RemoveAt(0);
                }

                if (e.EventType == EventType.TRAVELBACK && e.Status == Status.FINISH)
                {
                    Arsenal.Add(e.Car);
                    Arsenal = Arsenal.OrderByDescending(c => c.Capacity).ThenBy(c => c.TravelTime).ThenBy(c => c.LoadingTime).ThenBy(c => c.UnloadingTime).ToList();
                    if (!isLoading)
                    {
                        EventQueue.Enqueue(new Event(Arsenal[0], EventType.LOAD, Status.START, e.Time), e.Time);
                        Arsenal.RemoveAt(0);
                    }

                }
            }
            private void GetInput()
            {
                Console.WriteLine("Počet ruzných aut a hmotnost pisku:");
                int[] input = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);
                Sand = input[1];
                Console.WriteLine($"Na dalších {input[0]} řádků napiš jaké auta chceš ve formátu: count loadingTime unloadingTime travelTime capacity");

                for (int i = 0; i < input[0]; i++)
                {
                    int[] Car = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);

                    for (int j = 0; j < Car[0]; j++)
                    {
                        Car newCar = new Car(j + 1, i + 1, Car[1], Car[2], Car[3], Car[4]);
                        Arsenal.Add(newCar);
                    }
                }
            }
        }

        enum Status { START, FINISH }
        enum EventType { LOAD, UNLOAD, TRAVEL, TRAVELBACK }

        class Car
        {
            public int ID { get; private set; }
            public int Type { get; private set; }
            public int LoadingTime { get; private set; }
            public int UnloadingTime { get; private set; }
            public int TravelTime { get; private set; }
            public int Capacity { get; private set; }

            public Car(int iD, int type, int lT, int uT, int tT, int c)
            {
                ID = iD; LoadingTime = lT; UnloadingTime = uT;
                TravelTime = tT; Capacity = c;
                Type = type;
            }
        }
        class Event
        {
            public int Time { get; private set; }
            public Car Car { get; private set; }
            public Status Status { get; private set; }
            public EventType EventType { get; private set; }

            public Event(Car car, EventType eventType, Status status, int time)
            {
                Car = car; Status = status; EventType = eventType;
                Time = time;
            }

            public override string ToString()
            {
                return $"[{Time}] – Car type: {Car.Type} Car ID: {Car.ID} {EventType.ToString()} ({Status.ToString()})";
            }
        }
    }
}
