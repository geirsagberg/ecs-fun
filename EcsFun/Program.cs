using System;

namespace EcsFun
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new EcsGame())
                game.Run();
        }
    }
}
