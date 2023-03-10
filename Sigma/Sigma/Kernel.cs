using System;
using FS = Sigma.FileSystem;
using Sys = Cosmos.System;

namespace Sigma
{
    public class Kernel : Sys.Kernel
    {
        //31

        protected override void BeforeRun()
        {
            //Search for ATA disks
            if (!FS.DiskManager.InitalizeDisks())
            {
                Console.WriteLine("There is no harddrive device was found, system will halt.");
                Console.ReadLine();
                throw new InvalidOperationException();
            }
            FS.Controller.InitializeFilesystem();
            Console.WriteLine("Welcome to Codename Sigma!");
        }

        protected override void Run()
        {
            string crntdir = FS.Controller.GetCurrentDirectory();
            string command = Console.ReadLine();
            switch (command.Split(' ')[0])
            {
                case "echo":
                    if (command.Split(' ').Length > 1)
                        Console.WriteLine(command.Remove(0, 5));
                    break;
                case "cd":
                    string[] splitted = Helper.SpecialSplit(command);
                    if (splitted.Length > 1)
                    {
                        foreach (string s in splitted[1].Split('\\'))
                        {
                            if (s == "..")
                                FS.Controller.GoBack();
                            else
                            {
                                if (System.IO.Directory.Exists(crntdir + "\\" + s))
                                {
                                    FS.Controller.SetCurrentDirectory(crntdir + "\\" + s);
                                    crntdir += "\\" + s;
                                }
                                else
                                    Console.WriteLine("The directory you are trying to navigate doesn't exist.");
                            }
                        }
                    }
                    break;
                case "dir":
                    FS.FileManager.ListContents(crntdir);
                    break;
                case "read":
                    splitted = Helper.SpecialSplit(command);
                    string file = Helper.FindPath(splitted[1], crntdir);
                    if (System.IO.File.Exists(file))
                        Console.WriteLine(System.IO.File.ReadAllText(file));
                    break;
                case "new":
                    splitted = Helper.SpecialSplit(command);
                    file = Helper.FindPath(splitted[1], crntdir);
                    if (file.IndexOfAny(FS.Controller.invalidCharacters) >= 0)
                    {
                        Console.WriteLine("File cannot contain invalid characters (* \\ / \" < > : | ?)");
                        break;
                    }
                    FS.Controller.fs.CreateFile(file);
                    break;
                case "newd":
                    splitted = Helper.SpecialSplit(command);
                    file = Helper.FindPath(splitted[1], crntdir);
                    if (file.IndexOfAny(FS.Controller.invalidCharacters) >= 0)
                    {
                        Console.WriteLine("Directory cannot contain invalid characters (* \\ / \" < > : | ?)");
                        break;
                    }
                    FS.Controller.fs.CreateDirectory(file);
                    break;
                case "copy":
                    splitted = Helper.SpecialSplit(command);
                    string src = Helper.FindPath(splitted[1], crntdir);
                    string dst = Helper.FindPath(splitted[2], crntdir);
                    if (System.IO.File.Exists(src))
                        System.IO.File.Copy(src, dst);
                    else
                        Console.WriteLine("The file you are trying to copy doesn't exist.");

                    break;
                case "move":
                    splitted = Helper.SpecialSplit(command);
                    src = Helper.FindPath(splitted[1], crntdir);
                    dst = Helper.FindPath(splitted[2], crntdir);
                    if (System.IO.File.Exists(src))
                    {
                        System.IO.File.Copy(src, dst);
                        FS.Controller.fs.DeleteFile(FS.Controller.fs.GetFile(src));
                    }
                    break;
                case "remove":
                    splitted = Helper.SpecialSplit(command);
                    file = Helper.FindPath(splitted[1], crntdir);
                    if (System.IO.File.Exists(file))
                        FS.Controller.fs.DeleteFile(FS.Controller.fs.GetFile(file));
                    else
                        Console.WriteLine("The file you are trying to remove doesn't exist.");
                    break;
                case "removed":
                    splitted = Helper.SpecialSplit(command);
                    file = Helper.FindPath(splitted[1], crntdir);
                    if (System.IO.Directory.Exists(file))
                        FS.Controller.fs.DeleteDirectory(FS.Controller.fs.GetDirectory(file));
                    else
                        Console.WriteLine("The directory you try to remove doesn't exist.");
                    break;
                case "gcd":
                    Console.WriteLine(crntdir + "\\");
                    break;
                case "atainf":
                    FS.DiskManager.GetDetailedInformation();
                    break;
                case "kbd":
                    Usage.KeyboardSettings.ChangeKeyboard(command.Split(' ')[1]);
                    break;
                case "info":
                    Console.WriteLine("Codename Sigma 0.1.0");
                    break;
                case "shutdown":
                    splitted = command.Split(' ');
                    switch (splitted[1])
                    {
                        case "-s":
                            Cosmos.Core.ACPI.Shutdown();
                            break;
                        case "-r":
                            Cosmos.Core.CPU.Reboot();
                            break;
                        default:
                            Console.WriteLine("The action for this command isn't valid.\n (-s for a complete shutdown, -r for restart)");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("The command doesn' exist.");
                    break;
            }
        }
    }
}
