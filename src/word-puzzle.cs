using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;



namespace WordPuzzle
{
    class Program
    {
        // Globals
        public static string[] board = ReadFile("puzzle.txt"); // Read puzzle
        public static string[,] h_spaces = new string[100, 4]; // Horizontal Spaces
        public static string[,] v_spaces = new string[100, 4]; // Vertical Spaces
        public static string[] words = ReadFile("dictionary.txt"); // Read Words
        public static string[] words_placed = new string[words.Length];
        public static int placed_words_count = 0;

        // Fast mode
        public static bool fast_mode = false;


        static void Main(string[] args)
        {
            ModeChoice();
            SortWords();
            WriteWords();
            PlaceWords();
            WriteSolution();
        }

        // Read the file
        public static string[] ReadFile(string relative_path)
        {
            string[] data = File.ReadAllLines(relative_path);

            return data;
        }

        // Write given multiline string
        public static void WriteMultiline(string[] lines)
        {
            WriteWordCount(false);


            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            for (int i = 0; i < lines.Length; i++)
            {

                Console.SetCursorPosition(i + 1, 1);
                if (i < 10)
                    Console.Write(i);
                else Console.Write(i % 10);

                Console.SetCursorPosition(0, i + 2);
                if (i < 10)
                    Console.WriteLine(i + lines[i]);
                else Console.WriteLine((i % 10) + lines[i]);
            }
        }

        // Fast Mode
        public static void ModeChoice()
        {
            // Fast mode 
            Console.WriteLine("\nEnable fast mode? (Skips animations and sounds) (y/n)");
            string fm = Console.ReadLine();
            if (fm.ToLower() == "y")
                fast_mode = true;
        }

        // Sort words
        public static void SortWords()
        {
            for (int i = 0; i < words.Length; i++)
            {
                string temp = " ";
                for (int j = i + 1; j < words.Length; j++)
                {
                    if (words[i].Length > words[j].Length)
                    {
                        temp = words[j];
                        words[j] = words[i];
                        words[i] = temp;
                    }
                    else if (words[i].Length == words[j].Length && words[i].CompareTo(words[j]) > 0)
                    {
                        temp = words[j];
                        words[j] = words[i];
                        words[i] = temp;
                    }
                }
            }
        }
        // End sort

        // Write words
        public static void WriteWords()
        {
            int line = 2;

            Console.SetCursorPosition(62, 1);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ~~~~ Word List ~~~~ ");
            Console.BackgroundColor = ConsoleColor.White;

            for (int x = 0; x < words.Length; x++)
            {

                Console.ForegroundColor = ConsoleColor.Black;
                string word = words[x];
                if (word == null || word.Length == 0)
                    continue;

                string status = words_placed.Contains(word) ? "+" : " ";
                Console.SetCursorPosition(65, line);
                if (words_placed.Contains(word))
                    Console.ForegroundColor = ConsoleColor.DarkGreen;

                Console.WriteLine("[ " + status + " ] " + word);


                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(62, line);
                Console.Write("█");

                Console.SetCursorPosition(82, line);
                Console.Write("█");

                Console.ForegroundColor = ConsoleColor.Black;

                line++;
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(62, line);
            Console.Write("█████████████████████");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Read spaces horizontally
        public static void AnalyzeHorizontalSpaces()
        {


            int ind = 0,
                current_col = 0,
                current_row = 0;

            // Work every line
            foreach (string line in board)
            {
                // Progress flag
                bool space_progress = false;
                string space_mask = "";

                // Every line
                foreach (char c in line)
                {
                    // Each individual character
                    if (c != '█')
                    {
                        // Start recording if not block
                        space_progress = true;
                        space_mask += c.ToString();
                    }
                    else
                    {
                        // Kill the record if we are recording
                        if (space_progress)
                        {
                            space_progress = false;

                            // Save to array if worths
                            if (space_mask.Length > 1)
                            {
                                h_spaces[ind, 0] = space_mask;
                                h_spaces[ind, 1] = current_col.ToString();
                                h_spaces[ind, 2] = current_row.ToString();
                                h_spaces[ind, 3] = "0";
                                ind++;
                            }

                            space_mask = "";
                        }
                    }

                    current_col++;
                }
                // End line chars

                current_row++;
                current_col = 0;
            }
            // End lines

        }
        // End vertical analyze

        // Read spaces vertically
        public static void AnalyzeVerticalSpaces()
        {
            int col_length = board[0].Length;
            int ind = 0,
               current_col = 0,
               current_row = 0;

            // Iterate columns
            for (int i = 0; i < col_length; i++)
            {
                bool space_progress = true;
                string space_mask = "";

                // Iterate words
                foreach (string line in board)
                {
                    char c = line[i];

                    // Each individual character
                    if (c != '█')
                    {
                        // Start recording if not block
                        space_progress = true;
                        space_mask += c.ToString();
                    }
                    else
                    {
                        // Kill the record if we are recording
                        if (space_progress)
                        {
                            space_progress = false;

                            // Save to array if worths
                            if (space_mask.Length > 1)
                            {
                                v_spaces[ind, 0] = space_mask;
                                v_spaces[ind, 1] = current_col.ToString();
                                v_spaces[ind, 2] = current_row.ToString();
                                v_spaces[ind, 3] = "0";
                                ind++;
                            }

                            space_mask = "";
                        }
                        // End space progress
                    }
                    // End block hit


                    current_row++;
                }
                // End line each

                current_row = 0;
                current_col++;
            }
            // End col each

        }
        // End vertical analyze

        // place words to the board
        public static void PlaceWords()
        {
            // Placing progress flag

            bool progress = true;


           


            int wpi = 0;

            // Work until board is done
            while (progress)
            {
                int logY = 20;
                Console.Clear();
                WriteMultiline(board);



                h_spaces = new string[100, 4];
                v_spaces = new string[100, 4];

                // Analyze spaces
                AnalyzeHorizontalSpaces();
                AnalyzeVerticalSpaces();


                bool finish = true;




                // Work horizontal spaces
                for (int i = 0; i < h_spaces.GetLength(0); i++)
                {
                    // Skip if empty space
                    if (h_spaces[i, 1] == null)
                        continue;

                    // Read data
                    string space = h_spaces[i, 0];

                    // Skip if empty
                    if (space.Trim().Length == 0)
                        continue;


                    int xx = int.Parse(h_spaces[i, 1]),
                        yy = int.Parse(h_spaces[i, 2]);

                    string placed = h_spaces[i, 3];

                    // Skip if this space already filled
                    if (placed == "1")
                        continue;


                    string word_to_place = "";
                    int placeable_words = 0;

                    // Work words
                    foreach (string word in words)
                    {
                        // Place flag
                        bool place = true;

                        // Skip if already placed
                        if (words_placed.Contains(word))
                            continue;

                        // Skip if lengths doesn't match
                        if (word.Length != space.Length)
                            continue;

                        // Check for special chars
                        for (int j = 0; j < space.Length; j++)
                        {
                            char c = space[j];

                            // Skip checking spaces
                            if (c == ' ')
                                continue;

                            // Do not place the word if existing char in the space doesn't match
                            if (c != word[j])
                                place = false;
                        }

                        // Mark as placeable
                        if (place)
                        {
                            word_to_place = word;
                            placeable_words++;
                        }


                    }
                    // End words

                    // Place the found words
                    // Start placing
                    if (placeable_words == 1)
                    {
                        int x_place = xx - word_to_place.Length;
                        int j = 0;



                        foreach (char c in word_to_place)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.SetCursorPosition(x_place + 1, yy + 2);
                            Console.Write(c);


                            Sleep();

                            Console.ResetColor();

                            Console.SetCursorPosition((x_place++) + 1, yy + 2);
                            Console.Write(c);

                            StringBuilder sb = new StringBuilder(board[yy]);
                            sb[xx - word_to_place.Length + j] = c;
                            board[yy] = sb.ToString();
                            j++;
                            WriteWords();

                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            logY = 20;
                            Console.SetCursorPosition(1, logY++);
                            Console.WriteLine("EVENT LOG");

                            Beep(1000, 200);
                            Sleep();


                        }

                        WriteWordCount();

                        h_spaces[i, 0] = word_to_place;
                        h_spaces[i, 3] = "1";

                        // Mark as placed
                        words_placed[wpi++] = word_to_place;

                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                        Console.SetCursorPosition(1, logY++);
                        Console.WriteLine("Iteration made");
                        Console.SetCursorPosition(1, logY++);
                        Console.WriteLine(word_to_place + "      placed    at    ");
                        Console.SetCursorPosition(29, logY - 1);
                        Console.WriteLine(" " + (xx - space.Length) + "," + yy + " ");
                        Beep(700, 300);
                        Sleep();



                    }
                    // End place

                }
                // End horizontal spaces





                // Work vertical spaces
                for (int i = 0; i < v_spaces.GetLength(0); i++)
                {
                    // Skip if empty space
                    if (v_spaces[i, 1] == null)
                        continue;

                    // Read data
                    string space = v_spaces[i, 0];

                    // Skip if empty
                    if (space.Trim().Length == 0)
                        continue;


                    int xx = int.Parse(v_spaces[i, 1]),
                        yy = int.Parse(v_spaces[i, 2]);

                    string placed = v_spaces[i, 3];

                    // Skip if this space already filled
                    if (placed == "1")
                        continue;


                    string word_to_place = "";
                    int placeable_words = 0;


                    // Work words
                    foreach (string word in words)
                    {
                        // Place flag
                        bool place = true;

                        // Skip if already placed
                        if (words_placed.Contains(word))
                            continue;

                        // Skip if lengths doesn't match
                        if (word.Length != space.Length)
                            continue;

                        // Check for special chars
                        for (int j = 0; j < space.Length; j++)
                        {
                            char c = space[j];

                            // Skip checking spaces
                            if (c == ' ')
                                continue;

                            // Do not place the word if existing char in the space doesn't match
                            if (c != word[j])
                                place = false;
                        }

                        if (place)
                        {
                            word_to_place = word;
                            placeable_words++;
                        }


                    }
                    // End words


                    // Start placing
                    if (placeable_words == 1)
                    {


                        int y_place = yy - word_to_place.Length;

                        foreach (char c in word_to_place)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            Console.SetCursorPosition(xx + 1, y_place + 2);
                            Console.Write(c);

                            Sleep();

                            Console.ResetColor();

                            Console.SetCursorPosition(xx + 1, (y_place++) + 2);
                            Console.Write(c);

                            StringBuilder sb = new StringBuilder(board[y_place - 1]);
                            sb[xx] = c;
                            board[y_place - 1] = sb.ToString();
                            WriteWords();

                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            logY = 20;
                            Console.SetCursorPosition(1, logY++);
                            Console.WriteLine("EVENT LOG");
                            Beep(1000, 200);
                            Sleep();

                        }

                        WriteWordCount();

                        v_spaces[i, 0] = word_to_place;
                        v_spaces[i, 3] = "1";

                        // Mark as placed
                        words_placed[wpi++] = word_to_place;

                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                        Console.SetCursorPosition(1, logY++);
                        Console.WriteLine("Iteration made");
                        Console.SetCursorPosition(1, logY++);
                        Console.WriteLine(word_to_place + "      placed    at    ");
                        Console.SetCursorPosition(29, logY - 1);
                        Console.WriteLine(" " + xx + "," + (yy - space.Length) + " ");
                        Beep(700, 300);
                        Sleep();


                    }
                    // End place



                }
                // End Vertical spaces





                // Finish if all filled
                for (int i = 0; i < h_spaces.GetLength(0); i++)
                {

                    if (h_spaces[i, 0] == null || v_spaces[i, 0] == null)
                        continue;

                    string space_status_h = h_spaces[i, 3],
                            space_status_v = v_spaces[i, 3];

                    if (space_status_h == "1" || space_status_v == "1")
                    {
                        finish = false;
                        break;
                    }

                }


                if (finish)
                {
                    progress = false;
                    WriteWords();
                    Console.SetCursorPosition(1, 20);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("All words are placed.");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    
                    Console.WriteLine(" solution.txt generated");
                    Console.ReadLine();

                }

                //WriteWords();



            } // End progress loop

        }

        // Sleep
        public static void Sleep(int t = 200)
        {
            if (!fast_mode)
                Thread.Sleep(t);
        }

        // Beep
        public static void Beep(int x = 1000, int y = 200)
        {
            if (!fast_mode)
                Console.Beep(x, y);
        }

        // Word Placed
        public static void WriteWordCount(bool increase = true)
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;

            if (increase)
                placed_words_count++;

            string countText = placed_words_count < 10 ? (" " + placed_words_count) : placed_words_count.ToString();

            Console.WriteLine("~~~ WORD =" + countText + " ~~~");
        }

        // Write into solution.txt
        public static void WriteSolution()
        {
            File.WriteAllLines("solution.txt", board);
        }

    }
}