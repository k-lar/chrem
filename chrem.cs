using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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

                    case "-s":
                    case "--show":
                        Operations.DrawReminders();
                        arg_num++;
                        continue;

                    case "-r":
                    case "--remove":
                        if (arg_num+1 >= args.Length || args[arg_num+1] == "") {
                            Console.WriteLine("No removal target.");
                            arg_num = arg_num+2;
                            continue;
                        }
                        if (args[arg_num+1].Contains(',') == true && args[arg_num+1].Length > 1) {
                            int[] nums;
                            try {
                                nums = Array.ConvertAll(args[arg_num+1].Split(','), int.Parse);
                            } catch (Exception error) {
                                Console.WriteLine(error.Message);
                                arg_num = arg_num+2;
                                continue;
                            }
                            Array.Sort(nums);
                            Array.Reverse(nums);
                            for (int i = 0; i < nums.Length; i++) {
                                //Console.WriteLine("Removing:" + nums[i]);
                                try {
                                    Operations.RemoveEntry(nums[i]);
                                } catch (Exception error) {
                                    Console.WriteLine(error.Message);
                                    arg_num = arg_num+2;
                                    continue;
                                }
                            }
                        } else {
                            int entry_num;
                            try {
                                entry_num = Convert.ToInt16(args[arg_num+1]);
                            } catch (Exception error) {
                                Console.WriteLine(error.Message);
                                arg_num = arg_num+2;
                                continue;
                            }
                            Operations.RemoveEntry(entry_num);
                            arg_num = arg_num+2;
                            continue;
                        }
                        arg_num = arg_num+2;
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
  chrem                    Creates $HOME/.config/chreminders.txt file
                           WINDOWS: %appdata%\local\chrem\chreminders.txt
  chrem -a                 Add an entry inside the reminders file
  chrem -r                 Remove an entry [or multiple entries, seperated with "",""]
  chrem -R                 Remove chreminders.txt file
  chrem --show             Prints your reminders to the terminal
  chrem --version          Prints what version of chrem you have installed";
            Console.WriteLine(help_msg);
        }

        private static void DisplayVersion() {
            string version = "1.1.0";
            Console.WriteLine(version);
        }
    }

    class Reminder {
        public int ID;
        public string Text;

        // Parameterized Constructor
        // User defined
        public Reminder(int id, string text) {
            ID = id;
            Text = text;
        }
    }


    class Storage {
        public static async void AddReminder(string reminder) {
            if (File.Exists(Operations.GetChremFilePath()) == false) {
                Console.WriteLine("Chreminders file does not exist.");
                Console.Write("Do you want to create it?" + Environment.NewLine + "[Y/n]: ");
                string create_choice = Console.ReadLine();
                if (create_choice.ToLower() == "n") {
                    return;
                } else {
                    CreateSource();
                    Console.WriteLine("You can now add entries!");
                }
            }
            /* Console.WriteLine("Reminder is: " + reminder); */
            string entry = "[" + Operations.GetNextNum(Operations.GetChremFilePath()) + "] - " + reminder;
            using (StreamWriter file = new StreamWriter(Operations.GetChremFilePath(), append: true)) {
                await file.WriteLineAsync(entry);
            }
        }

        public static void CreateSource() {
            Directory.CreateDirectory(Operations.GetChremDirPath());
            File.Create(Operations.GetChremFilePath());
        }

        public static void RemoveSource() {
            File.Delete(Operations.GetChremFilePath());
        }


    }

    class Operations {
        public static string GetNextNum(string file) {
            using (StreamReader reader = File.OpenText(file)) {
                var lineNumber = 0;
                string line;
                do {
                    lineNumber++;
                } while ((line = reader.ReadLine()) != null);
                return lineNumber.ToString();
            }
        }

        public static int GetLongestLine() {
            var lines = File.ReadLines(GetChremFilePath());
            var Maximum = "";
            foreach (string line in lines) {
                if (Maximum.Length < line.Length) {
                    Maximum = line;
                }
            }
            return Maximum.Length;
        }


        public static async void RemoveEntry(int entry_num) {
            var tempFile = Path.GetTempFileName();
            int counter = 0;
            foreach (string line in File.ReadLines(Operations.GetChremFilePath())) {
                counter++;
                string entry_text = line.Substring(line.IndexOf("- ") + 1);
                string new_entry = "[" + Operations.GetNextNum(tempFile) + "] -" + entry_text;
                if (counter != entry_num) {
                    using (StreamWriter file = new StreamWriter(tempFile, append: true)) {
                        await file.WriteLineAsync(new_entry);
                    }
                }
            }
            File.Delete(Operations.GetChremFilePath());
            File.Move(tempFile, Operations.GetChremFilePath());
        }

        public static void DrawReminders() {
            int bottom_frame = GetLongestLine();
            int half_frame = GetLongestLine()/2;
            int side_frame = 0;
            string side = "";
            if (bottom_frame > 13 && bottom_frame < 80) {
                while (side_frame < half_frame) {
                    side = side + "#";
                    side_frame++;
                }

                bottom_frame = side_frame*2+11;
                int bottom_frame_counter = 0;
                string bottom = "";
                while (bottom_frame_counter < bottom_frame) {
                    bottom = bottom + "#";
                    bottom_frame_counter++;
                }

                Console.WriteLine(side + " Reminders " + side);
                Operations.ShowReminders();
                Console.WriteLine(bottom);

            } else if (bottom_frame > 80) {
                Console.WriteLine("################################## Reminders ##################################");
                Operations.ShowReminders();
                Console.WriteLine("###############################################################################");
            } else {
                Console.WriteLine("# Reminders #");
                Operations.ShowReminders();
                Console.WriteLine("#############");
            }
        }

        public static void ShowReminders() {
            if (File.Exists(Operations.GetChremFilePath()) == false) {
                return;
            }

            using (var reader = new StreamReader(Operations.GetChremFilePath())) {
                string reminder;
                do
                {
                    reminder = reader.ReadLine();
                    if (reminder != null) {
                        Console.WriteLine(reminder);
                    }
                } while (reminder != null);
            }
        }

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
                    chreminders_file = Path.Combine(dir_path, "chreminders.txt");
                    break;

                case "Windows":
                    chreminders_file = Path.Combine(dir_path, "chreminders.txt");
                    break;

                case "Mac":
                    chreminders_file = Path.Combine(dir_path, "chreminders.txt");
                    break;

                default:
                    throw new Exception("Could not recognize system.");
            }
            return chreminders_file;
        }
    }
}
