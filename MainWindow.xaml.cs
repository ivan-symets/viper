using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Web.Script.Serialization;
using System.Windows.Input;

using VIPER.Presenters;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VIPER.Graph.Window;
using VIPER.StateModels;
using MoreLinq;

using VIPER.ReliabilityWindows;
using VIPER.Helpers;
using System.Dynamic;
using System.Linq.Dynamic;
using System.IO;
using System.Globalization;
using System.Threading;
using Microsoft.Win32;
using Newtonsoft.Json.Schema;
using VIPER.Models;
using Newtonsoft.Json;

namespace VIPER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }



        public MainWindow()
        {

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Language);
            InitializeComponent();
            DataContext = this;






            //     ConfigurationHelper.USeRepairModules = true;


        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        Subsystem subsystem;
        TextBox textFormula;
        TextBox Step;
        TextBox MaxTime;

        private int _count = 22;
        public int Cout
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("test");
            }
        }

        public ObservableCollection<int> Numbers { get; set; } = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            subsystem = base.GetTemplateChild("subsystem") as Subsystem;
            textFormula = base.GetTemplateChild("Formula") as TextBox;
            Step = base.GetTemplateChild("Step") as TextBox;
            MaxTime = base.GetTemplateChild("MaxTime") as TextBox;
        }


        private ConnectionLine CreationLine { get; set; }
        public Dictionary<string, int> aaa = new Dictionary<string, int> { { "Left", 669 }, { "Top", 73 } };
        private void ModuleAddModeChecked(object sender, RoutedEventArgs e)
        {


            subsystem.Mode = CreativeMode.Module;


        }

        private void Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var aa = StringHelper.GetSubScript(123456789);
                Presenters.Path.PathNodes.Clear();
                ConfigurationHelper.Modules.Clear();


                Presenters.Path.logFormElements.Add($"({DateTime.Now:H:mm:ss}) Action: Get condirion.");
                System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                myStopwatch.Start();
                textFormula.Text = subsystem.GetFormula();

                var count = ConfigurationHelper.Modules.Count();

                var aaa = StringHelper.GetUpScript(textFormula.Text);
                ConfigurationHelper.WorkCondition = textFormula.Text;

                myStopwatch.Stop();
                var ms = myStopwatch.Elapsed;


                Presenters.Path.logFormElements.Add($"Get  condition time {ms} ms.");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void NodeAddModeChecked(object sender, RoutedEventArgs e)
        {
            subsystem.Mode = CreativeMode.Node;
        }

        private void LineAddModeChecked(object sender, RoutedEventArgs e)
        {
            subsystem.Mode = CreativeMode.LineBegin;
        }

        private void ModeUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //subsystem.Mode = CreativeMode.None;
            //RB1.IsChecked = false;
            //RB2.IsChecked = false;
            //RB3.IsChecked = false;
        }

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {
            subsystem.Mode = CreativeMode.Node;
        }


        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            subsystem.Clear();
        }


        private void Delete_Select_Element(object sender, RoutedEventArgs e)
        {
            subsystem.isSelectDelete = !subsystem.isSelectDelete;

        }

        private void ToFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            ConfigurationHelper.CountDragingElement = Convert.ToInt32((e.AddedItems[0] as ComboBoxItem).Content);
        }
        private void ComboBox_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            ConfigurationHelper.CountDragingElementLine = (e.AddedItems[0] as ComboBoxItem).Content as string;
        }

        private void GetGraph_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationHelper.Step = Convert.ToDouble(Step.Text);
            ConfigurationHelper.MaxTime = Convert.ToDouble(MaxTime.Text);


            if (String.IsNullOrEmpty(ConfigurationHelper.WorkCondition))
            {
                MessageBox.Show(($"{Properties.Resources.ErrorNoCondition}"));
            }
            else
            {

                GraphWindow graphWindow = new GraphWindow();
                graphWindow.scaleDetailGraph.ScaleX = 0.82644628099173545;
                graphWindow.scaleDetailGraph.ScaleY = 0.82644628099173545;

                graphWindow.scaleGraph.ScaleX = 0.62092132305915493;
                graphWindow.scaleGraph.ScaleY = 0.62092132305915493;


                graphWindow.ShowDialog();
            }

        }

        private void GetReliability_Click(object sender, RoutedEventArgs e)
        {
            ReliabilityWindow readabilityWindow = new ReliabilityWindow();



            readabilityWindow.ShowDialog();
        }

        private void Use_Repair_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationHelper.UseRepairModules = !ConfigurationHelper.UseRepairModules;
        }

        private void ShowGraph_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationHelper.ComputeGraph = !ConfigurationHelper.ComputeGraph;
        }

        private void CalculateEquations_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationHelper.ComputeEquations = !ConfigurationHelper.ComputeEquations;
        }

        private void ShowCharts_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationHelper.ComputeCharts = !ConfigurationHelper.ComputeCharts;

            if (ConfigurationHelper.ComputeCharts)
            {
                ConfigurationHelper.ComputeEquations = true;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var canvas = subsystem.schema;

            var jsonSchemaObjects = new Models.JsonSchema();
            var canvasElements = canvas.Children;


            foreach (object element in canvasElements)
            {

                if (element is Module)
                {
                    var module = element as Module;


                    if (!jsonSchemaObjects.JsonSchemaModules.Any(a => a.Name == module.ModuleName))
                        jsonSchemaObjects.JsonSchemaModules.Add(new JsonSchemaModule(module));

                    if (!jsonSchemaObjects.JsonSchemaConnectionLines.Any(a => a.Name == module.InputLine?.Name) && module.InputLine != null)
                        jsonSchemaObjects.JsonSchemaConnectionLines.Add(new JsonSchemaConnectionLine(module.InputLine));

                    if (!jsonSchemaObjects.JsonSchemaConnectionLines.Any(a => a.Name == module.OutputLine?.Name) && module.OutputLine != null)
                        jsonSchemaObjects.JsonSchemaConnectionLines.Add(new JsonSchemaConnectionLine(module.OutputLine));
                }
                else if (element is Node)
                {
                    var node = element as Node;


                    if (!jsonSchemaObjects.JsonSchemaNodes.Any(a => a.Name == node.Name))
                        jsonSchemaObjects.JsonSchemaNodes.Add(new JsonSchemaNode(node));
                }
                else if (element is ConnectionLine)
                {
                    var connectionLine = element as ConnectionLine;

                    if (!jsonSchemaObjects.JsonSchemaConnectionLines.Any(a => a.Name == connectionLine.Name))
                        jsonSchemaObjects.JsonSchemaConnectionLines.Add(new JsonSchemaConnectionLine(connectionLine));

                    if (connectionLine.Element1 is Module && !jsonSchemaObjects.JsonSchemaModules.Any(a => a.Name == connectionLine.Element1?.Name) && connectionLine.Element1 != null)
                        jsonSchemaObjects.JsonSchemaModules.Add(new JsonSchemaModule(connectionLine.Element1 as Module));

                    if (connectionLine.Element2 is Module && !jsonSchemaObjects.JsonSchemaModules.Any(a => a.Name == connectionLine.Element2?.Name) && connectionLine.Element2 != null)
                        jsonSchemaObjects.JsonSchemaModules.Add(new JsonSchemaModule(connectionLine.Element2 as Module));

                    if (connectionLine.Element1 is Node && !jsonSchemaObjects.JsonSchemaNodes.Any(a => a.Name == connectionLine.Element1?.Name) && connectionLine.Element1 != null)
                        jsonSchemaObjects.JsonSchemaNodes.Add(new JsonSchemaNode(connectionLine.Element1 as Node));

                    if (connectionLine.Element2 is Node && !jsonSchemaObjects.JsonSchemaNodes.Any(a => a.Name == connectionLine.Element2?.Name) && connectionLine.Element2 != null)
                        jsonSchemaObjects.JsonSchemaNodes.Add(new JsonSchemaNode(connectionLine.Element1 as Node));

                }
            }

            var json = JsonConvert.SerializeObject(jsonSchemaObjects);

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Json files (*.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, json);
            }


        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();




            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json files (*.json)|*.json";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            textFormula.Text = String.Empty;

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                subsystem.schema.Clear();

                using (StreamReader r = new StreamReader(dlg.FileName))
                {
                    string json = r.ReadToEnd();

                    var jsonSerialiser = new JavaScriptSerializer();
                    var jsonSchema = JsonConvert.DeserializeObject<Models.JsonSchema>(json);

                    var shemaElements = new List<DraggingElement>();
                    var shemaLines = new List<ConnectionLine>();

                    foreach (var element in jsonSchema.JsonSchemaModules)
                    {

                        var module = element.ToModule();

                        subsystem.schema.Children.Add(module);
                        Canvas.SetLeft(module, element.InputCoordinates.X);
                        Canvas.SetTop(module, element.InputCoordinates.Y);

                        shemaElements.Add(module);

                    }

                    foreach (var element in jsonSchema.JsonSchemaConnectionLines)
                    {
                        var connectionLine = element.ToConnectionLine();

                

                        subsystem.schema.Children.Add(connectionLine);
                        shemaLines.Add(connectionLine);

                    }

                    foreach (var element in jsonSchema.JsonSchemaNodes)
                    {
                        var node = element.ToNode();

                        if (!node.IsStartNode && !node.IsEndNode)
                        {
                            subsystem.schema.Children.Add(node);
                            Canvas.SetLeft(node, element.Center.X);
                            Canvas.SetTop(node, element.Center.Y);
                        }

                        if(node.IsStartNode)
                            shemaElements.Add(subsystem.schema.startNode);

                        if (node.IsEndNode)
                            shemaElements.Add(subsystem.schema.endNode);

                    }

                    foreach (var line in shemaLines)
                    {
                        var jsonLine = jsonSchema.JsonSchemaConnectionLines.FirstOrDefault(o => o.Name == line.Name);

                        if (shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element1Name) is Module)
                        {
                            var element1Module = shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element1Name) as Module;
                            element1Module.SelectedEllipse = Module.OutputCoordinatesProperty;
                            line.Element1 = element1Module;
                            element1Module.OutputLine = line;
                        }

                        if (shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element2Name) is Module)
                        {
                            var element2Module = shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element2Name) as Module;
                            element2Module.SelectedEllipse = Module.InputCoordinatesProperty;
                            line.Element2 = element2Module;
                            element2Module.InputLine = line;
                        }

                        if (shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element1Name) is Node)
                        {
                            var element1Node = shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element1Name) as Node;
                            line.Element1 = element1Node;
                            element1Node.ConnectionLines.Add(line);
                        }

                        if (shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element2Name) is Node)
                        {
                            var element2Node = shemaElements.FirstOrDefault(o => o.Name == jsonLine.Element2Name) as Node;
                            line.Element2 = element2Node;
                            element2Node.ConnectionLines.Add(line);
                        }
                    }
                }
            }
        }
    }
}
