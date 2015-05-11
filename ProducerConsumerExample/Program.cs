using System;
using System.Threading;

namespace ProducerConsumerExample
{
    interface IProducerConsumer<T>
    {
        void Add(T item);
        T Get();
    }

    /// <summary>
    /// Incorrect implementation of Producer-Consumer pattern using <see cref="ManualResetEvent"/>.
    /// Causes deadlock when one thread is sleeping inside lock.
    /// </summary>
    class EventBased<T> : IProducerConsumer<T>
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(false);
        private readonly T[] _buffer = new T[10];
        private readonly object _locker = new object();
        private int _index;

        public virtual void Add(T item)
        {
            lock (_locker)
            {
                while (_index > 9)
                    _event.WaitOne();

                _buffer[_index++] = item;
                _event.Set();
            }
        }

        public virtual T Get()
        {
            lock (_locker)
            {
                while (_index == 0)
                    _event.WaitOne();

                var c = _buffer[--_index];
                _event.Set();
                return c;
            }
        }
    }

    /// <summary>
    /// Correct implementation of Producer-Consumer pattern using <see cref="Monitor"/>.
    /// Doesn't cause deadlock because <see cref="Monitor.Wait(object)"/> releases lock.
    /// </summary>
    class MonitorBased<T> : IProducerConsumer<T>
    {
        private readonly T[] _buffer = new T[10];
        private readonly object _locker = new object();
        private int _index;

        public void Add(T item)
        {
            lock (_locker)
            {
                while (_index > 9)
                    Monitor.Wait(_locker);

                _buffer[_index++] = item;
                Monitor.Pulse(_locker);
            }
        }

        public T Get()
        {
            lock (_locker)
            {
                while (_index == 0)
                    Monitor.Wait(_locker);

                var c = _buffer[--_index];
                Monitor.Pulse(_locker);
                return c;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
//            IProducerConsumer<int> data = new MonitorBased<int>();
            IProducerConsumer<int> data = new EventBased<int>();

            var t1 = new Thread(() => Thread1(data)) { Name = "Producer" };
            t1.Start();

            var t2 = new Thread(() => Thread2(data)) { Name = "Consumer" };
            t2.Start();

            t2.Join();
        }

        static void Thread1(IProducerConsumer<int> data)
        {
            Console.WriteLine("Producer: start ({0}).", Thread.CurrentThread.Name);

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(50);
                Console.WriteLine("Producer: " + i);
                data.Add(i);
            }
        }

        static void Thread2(IProducerConsumer<int> data)
        {
            Console.WriteLine("Consumer: start ({0}).", Thread.CurrentThread.Name);

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(90);
                Console.WriteLine("Consumer: " + data.Get());
            }
        }
    }
}
