using System;
using System.Linq;
using System.Text;

namespace CrmCommandLineHelpersCore.CommandLine
{
    public static class ConsoleHelper
    {
        public static LogLevel LogLevel = LogLevel.Debug;

        public static void PrintFatalError(Exception ex)
        {
            WriteLine("=== ERROR ===", foregroundColor: ConsoleColor.DarkRed);
            WriteLine();
            PrintError(ex);
        }

        public static void PrintError(Exception ex)
        {
            WriteLine(ex.GetType().Name, indent: 1);
            WriteLine();
            var aggException = ex as AggregateException;
            if (aggException != null)
            {
                WriteLine(ex.Message, indent: 1);
                WriteLine();
                aggException.InnerExceptions.ToList().ForEach(innerEx => WriteLine("> " + innerEx.Message, indent: 1));
            }
            else
            {
                WriteLine(ex.Message, indent: 1);
            }
            WriteLine();
        }

        public static void WriteLine(string line = "", int indent = 0, int? wrappedIndent = null, int rightPad = 1, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null, LogLevel logLevel = LogLevel.Info)
        {
            if ((int)logLevel < (int)LogLevel) return;
            var wrappedText = WrapAndIndentText(line, GetCursorLeft(), indent, wrappedIndent ?? indent, GetBufferWidth() - rightPad);
            SetColorsForAction(() => Console.WriteLine(wrappedText), foregroundColor, backgroundColor);
        }

        public static void Write(string text, int indent = 0, int? wrappedIndent = null, int rightPad = 1, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null, LogLevel logLevel = LogLevel.Info)
        {
            if ((int)logLevel < (int)LogLevel) return;
            var wrappedText = WrapAndIndentText(text, GetCursorLeft(), indent, wrappedIndent ?? indent, GetBufferWidth() - rightPad);
            SetColorsForAction(() => Console.Write(wrappedText), foregroundColor, backgroundColor);
        }

        private static int GetCursorLeft()
        {
            return GetConsoleProperty(() => Console.CursorLeft, 0);
        }

        private static int GetBufferWidth()
        {
            return GetConsoleProperty(() => Console.BufferWidth, int.MaxValue);
        }

        private static T GetConsoleProperty<T>(Func<T> action, T defaultValue)
        {
            if (Console.IsInputRedirected) return defaultValue;
            try
            {
                return action.Invoke();
            }
            catch (Exception)
            {
                // System.IO Exceptions can result if the code is run through a non-standard console
                return defaultValue;
            }
        }

        private static void SetConsoleProperty(Action action)
        {
            if (Console.IsInputRedirected) return;
            try
            {
                action.Invoke();
            }
            catch (Exception)
            {
                // System.IO Exceptions are caused when run through a non-standard console
            }
        }

        private static void SetColorsForAction(Action action, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
        {
            SetBackgroundColorForAction(() => SetForegroundColorForAction(action, foregroundColor), backgroundColor);
        }

        private static void SetForegroundColorForAction(Action action, ConsoleColor? foregroundColor)
        {
            if (foregroundColor == null)
            {
                action.Invoke();
                return;
            }
            var oldForegroundColor = GetConsoleProperty(() => Console.ForegroundColor, ConsoleColor.White);
            SetConsoleProperty(() => Console.ForegroundColor = foregroundColor.Value);
            action.Invoke();
            SetConsoleProperty(() => Console.ForegroundColor = oldForegroundColor);
        }

        private static void SetBackgroundColorForAction(Action action, ConsoleColor? backgroundColor)
        {
            if (backgroundColor == null)
            {
                action.Invoke();
                return;
            }
            var oldBackgroundColor = GetConsoleProperty(() => Console.BackgroundColor, ConsoleColor.Black);
            SetConsoleProperty(() => Console.BackgroundColor = backgroundColor.Value);
            action.Invoke();
            SetConsoleProperty(() => Console.BackgroundColor = oldBackgroundColor);
        }

        private static string WrapAndIndentText(string text, int firstLineStartsAt, int firstLineIndent, int generalIndent, int wrapAt, bool removeWhitespaceAtStartOfFutureLines = true, bool wrapAtWhitespace = true)
        {
            var currentStringPosition = 0;

            var output = new StringBuilder();
            AppendNextLine(output, text, ref currentStringPosition, firstLineIndent, wrapAt - firstLineStartsAt, wrapAtWhitespace);

            while (currentStringPosition < text.Length)
            {
                if (removeWhitespaceAtStartOfFutureLines && IsWhiteSpace(text[currentStringPosition]))
                {
                    currentStringPosition++;
                    continue;
                }

                output.Append(Environment.NewLine);
                AppendNextLine(output, text, ref currentStringPosition, generalIndent, wrapAt, wrapAtWhitespace);
            }

            return output.ToString();
        }

        private static void AppendNextLine(StringBuilder builder, string text, ref int position, int indent, int wrapAt, bool wrapAtWhitespace)
        {
            var maxlineWidth = wrapAt > indent ? wrapAt - indent : wrapAt; // In console too narrow, work around it
            var lineWidth = maxlineWidth;

            bool atEnd;
            var possibleNextLine = GetPossibleNextLine(text, position, lineWidth, out atEnd);
            string nextLine;

            if (!atEnd && wrapAtWhitespace)
            {
                // ReSharper disable once PossibleInvalidOperationException
                var lastWhitespaceCharacterIndex = Enumerable.Range(0, possibleNextLine.Length).Cast<int?>().LastOrDefault(i => IsWhiteSpace(possibleNextLine[i.Value]));
                nextLine = !lastWhitespaceCharacterIndex.HasValue ? possibleNextLine : GetPossibleNextLine(text, position, lastWhitespaceCharacterIndex.Value, out atEnd);
            }
            else
            {
                nextLine = possibleNextLine;
            }

            position += nextLine.Length;
            var padding = new string(Enumerable.Range(0, indent).Select(i => ' ').ToArray());
            builder.Append(padding);
            builder.Append(nextLine);
        }

        private static string GetPossibleNextLine(string text, int position, int numberChars, out bool atEnd)
        {
            var oldPosition = position;
            position += numberChars;
            if (position < text.Length)
            {
                atEnd = false;
                return text.Substring(oldPosition, position - oldPosition);
            }
            atEnd = true;
            return text.Substring(oldPosition);
        }

        private static bool IsWhiteSpace(char c)
        {
            return string.IsNullOrWhiteSpace(c.ToString());
        }
    }
}
