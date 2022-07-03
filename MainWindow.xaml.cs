// Decompiled with JetBrains decompiler
// Type: MyCalculatorv1.MainWindow
// Assembly: MyCalculatorv1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8E4247A5-25E4-47A6-84F4-A414933F7536
// Assembly location: C:\Training\.NET Intermediate 2022 Q2\04. Analyzing and profiling tools\DumpHomework\MyCalculator.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MyCalculatorv1
{
    public partial class MainWindow : Window, IComponentConnector
    {
        private const char Multiply = '*';
        private const char Divide = '/';
        private const char Plus = '+';
        private const char Minus = '-';
        private const char Equal = '=';

        private readonly char[] Actions = new char[] { Multiply, Divide, Plus, Minus};

        public MainWindow() => this.InitializeComponent();

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (char.TryParse((string)((Button)sender).Content, out var value))
            {
                var equalSign = tb.Text.IndexOf(Equal);
                if (equalSign > 0)
                {
                    tb.Text = Actions.Contains(value) 
                        ? $"{tb.Text.Substring(equalSign + 1)}{value}" 
                        : $"{value}";
                }
                else if (!string.IsNullOrEmpty(tb.Text))
                {
                    var isAction = Actions.Contains(value);
                    var lastChar = tb.Text.Last();
                    if (isAction && Actions.Contains(lastChar))
                    {
                        tb.Text = $"{tb.Text.TrimEnd(lastChar)}{value}";
                    }
                    else
                    {
                        tb.Text += value;
                    }
                }
                else
                {
                    tb.Text = $"{value}";
                }
            }
        }

        private void Result_click(object sender, RoutedEventArgs e) => this.GetResult();

        private void GetResult()
        {
            if (string.IsNullOrEmpty(tb.Text) || tb.Text.Contains(Equal) || !tb.Text.Any(x => Actions.Contains(x)))
            {
                return;
            }

            var numbers = GetNumbers();
            if (numbers.Length <= 1)
            {
                return;
            }

            var actions = GetActions();
            for (int i = 1; i < numbers.Length; i++)
            {
                var action = actions.Dequeue();
                if (IsPriority(action))
                {
                    var left = numbers[i - 1].Value;
                    var right = numbers[i].Value;
                    numbers[i - 1] = null;
                    numbers[i] = GetResult(left, right, action);
                }
                else
                {
                    actions.Enqueue(action);
                }
            }

            numbers = numbers.Where(x => x.HasValue).ToArray();
            for (int i = 1; i < numbers.Length; i++)
            {
                var action = actions.Dequeue();
                var left = numbers[i - 1].Value;
                var right = numbers[i].Value;
                numbers[i] = GetResult(left, right, action);
            }

            var lastChar = tb.Text.Last();
            tb.Text = Actions.Contains(lastChar)
                    ? $"{tb.Text.TrimEnd(lastChar)}{numbers[numbers.Length - 1]}"
                    : $"{tb.Text}{Equal}{numbers[numbers.Length - 1]}";
        }

        private double?[] GetNumbers() => tb.Text.Split(Actions, StringSplitOptions.RemoveEmptyEntries)
                            .Select<string, double?>((x, i) =>
                            {
                                if (i == 0 && tb.Text.First() == Minus)
                                {
                                    return -double.Parse(x);
                                }
                                return double.Parse(x);
                            }).ToArray();

        private Queue<char> GetActions() => new Queue<char>(tb.Text
                            .Skip(1)
                            .Take(tb.Text.Length - 2)
                            .Where(x => Actions.Contains(x)));

        private void Off_Click_1(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Del_Click(object sender, RoutedEventArgs e) => this.tb.Text = "";

        private void R_Click(object sender, RoutedEventArgs e)
        {
            if (this.tb.Text.Length <= 0)
                return;
            this.tb.Text = this.tb.Text.Substring(0, this.tb.Text.Length - 1);
        }

        private double GetResult(double left, double right, char action)
        {
            switch (action)
            {
                case Plus:
                    return left + right;
                case Minus:
                    return left - right;
                case Multiply:
                    return left * right;
                case Divide:
                    return left / right;
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool IsPriority(char c) => c == Multiply || c == Divide;
    }
}
