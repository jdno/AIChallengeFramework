# AIChallengeFramework

AIChallengeFramework can be used to implement C# bots for Warlight's AI
challenge. It handles the communication with the game engine and provides
useful information about the state of the game.

Find out more about the challenge here:
http://theaigames.com/competitions/warlight-ai-challenge

## Disclaimer

This is a custom framework for the challenge, and in no way officially or in
any other way related to either Warlight or Conquest.

## Usage

1. Create a console application and add a reference to this library
2. Create your bot's class file, and add the namespace AIChallengeFramework
3. Have your bot implement the interface IBot
4. Start your application with something like this:

````C#
public static void Main(String[] args)
    {
        Parser parser = new Parser(new YourBot());
        parser.Run();
    }
````

## Documentation

Visit the project's Wiki on GitHub:

https://github.com/jdno/AIChallengeFramework/wiki

## Issue reporting

If you are experiencing any problems, please open an issue on GitHub:
https://github.com/jdno/AIChallengeFramework/issues

## License

The project is licensed under the Apache License, Version 2.0

## Copyright

2014 jdno (https://github.com/jdno/)

## Enjoy

Have fun & good luck!