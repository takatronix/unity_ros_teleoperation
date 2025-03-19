using System;

namespace CircularBuffer
{
    public class CircularBuffer<T>
    {
        private T[] buffer;
        private int head;
        private int tail;
        private int bufferSize;
        private int count;

        public CircularBuffer(int size)
        {
            buffer = new T[size];
            bufferSize = size;
            head = 0;
            tail = 0;
            count = 0;
        }

        public void Add(T item)
        {
            buffer[head] = item;
            head = (head + 1) % bufferSize;
            if (count == bufferSize)
            {
                tail = (tail + 1) % bufferSize;
            }
            else
            {
                count++;
            }
        }

        public T[] GetBuffer()
        {
            T[] result = new T[count];
            if (count == 0)
            {
                return result;
            }

            if (head > tail)
            {
                Array.Copy(buffer, tail, result, 0, count);
            }
            else
            {
                Array.Copy(buffer, tail, result, 0, bufferSize - tail);
                Array.Copy(buffer, 0, result, bufferSize - tail, head);
            }

            return result;
        }

        public void Clear()
        {
            head = 0;
            tail = 0;
            count = 0;
        }

        public int Count => count;
    }
}