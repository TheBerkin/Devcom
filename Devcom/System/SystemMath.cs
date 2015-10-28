using System;

namespace DeveloperCommands
{
    [Category("math")]
    internal static class SystemMath
    {
        [Command("add", "Adds two values and stores the sum in a convar.")]
        public static void Add(Context context, double a, double b, string cvOut)
        {
            Convar o;
            if (!context.RequestConvar(cvOut, out o)) return;
            o.Value = a + b;
        }

        [Command("sub", "Subtracts two values and stores the difference in a convar.")]
        public static void Sub(Context context, double a, double b, string cvOut)
        {
            Convar o;
            if (!context.RequestConvar(cvOut, out o)) return;
            o.Value = a - b;
        }

        [Command("mul", "Multiplies two values and stores the product in a convar.")]
        public static void Mul(Context context, double a, double b, string cvOut)
        {
            Convar o;
            if (!context.RequestConvar(cvOut, out o)) return;
            o.Value = a * b;
        }

        [Command("div", "Divides two values and stores the quotient in a convar.")]
        public static void Div(Context context, double a, double b, string cvOut)
        {
            Convar o;
            if (!context.RequestConvar(cvOut, out o)) return;
            o.Value = a / b;
        }
    }
}