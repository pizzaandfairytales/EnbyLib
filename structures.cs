using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class Coord
    {
        public int row, col;

        public Coord(int _row, int _col)
        {
            row = _row;
            col = _col;
        }

        public Coord(int _square)
        {
            row = _square;
            col = _square;
        }
      
      public static Coord Empty;

        public bool InRange(Coord topLeft, Coord bottomRight)
        {
            if (topLeft.row <= row && bottomRight.row >= row
                && topLeft.col <= col && bottomRight.col >= col)
            {
                return true;
            }
            return false;
        }
        public bool Clamp(Coord lowerBound, Coord upperBound)
        {
            var result = false;
            if (row < lowerBound.row)
            {
                result = true;
                row = lowerBound.row;
            }
            if (col < lowerBound.col)
            {
                result = true;
                col = lowerBound.col;
            }
            if (row > upperBound.row)
            {
                result = true;
                row = upperBound.row;
            }
            if (col > upperBound.col)
            {
                result = true;
                col = upperBound.col;
            }
            return result;
        }
        public Coord Times(int factor)
        {
            return new Coord(row * factor, col * factor);
        }
        public Coord FloorTimes(double factor)
        {
            return new Coord(row.FloorTimes(factor), col.FloorTimes(factor));
        }
        public Coord Plus(Coord offset)
        {
            return new Coord(row + offset.row, col + offset.col);
        }
        public Coord Minus(Coord offset)
        {
            return new Coord(row - offset.row, col - offset.col);
        }
        public Coord Difference(Coord offset)
        {
            return new Coord(Math.Abs(row - offset.row), Math.Abs(col - offset.col));
        }
        public Coord FloorDivide(int factor)
        {
            return new Coord(row.FloorDivide(factor), col.FloorDivide(factor));
        }
        public Coord Between(Coord offset, double factor)
        {
            return offset.Minus(this).FloorTimes(factor).Plus(this);
        }
    }

    public class Cell<T> : GraphNode<T> where T : new()
    {
        private Coord _loc;
        public Coord loc { get { return _loc; } set { } }
        public T value;
        public Cell(Coord c)
        {
            _loc = c;
            value = new T();
        }
      
      public override void Output(){
        Console.WriteLine(loc.row + " - " + loc.col);
      }
    }

    public class Grid<T> : Graph<T> where T : new()
    {
        private Coord _gridSize;
        public Coord gridSize { get { return _gridSize; } set { } }
        public Grid(Coord param_gridSize)
        {
            _gridSize = param_gridSize;
            for (int row = 0; row < gridSize.row; row++)
            {
                for (int col = 0; col < gridSize.col; col++)
                {
									var cell = new Cell<T>(new Coord(row, col));
										nodes.Add(cell);
                }
            }
					foreach (Cell<T> cell in EachCell()){
              var coord = cell.loc.Plus(new Coord(-1, 0));
              if (InFrame(coord)){
                cell.neighborEdges.Add(new GraphEdge<T>(GetCell(coord)));
              }
              coord = cell.loc.Plus(new Coord(0, -1));
              if (InFrame(coord)){
                cell.neighborEdges.Add(new GraphEdge<T>(GetCell(coord)));
              }
              coord = cell.loc.Plus(new Coord(1, 0));
              if (InFrame(coord)){
                cell.neighborEdges.Add(new GraphEdge<T>(GetCell(coord)));
              }
              coord = cell.loc.Plus(new Coord(0, 1));
              if (InFrame(coord)){
                cell.neighborEdges.Add(new GraphEdge<T>(GetCell(coord)));
              }
					}
        }

        public bool SetCell(Coord loc, T value)
        {
            if (InFrame(loc))
            {
                GetCell(loc).value = value;
                return true;
            }
            return false;
        }
      
        public Cell<T> GetCell(Coord loc)
        {
            if (InFrame(loc))
            {
                return (Cell<T>)nodes[loc.row * gridSize.col + loc.col];
            }
            throw new Exception("Coordinates out of range");
        }

        public bool InFrame(Coord loc)
        {
            return loc.InRange(new Coord(0,0), gridSize.Plus(new Coord(-1,-1)));
        }

        public Grid<T> Crop(Coord topLeft, Coord size)
        {
            var result = new Grid<T>(size);
            foreach (var C in Methods.EachPoint(size))
            {
                var offset = new Coord(topLeft.row + C.row, topLeft.col + C.col);
                if (!InFrame(offset))
                {
                    result.SetCell(C, new T());
                }
                else
                {
                    result.SetCell(C, GetCell(offset).value);
                }
            }     
            return result;
        }

        public List<Cell<T>> EachCell()
        {
            return nodes.Select(x => (Cell<T>)x).ToList();
        }

        public List<Coord> EachPoint()
        {
            return EachCell().Select(x => x.loc).ToList();
        }

        // to do: "blend" action that's like stamp, but calls a method on T for resolving the interaction
        //      also: revisit this method when adding opacity
        public bool Stamp(Coord topLeft, Grid<T> grid)
        {
            foreach (var C in grid.EachPoint())
            {
                Coord offset = topLeft.Plus(C);
                if (InFrame(offset))
                {
                    SetCell(offset, grid.GetCell(C).value);
                }
            }
            return true;
        }
      public List<List<Cell<T>>> Rows(){
        var result = new List<List<Cell<T>>>();
        for (int row = 0; row < gridSize.row; row++){
          var newRow = new List<Cell<T>>();
          for (int col = 0; col < gridSize.col; col++){
            newRow.Add(GetCell(new Coord(row, col)));
          }
          result.Add(newRow);
        }
        return result;
      }
      public List<List<Cell<T>>> Cols(){
        var result = new List<List<Cell<T>>>();
        for (int col = 0; col < gridSize.col; col++){
          var newCol = new List<Cell<T>>();
          for (int row = 0; row < gridSize.row; row++){
            newCol.Add(GetCell(new Coord(row, col)));
          }
          result.Add(newCol);
        }
        return result;
      }
      
      public List<List<Cell<T>>> FullDiagonals(){
        var result = new List<List<Cell<T>>>();
        if (gridSize.row != gridSize.col){
          return result;
        }
        var downRight = new List<Cell<T>>();
        var upRight = new List<Cell<T>>();
        for(int x = 0; x < gridSize.row; x++){
          downRight.Add(GetCell(new Coord(x, x)));
        }
        for (int x = 0; x < gridSize.row; x++){
          upRight.Add(GetCell(new Coord((gridSize.row - 1) - x, x)));
        }
        result.Add(downRight);
        result.Add(upRight);
        return result;
      }
      
      public List<List<Cell<T>>> AllSubsets(){
        var result = new List<List<Cell<T>>>();
        var rows = Rows();
        var cols = Cols();
        var diags = FullDiagonals();
        foreach (var row in rows){
          result.Add(row);
        }
        foreach (var col in cols){
          result.Add(col);
        }
        foreach (var diag in diags){
          result.Add(diag);
        }
        return result;
      }
    }

public class DijkstraStruct<T>// where T : new()
{
  public GraphNode<T> node;
  public int distance;
	public DijkstraStruct<T> previous;
}

public class GraphEdge<T>
{
	public GraphNode<T> destination;
	public int cost;
	
	public GraphEdge(GraphNode<T> _destination, int _cost = 1){
		destination = _destination;
		cost = _cost;
	}
}

public abstract class GraphNode<T>
{
  public List<GraphEdge<T>> neighborEdges;
  public bool navigable;
  
  public GraphNode(){
    neighborEdges = new List<GraphEdge<T>>();
    navigable = true;
  }
  
  public virtual bool Navigable(){
    return true;
  } 
  
  public abstract void Output();
}

public abstract class Graph<T>{
  public List<GraphNode<T>> nodes;
  
  public Graph(){
    nodes = new List<GraphNode<T>>();
  }
	
	public List<DijkstraStruct<T>> Dijkstra(GraphNode<T> source){
		List<DijkstraStruct<T>> result;
		var Q = new List<DijkstraStruct<T>>();
		foreach (var node in nodes){
			var dstruct = new DijkstraStruct<T>();
			dstruct.node = node;
			if (node == source){
				dstruct.distance = 0;
			} else {
				dstruct.distance = -1;
			}
			dstruct.previous = null;
			Q.Add(dstruct);
		}
		result = new List<DijkstraStruct<T>>(Q);
		while (Q.Where(x => x.distance >= 0).Count() > 0){
      Q = Q.Where(x => x.distance >= 0).OrderBy(x => x.distance).Concat(Q.Where(x => x.distance < 0)).ToList();
			var current = Q.First();
			Q.Remove(current);
			foreach (var neighborEdge in current.node.neighborEdges.Where(x => x.destination.navigable).ToList()){
				var dstructNeighbor = Q.Where(x => x.node == neighborEdge.destination).FirstOrDefault();
				if (dstructNeighbor != null){
					var distance = current.distance + neighborEdge.cost;
					if (dstructNeighbor.distance == -1 || dstructNeighbor.distance < distance){
						dstructNeighbor.distance = distance;
						dstructNeighbor.previous = current;
					}
				}
			}
		}
		return result;
	}
	
	public List<DijkstraStruct<T>> ShortestPath(List<DijkstraStruct<T>> dijkstraMap, GraphNode<T> source, GraphNode<T> destination){
		var current = dijkstraMap.Where(x => x.node == destination).First();
		var result = new List<DijkstraStruct<T>>();
		while (current.previous != null){
			result.Add(current);
			current = current.previous;
		}
		result.Reverse();
		return result;
	}
}

public class ProbabilityVector {
  public double value;
  
  public ProbabilityVector(double val) {
    value = val;
  }
  
  public int Resolve(Random rng) {
    int definite = (int)Math.Floor(value);
    double chance = value - definite;
    if (chance >= rng.NextDouble()) {
      definite += 1;
    }
    return definite;
  }
}

public class ConsoleCell{
  public ArtColor bgColor, fgColor;
  public char c;
  
  public ConsoleCell(){
    bgColor = ArtColor.Black;
    fgColor = ArtColor.White;
    c = ' ';
  }
}

public class ArtColor
    {
        private int _red, _green, _blue, colorRange, _opacity;
        public int red { get => _red; set { _red = value.Clamp(0, colorRange); } }
        public int green { get => _green; set { _green = value.Clamp(0, colorRange); } }
        public int blue { get => _blue; set { _blue = value.Clamp(0, colorRange); } }
        public int opacity { get => _opacity; set { _opacity = value.Clamp(0, 100); } }

        public ArtColor(int r, int g, int b, int o = 100, int _colorRange = 256)
        {
            colorRange = _colorRange - 1;
            red = r;
            green = g;
            blue = b;
            opacity = o;
        }

        public ArtColor()
        {
            colorRange = 255;
            red = colorRange;
            green = colorRange;
            blue = colorRange;
        }

        public System.Drawing.Color Render()
        {
            return System.Drawing.Color.FromArgb(red, green, blue);
        }

        public static readonly ArtColor Black = new ArtColor(0, 0, 0);
        public static readonly ArtColor White = new ArtColor(255, 255, 255);
        public static readonly ArtColor Red = new ArtColor(255, 0, 0);
        public static readonly ArtColor Green = new ArtColor(0, 255, 0);
        public static readonly ArtColor Blue = new ArtColor(0, 0, 255);
        public static readonly ArtColor Cyan = new ArtColor(0, 255, 255);
        public static readonly ArtColor Yellow = new ArtColor(255, 255, 0);
        public static readonly ArtColor Magenta = new ArtColor(255, 0, 255);

        public ArtColor Blend(ArtColor mixer, int mixerPercent = 50)
        {
            if (mixerPercent <= 0)
            {
                return this;
            } else if (mixerPercent >= 100)
            {
                return mixer;
            }
            var thisPercent = 100 - mixerPercent;
            var mixRed = Methods.FloorDivide(((red * (thisPercent * (opacity.Divide(100))))) + (mixer.red * (mixerPercent * (mixer.opacity.Divide(100)))), 100);
            var mixGreen = Methods.FloorDivide(((green * (thisPercent * (opacity.Divide(100))))) + (mixer.green * (mixerPercent * (mixer.opacity.Divide(100)))), 100);
            var mixBlue = Methods.FloorDivide(((blue * (thisPercent * (opacity.Divide(100))))) + (mixer.blue * (mixerPercent * (mixer.opacity.Divide(100)))), 100);
            return new ArtColor(mixRed, mixGreen, mixBlue);
        }
  
  public static List<ArtColor> PaletteExploder(List<ArtColor> input, int numDivisions, bool circular = false)
        {
            var result = new List<ArtColor>();
            for (int x = 1; x < input.Count; x++)
            {
                var color2 = input[x];
                var color1 = input[x - 1];
                result.AddRange(ColorSplitter(color1, color2, numDivisions));
            }
            if (!circular)
            {
                result.Add(input.Last());
            }
            else
            {
                result.AddRange(ColorSplitter(input.Last(), input.First(), numDivisions));
            }
            return result;
        }
  
  public static List<ArtColor> ColorSplitter(ArtColor c1, ArtColor c2, int numDivisions)
        {
            var result = new List<ArtColor>();
            result.Add(c1);
            for (int y = 0; y < numDivisions - 1; y++)
            {
                var blendPercent = 100.FloorDivide(numDivisions) * (y + 1);
                result.Add(c1.Blend(c2, blendPercent));
            }
            return result;
        }
    }

public enum ResultStatus{
    Success,
    Error
  }
  
  public class Result<T>{
    public T Res;
    public ResultStatus Status;
    
    public Result(T _res, ResultStatus _status = ResultStatus.Success){
      Res = _res;
      Status = _status;
    }
  }

class TestController
    {
        TestSuite fullSuite;

        public TestController()
        {
            fullSuite = new TestSuite();

            // Add new test sets to the suit here
            //fullSuite.subTests.Add(new BoardOperationTests());
            //fullSuite.subTests.Add(new PieceMovementTests());
            // MiscTests has full-game bot testing and can take a while so prefer to comment out unless specifically testing that
            //fullSuite.subTests.Add(new MiscTests());
            //fullSuite.subTests.Add(new MLAgentTests());
        }

        public bool run()
        {
            bool result = false;
            string output = fullSuite.run();
            if (output.Length == 0)
            {
                result = true;
                output = "All tests passed.";
            }
            Console.WriteLine(output);
            return result;
        }

    }

    class TestSuite
    {
        public List<TestSuite> subTests;
        public List<Test> tests;

        public TestSuite()
        {
            subTests = new List<TestSuite>();
            tests = new List<Test>();
        }

        public string run()
        {
            string output = "";
            foreach (Test t in tests)
            {
                // Console.WriteLine("Testing: " + t.description);
                if (t.run() == false)
                {
                    output += "Failed: " + t.description + "\n";
                }
            }
            foreach (TestSuite t in subTests)
            {
                output += t.run();
            }
            return output;
        }
    }

    abstract class Test
    {
        public string description;
        abstract public bool run();
    }

    class IntentionalBadTest : Test // who watches the watchers?!
    {
        public IntentionalBadTest()
        {
            description = "This test is designed to fail. Don't include it in a real test suite.";
        }

        public override bool run()
        {
            return false;
        }
    }