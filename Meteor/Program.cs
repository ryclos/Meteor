

using Meteor.IO;
using Meteor.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteor
{
    internal class Program
    {
        internal static Server _server = null;
        internal static Boolean IsRunning = true;
        internal static bool debug = true;//Permet de gérer l'affichage de log en mode debug
        static void Main(string[] args)
        {
            _server = new Server();
            _server.Initialize();
            _server.Run();
        }
    }
}
