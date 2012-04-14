using System;

namespace LodViewer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (LodViewer game = new LodViewer(null, null))
            {
                game.Run();
            }
        }
    }
#endif
}

