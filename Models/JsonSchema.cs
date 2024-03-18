
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using VIPER.Presenters;

namespace VIPER.Models
{

    internal class JsonSchema
    {
        public List<JsonSchemaModule> JsonSchemaModules { get; set; } = new List<JsonSchemaModule>();
        public List<JsonSchemaConnectionLine> JsonSchemaConnectionLines { get; set; } = new List<JsonSchemaConnectionLine>();
        public List<JsonSchemaNode> JsonSchemaNodes { get; set; } = new List<JsonSchemaNode>();
    }

    internal class JsonSchemaObject
    {
        public string Name { get; set; }
        public CreativeMode Type { get; set; }
    }

    internal class JsonSchemaModule: JsonSchemaObject
    {
        public int Number { get; set; }

        public double Lambda { get; set; }
        public double Mu { get; set; }

        public int RepairCount { get; set; }

        public string InputLineName { get; set; }
        public string OutputLineName { get; set; }

        public Point InputCoordinates { get; set; }
        public Point OutputCoordinates { get; set; }
        public JsonSchemaModule()
        {
        }

        public JsonSchemaModule(Module module)
        {
            this.Type = CreativeMode.Module;
            this.Name = module.Name;
            this.Number = module.Number;
            this.Lambda = module.Lambda;
            this.Mu = module.Mu;
            this.RepairCount = module.RepairCount;
            this.InputLineName = module.InputLine?.Name; 
             this.OutputLineName = module.OutputLine?.Name;
            this.InputCoordinates = module.InputCoordinates;
            this.OutputCoordinates = module.OutputCoordinates;
        }

        public Module ToModule()
        {
            var module = new Module
            {
                Name = this.Name,
                Number = this.Number,
                Lambda = this.Lambda,
                Mu = this.Mu,
                RepairCount = this.RepairCount,
                InputCoordinates = this.InputCoordinates,
                OutputCoordinates = this.OutputCoordinates,
            };

            return module;
        }


    }

    internal class JsonSchemaConnectionLine : JsonSchemaObject
    {
  
        public Point Start { get; set; }

        public Point End { get; set; }

        public string Element1Name { get; set; }
        public string Element2Name { get; set; }

        public JsonSchemaConnectionLine(ConnectionLine connectionLine)
        {
            if (connectionLine == null)
                return;
            this.Type = CreativeMode.Line;
            this.Name = connectionLine.Name;

            this.Start = connectionLine.Start;
            this.End = connectionLine.End;

            this.Element1Name = connectionLine.Element1?.Name;
            this.Element2Name = connectionLine.Element2?.Name;
        }

        public ConnectionLine ToConnectionLine()
        {
            var connectionLine = new ConnectionLine
            {
                Name = this.Name,
                Start = this.Start,
                End = this.End
            };
            return connectionLine;
            }
    }


    internal class JsonSchemaNode : JsonSchemaObject
    {
        #region Properties

        public bool IsEndNode { get; set; }
        public bool IsStartNode { get; set; }

        public List<string> ConnectionLineNames
        {
            get;
            set;
        }

        public Point Center { get; set; }
        #endregion
        public JsonSchemaNode()
        {
        }
        public JsonSchemaNode(Node node)
        {
            this.Name = node.Name;
            this.IsEndNode = node.IsEndNode;
            this.IsStartNode = node.IsStartNode;
            this.ConnectionLineNames = node.ConnectionLines.Select(s=>s.Name).ToList();
            this.Center = node.Center;
            this.Type = CreativeMode.Node;
        }

        public Node ToNode()
        {
            var node = new Node
            {
             
                Name = this.Name,
                IsEndNode = this.IsEndNode,
                IsStartNode = this.IsStartNode,
                Center = this.Center
            };

            return node;
        }

    }
}
