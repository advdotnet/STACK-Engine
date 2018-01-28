using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace STACK.Input
{
    /// <summary>
    /// Reconstructs input events using a given recorded demo file.
    /// </summary>
    public class PlaybackInputProvider : InputProvider
    {
        readonly Stream Reader;
        readonly DeflateStream ZipStream;
        readonly BinaryFormatter Formatter;
        long TimeStamp;
        InputQueue Queue = new InputQueue();

        bool end = false;
        InputEvent CurrentEvent;

        public PlaybackInputProvider(string filename)
        {
            Reader = File.Open(filename, FileMode.Open, FileAccess.Read);
            ZipStream = new DeflateStream(Reader, CompressionMode.Decompress, true);
            Formatter = new BinaryFormatter();
            CurrentEvent = LoadInputEventFromStream();
        }

        public override KeyboardState KeyboardState
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MouseState MouseState
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public InputEvent LoadInputEventFromStream()
        {
            InputEvent Result = new InputEvent();

            try
            {
                Result = (InputEvent)Formatter.Deserialize(ZipStream);
                ZipStream.Flush();
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                end = true;
                ZipStream.Close();
                ZipStream.Dispose();
                Reader.Dispose();
            }

            return Result;
        }


        void Update()
        {
            TimeStamp += (long)GameSpeed.TickDuration;

            // make sure to enqueue all events for the current timestamp
            while (CurrentEvent.Timestamp <= TimeStamp && !end)
            {
                Queue.Enqueue(CurrentEvent);
                CurrentEvent = LoadInputEventFromStream();
            }
        }

        public override void Dispatch(bool paused)
        {
            Update();
            while (Queue.Count > 0)
            {
                var Event = Queue.Dequeue();
                Event.Paused = paused;
                Notify(Event);
            }
        }
    }
}
