using System;
using System.IO;
using System.Linq;
using System.Text;

namespace WriteFixedLength
{
    public class FixedWidthWriter : StreamWriter
    {
        public char NonDataCharacter { get; set; }

        public int[] Widths { get; set; }

        public FixedWidthWriter(int[] widths, string path, bool append, Encoding encoding)
            : base(path, append, encoding)
        {
            NonDataCharacter = ' ';
            Widths = widths;
        }

        public FixedWidthWriter(int[] widths, string file) : this(widths, file, false, Encoding.UTF8) { }

        public FixedWidthWriter(int[] widths, string file, bool append) : this(widths, file, append, Encoding.UTF8) { }

        public void WriteLine(string[] data)
        {
            if (data.Length > Widths.Length)
                throw new InvalidOperationException("The data has too many elements.");

            for (int i = 0; i < data.Length; i++)
            {
                WriteField(data[i], Widths[i]);
            }
            WriteLine();
        }

        private void WriteField(string datum, int width)
        {
            char[] characters = datum.ToCharArray();
            if (characters.Length > width)
            {
                Write(characters, 0, width);
            }
            else
            {
                Write(characters);
                Write(new string(NonDataCharacter, width - characters.Length));
            }
        }
    }
}
