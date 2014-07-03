using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * File : Log
 * Author : Filipe
 * Date : 05/02/2014 20:26:32
 * Description :
 *
 */

namespace Meteor.IO
{
    public enum LogType { Info, Debug, Error, Done, Warning, Thread, Loading }
    public class Log
    {
        #region FIELDS

        private static StreamWriter[] fileWriter { get; set; }

        private static Object Lock = new Object();

        #endregion

        #region CONSTRUCTORS

        static Log()
        {
            if (Directory.Exists("logs") == false)
            {
                Directory.CreateDirectory("logs");
            }
            fileWriter = new StreamWriter[3];
            fileWriter[(Int32)LogType.Info] = new StreamWriter("logs\\general.log", true);
            fileWriter[(Int32)LogType.Error] = new StreamWriter("logs\\error.log", true);
            fileWriter[(Int32)LogType.Debug] = new StreamWriter("logs\\debug.log", true);
            foreach (StreamWriter _writer in fileWriter)
            {
                _writer.AutoFlush = true;
                _writer.WriteLine(Environment.NewLine + 
                    "============== NEW SESSION STARTED : {0} ==============" + 
                    Environment.NewLine, DateTime.Now);
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Write a log message on the console
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        public static void Write(LogType type, Object obj)
        {
            Write(type, "{0}", obj);
        }

        /// <summary>
        /// Write a log message on the console
        /// </summary>
        /// <param name="type">Log type</param>
        /// <param name="format">Message</param>
        /// <param name="args"></param>
        public static void Write(LogType type, String format, params Object[] args)
        {
            try
            {
                String _text = String.Format(format, args);
                Console.Write("\r");
                switch (type)
                {
                    case LogType.Info:
                        SetColor(ConsoleColor.Green);
                        Console.Write("[Info]: ");
                        ResetColor();
                        break;
                    case LogType.Debug:
                        SetColor(ConsoleColor.Blue);
                        Console.Write("[Debug]: ");
                        ResetColor();
                        break;
                    case LogType.Error:
                        SetColor(ConsoleColor.Red);
                        Console.Write("[Error]: ");
                        ResetColor();
                        break;
                    case LogType.Done:
                        SetColor(ConsoleColor.DarkCyan);
                        Console.Write("[Done]: ");
                        ResetColor();
                        break;
                    case LogType.Warning:
                        SetColor(ConsoleColor.Yellow);
                        Console.Write("[Warning]: ");
                        ResetColor();
                        break;
                    case LogType.Thread:
                        SetColor(ConsoleColor.Gray);
                        Console.Write("[Thread]: ");
                        ResetColor();
                        break;
                    case LogType.Loading:
                        SetColor(ConsoleColor.Magenta);
                        Console.Write("[Loading]: ");
                        ResetColor();
                        break;
                }
                if (type != LogType.Loading)
                {
                    Console.WriteLine(_text);
                    WriteStream(type, _text);
                }
                else
                {
                    Console.Write(_text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error]: Log.Write() ", e.Message);
            }
        }

        /// <summary>
        /// Write a message in file
        /// </summary>
        /// <param name="type">Log type</param>
        /// <param name="text">Message</param>
        private static void WriteStream(LogType type, String text)
        {
            if (type == LogType.Error || type == LogType.Debug)
            {
                fileWriter[(Int32)type].WriteLine("[" + DateTime.Now + "]: " + text);
            }
            else
            {
                fileWriter[(Int32)LogType.Info].WriteLine("[" + type + "][" + DateTime.Now + "]: " + text);
            }
        }

        /// <summary>
        /// Set the console font color
        /// </summary>
        /// <param name="color">Color</param>
        private static void SetColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// Reset the console font color (white)
        /// </summary>
        private static void ResetColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        #endregion
    }
}
