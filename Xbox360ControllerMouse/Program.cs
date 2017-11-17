namespace Xbox360ControllerMouse
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var controllerInput = new ControllerInput();
            ConsoleHelper.ShowConsole(false);
            Console.ReadLine();
        }
    }
}