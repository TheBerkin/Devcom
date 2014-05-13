Devcom
======

Devcom (short for Developer Commands) is a simple, extensible system for adding command-line interfaces to applications, games, and other interactive media running on the CLR.

Defining new commands is a straightforward, easy process. Make a class marked with the `[DevcomCategory]` attribute. Then add static methods inside it with `[Command]` attributes that define what the commands are called.

```cs
using System;
using DeveloperCommands;

namespace Example
{
    [DevcomCategory]
    public static class Commands
    {
        [Command("add", "Adds two numbers. Because why not.")]
        public static void Add(DevcomContext context, float a, float b)
        {
            Devcom.Print(a + b);
        }
    }
}
```

A simple Devcom command-line can be created with just a few lines of code:

```cs
static void Main(string[] args)
{
    Console.Title = "Devcom";
    Devcom.Load();
    while(true)
    {
        Console.Write(Devcom.Prompt);
        Devcom.SendCommand(null, Console.ReadLine());
    }
}
```

When this program is run, Devcom will scan your assembly (as well as any libraries you use) for Devcom commands and register them.

Then, you can use your commands:
```
devcom > add 12 18
30
```
