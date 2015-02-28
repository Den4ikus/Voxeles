using System;

namespace Program
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new MyGame())
            {
                game.Run();
            }
        }
    }
}
