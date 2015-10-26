Devcom
======

![Build Status](https://ci.appveyor.com/api/projects/status/okhf8k49cjdql4tf?svg=true)

Devcom (short for Developer Commands) is a simple, extensible system for adding command-line interfaces to applications, games, and other interactive media running on the CLR.

A simple Devcom command-line can be created with just a few lines of code:

```cs
static void Main(string[] args)
{
    Console.Title = "Devcom";
    Devcom.Load();
    while(true)
    {
        Console.Write(Context.Default.Prompt);
        Devcom.SendCommand(Console.ReadLine());
    }
}
```

###Commands

Defining new commands is a straightforward, easy process. Make a class marked with the `[Category]` attribute. Then add static methods inside it with `[Command]` attributes that define what the commands are called.

```cs
using System;
using DeveloperCommands;

namespace Example
{
    [Category]
    public static class Commands
    {
        [Command("add", "Adds two numbers. Because why not.")]
        public static void Add(Context context, float a, float b)
        {
            Devcom.Print(a + b);
        }
    }
}
```

When this program is run, Devcom will scan your assembly (as well as any libraries you use) for Devcom commands and register them.

Then, you can use your commands:
```
devcom > add 12 18
30
```

###Convars

Convars are just as easy to create in code. To make one, use the `[Convar]` attribute on any static property or field. Here's an example:

```cs
using System;
using DeveloperCommands;

namespace Example
{
    [Category("example")]
    public static class MyConvars
    {
        [Convar("my_int")]
        public static int MyInt
        {
            get;
            set;
        }
    }
}
```

Any convar's value can be inserted into a command by surrounding the name of the convar in colons. Here is an example showing the `set` command to set the convar, and the `echo` command to display it:
```
devcom > echo "MyInt = :example.my_int:"
MyInt = 0
devcom > set example.my_int 123
devcom > echo "MyInt = :example.my_int:"
MyInt = 123
```
