using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Chrem {

    class Program {
        public static void Main(string[] args) {
            if (args.Length == 0) {
                if (File.Exists(Operations.GetChremFilePath()) == false) {
                    Storage.CreateSource();
                    Console.WriteLine("Chreminders file created!");
                    Console.WriteLine("Location: " + Operations.GetChremFilePath());
                    Console.WriteLine("Use the \"--help\" flag for additional information.");
                } else {
                    DisplayHelp();
                }
            }

            /* Console.WriteLine("Number of arguments: " + args.Length); */
            int arg_num = 0; // args start with 0 in csharp -_-
            while (arg_num < args.Length) {
                switch (args[arg_num]) {
                    case "-h":
                    case "--help":
                    case "/?":
                        DisplayHelp();
                        arg_num++;
                        continue;

                    case "-v":
                    case "--version":
                        DisplayVersion();
                        arg_num++;
                        continue;

                    case "-a":
                    case "--add":
                        if (arg_num+1 >= args.Length || args[arg_num+1] == "") {
                            Console.WriteLine("Empty string.");
                            arg_num = arg_num+2;
                            continue;
                        }
                        string reminder = args[arg_num+1];
                        Storage.AddReminder(reminder);
                        arg_num = arg_num+2; //so it skips the "unknown argument"
                        continue;

                    case "-R":
                        if (File.Exists(Operations.GetChremFilePath()) == true) {
                            Storage.RemoveSource();
                        }
                        arg_num++;
                        continue;

                    default:
                        Console.WriteLine("Unknown argument: " + "\"" + args[arg_num] + "\"");
                        arg_num++;
                        continue;
                }
            }
        }

        private static void DisplayHelp() {
            string help_msg =
@"Usage:
  chrem                    Creates $HOME/.config/chrem-reminders file
  chrem -a                 Add an entry inside the reminders file
  chrem -r                 Remove an entry [or multiple entries, seperated with "",""]
  chrem -rn                Renumber entries inside the reminders file
  chrem -R                 Remove $HOME/.config/chrem-reminders file
  chrem --show             Prints your reminders to the terminal
  chrem --add-to-sh        Adds reminder autodetection inside bash|zsh|fish
  chrem --version          Prints what version of chrem you have installed";
            Console.WriteLine(help_msg);
        }

        private static void DisplayVersion() {
            string version = "1.0.0";
            Console.WriteLine(version);
        }
    }

    class Reminder {
        public int ID;
        public string Text;

        // Parameterized Constructor
        // User defined
        public Reminder(int id, string text)
        {
            ID = id;
            Text = text;
        }

        public int GetID()
        {
            // Some way to get ID from XML
            return 1;
        }
    }


    class Storage {

        public static void AddReminder(string reminder) {
            if (File.Exists(Operations.GetChremFilePath()) == false) {
                Console.WriteLine("Chreminders file does not exist.");
                Console.Write("Do you want to create it?" + Environment.NewLine + "[Y/n]: ");
                string create_choice = Console.ReadLine();
                if (create_choice.ToLower() == "n") {
                    return;
                } else {
                    CreateSource();
                }
            }
            Console.WriteLine("Reminder is: " + reminder);

            /* using (XmlWriter NewReminder = XmlWriter.Load(Operations.GetChremFilePath())) { */
            /* NewReminder.WriteStartDocument(); */
            /* NewReminder.WriteStartElement("Reminder"); */
            /* NewReminder.WriteElementString("text", reminder); */
            /* NewReminder.WriteEndElement(); */

            /* NewReminder.Flush(); */
            /* NewReminder.Close(); */
            /* } */
            /* using (XmlWriter writer = XmlWriter.Load(Operations.GetChremFilePath())); */
        }

        public static void CreateSource() {
            /* var settings = new XmlWriterSettings() { */
            /*     Indent = true, */
            /* }; */
            XmlWriter.Create(Operations.GetChremFilePath());
            Directory.CreateDirectory(Operations.GetChremDirPath());
        }

        public static void RemoveSource() {
            File.Delete(Operations.GetChremFilePath());
        }


    }

    class Operations {
        public static string CheckPlatform() {
            string windir = Environment.GetEnvironmentVariable("windir");
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir)) {
                return "Windows";
            } else if (File.Exists(@"/proc/sys/kernel/ostype")) {
                string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase)) {
                    // Note: Android gets here too
                    return "Linux";
                } else {
                    throw new Exception(osType);
                }
            }
            else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist")) {
                // Note: iOS gets here too
                return "Mac";
            } else {
                throw new Exception("Unsupported system!");
            }
        }

        public static string GetChremDirPath() {
            string dir_path;
            switch (Operations.CheckPlatform()) {
                case "Linux":
                    dir_path = Path.Combine("/home", Environment.UserName, ".config", "chrem");
                    break;

                case "Windows":
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    dir_path = Path.Combine(appDataPath, "chrem");
                    break;

                case "Mac":
                    dir_path = Path.Combine("/Users", Environment.UserName, "Library", "Application Support", "chrem");
                    break;

                default:
                    throw new Exception("Could not recognize system.");
            }
            return dir_path;
        }

        public static string GetChremFilePath() {
            string dir_path = GetChremDirPath();
            string chreminders_file;
            switch (Operations.CheckPlatform()) {
                case "Linux":
                    chreminders_file = Path.Combine(dir_path, "chreminders.xml");
                    break;

                case "Windows":
                    chreminders_file = Path.Combine(dir_path, "chreminders.xml");
                    break;

                case "Mac":
                    chreminders_file = Path.Combine(dir_path, "chreminders.xml");
                    break;

                default:
                    throw new Exception("Could not recognize system.");
            }
            return chreminders_file;
        }
    }
}
