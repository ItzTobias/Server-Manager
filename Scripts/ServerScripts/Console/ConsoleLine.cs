using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Server_Manager.Scripts.ServerScripts
{
    public class ConsoleLine : INotifyPropertyChanged
    {
        public static ObservableCollection<ConsoleLine> Lines { get; private set; } = new ObservableCollection<ConsoleLine>();

        private string data;
        public string Data
        {
            get => data;
            private set
            {
                data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
            }
        }
        public Brush Color { get; private set; }

        private readonly List<string> columns = new();
        private readonly string value;
        private int dupeCount;
        public int DupeCount
        {
            get => dupeCount;
            set
            {
                dupeCount = value;
                ReformatRequired?.Invoke();
            }
        }

        private static readonly List<int> hiddenColumns = new();
        private static RefReq ReformatRequired;

        private delegate void RefReq();
        public event PropertyChangedEventHandler PropertyChanged;

        public ConsoleLine(string data, Brush color)
        {
            Color = color;

            string[] columnValue = data.Split(": ", 2);

            if (columnValue.Length == 2)
            {
                string[] values = columnValue[0].Split(']', StringSplitOptions.RemoveEmptyEntries);
                value = columnValue[1];

                values[0] += ']';
                if (values.Length > 1)
                {
                    for (int i = 1; i < values.Length; i++)
                    {
                        values[i] = values[i].Remove(0, 1) + ']';
                    }
                }

                columns.AddRange(values);
            }
            else
            {
                value = columnValue[0];
            }

            ReformatRequired += Reformat;
            ReformatRequired?.Invoke();
        }

        public void Reformat()
        {
            StringBuilder format = new();

            if (columns.Count > 0)
            {
                for (int i = 0; i < columns.Count - 1; i++)
                {
                    if (hiddenColumns.Contains(i))
                    {
                        continue;
                    }

                    format.Append(columns[i]);
                    format.Append(' ');
                }
                if (!hiddenColumns.Contains(columns.Count - 1))
                {
                    format.Append(columns[^1]);
                }
            }

            if (format.Length != 0)
            {
                format.Append(": ");
            }

            format.Append(value);

            if (DupeCount > 0)
            {
                format.Append(" (");
                format.Append(DupeCount + 1);
                format.Append(')');
            }

            Data = format.ToString();
        }

        public static void Add(string data)
        {
            Add(data, MessageType.normal);
        }

        public static void Add(string data, MessageType type)
        {
            Lines.Add(type switch
            {
                MessageType.highlighted => new ConsoleLine(data, App.whiteBrush),
                MessageType.error => new ConsoleLine(data, App.redBrush),
                MessageType.green => new ConsoleLine(data, App.greenBrush),
                _ => new ConsoleLine(data, App.fontColorBrush),
            });
        }

        private static void ReformatAll()
        {
            foreach (var line in Lines)
            {
                line.Reformat();
            }
        }

        public static void SetColumn(int column, ColumnAction action)
        {
            if (action == ColumnAction.Hide)
            {
                if (hiddenColumns.Contains(column))
                {
                    return;
                }

                hiddenColumns.Add(column);
            }
            else
            {
                if (!hiddenColumns.Remove(column))
                {
                    return;
                }
            }

            ReformatAll();
        }

        public static void IncreaseDupeCount()
        {
            Lines.Last().DupeCount++;
        }
    }
}
