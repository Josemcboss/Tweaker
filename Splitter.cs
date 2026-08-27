using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

class Program {
    static void Main() {
        string inputPath = @"c:\Users\Administrator\source\repos\Tweaker\Tweaker\MainWindow.xaml.cs";
        string[] lines = File.ReadAllLines(inputPath);
        
        string usingsAndClass = "";
        int i = 0;
        
        // Find the start of the class and the first region
        List<string> header = new List<string>();
        while (i < lines.Length && !lines[i].TrimStart().StartsWith("#region Title Bar Controls")) {
            header.Add(lines[i]);
            i++;
        }
        
        // We know the closing braces are at the end
        int endBracesCount = 0;
        int j = lines.Length - 1;
        List<string> footer = new List<string>();
        while (j >= 0 && endBracesCount < 2) {
            if (lines[j].Trim() == "}") {
                endBracesCount++;
            }
            footer.Insert(0, lines[j]);
            j--;
        }
        
        // Write the header to MainWindow.xaml.cs (we'll overwrite it later)
        File.WriteAllText("header.txt", string.Join(Environment.NewLine, header));
        File.WriteAllText("footer.txt", string.Join(Environment.NewLine, footer));
        
        Dictionary<string, List<string>> files = new Dictionary<string, List<string>>();
        string currentFile = "MainWindow.TweakHandlers.cs"; // default
        
        int depth = 0;
        
        while (i <= j) {
            string line = lines[i];
            string trimmed = line.TrimStart();
            
            if (trimmed.StartsWith("#region ")) {
                if (depth == 0) {
                    string regionName = trimmed.Substring(8).Trim();
                    if (regionName.Contains("Navegac") || regionName.Contains("Title Bar")) currentFile = "MainWindow.Navigation.cs";
                    else if (regionName.Contains("Dashboard")) currentFile = "MainWindow.Dashboard.cs";
                    else if (regionName.Contains("Smart Scan") || regionName.Contains("Startup Manager")) currentFile = "MainWindow.SmartScan.cs";
                    else if (regionName.Contains("Indicadores Visuales")) currentFile = "MainWindow.Indicators.cs";
                    else if (regionName.Contains("Tweak Info Popups")) currentFile = "MainWindow.Dialogs.cs";
                    else currentFile = "MainWindow.TweakHandlers.cs";
                }
                depth++;
            }
            
            if (!files.ContainsKey(currentFile)) files[currentFile] = new List<string>();
            files[currentFile].Add(line);
            
            if (trimmed.StartsWith("#endregion")) {
                depth--;
            }
            
            i++;
        }
        
        // Now output
        string baseUsings = @"#pragma warning disable CS8600, CS8601, CS8602, CS8603, CS8604, CS8618, CS8619, CS8620, CS8621, CS8622, CS8625, CS0168
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Tweaker.Models;
using Tweaker.Utilities;
using Tweaker.Data;
using Tweaker.Optimizations;

namespace Tweaker
{
    public partial class MainWindow : Window
    {";

        foreach (var kvp in files) {
            string path = Path.Combine(@"c:\Users\Administrator\source\repos\Tweaker\Tweaker\", kvp.Key);
            List<string> outLines = new List<string>();
            outLines.Add(baseUsings);
            outLines.AddRange(kvp.Value);
            outLines.Add("    }");
            outLines.Add("}");
            File.WriteAllLines(path, outLines);
            Console.WriteLine("Created: " + kvp.Key + " with " + kvp.Value.Count + " lines");
        }
        
        // Finally rewrite MainWindow.xaml.cs
        List<string> mainOut = new List<string>(header);
        mainOut.AddRange(footer);
        File.WriteAllLines(inputPath, mainOut);
        Console.WriteLine("Updated: MainWindow.xaml.cs");
    }
}
