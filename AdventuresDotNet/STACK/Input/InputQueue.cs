using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace STACK.Input
{

    /// <summary>
    /// Logs input events to a file for recording purposes.
    /// </summary>
    class InputEventFileLogger : IDisposable
    {
        readonly Stream FileStream;
        readonly BinaryFormatter Formatter;
        readonly DeflateStream ZipStream;
        
        public InputEventFileLogger(string filename)
        {            
            FileStream = File.Open(SaveGame.UserStorageFolder() + filename, FileMode.Create);
            ZipStream = new DeflateStream(FileStream, CompressionMode.Compress, true);
            Formatter = new BinaryFormatter();
        }        

        public void Log(InputEvent input)
        {
            Formatter.Serialize(ZipStream, input);                                    
        }

        public void Dispose()
        {
            ZipStream.Flush();
            ZipStream.Close();
            ZipStream.Dispose();
            FileStream.Dispose();
        }

        public void Flush()
        {
            Dispose();
        }
    }

    /// <summary>
    /// Queue of InputEvents.
    /// </summary>
    public class InputQueue : Queue<InputEvent>
    {
        private InputEventFileLogger Logger;

        public InputQueue(bool record = false) : base(5) 
        {
            if (record)
            {
                Logger = new InputEventFileLogger("demo.dat");
            }
        }        

        public new void Enqueue(InputEvent item)
        {            
            if (Logger != null)
            {
                Logger.Log(item);
            }

            base.Enqueue(item);
        }

        public void OnUnload()
        {
            if (Logger != null)
            {
                Logger.Flush();
            }
        }
    }
}
