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

    public class Cell<T> where T : new()
    {
        private Coord _loc;
        public Coord loc { get { return _loc; } set { } }
        public T value;
        public Cell(Coord c)
        {
            _loc = c;
            value = new T();
        }
    }

    public class Grid<T> where T : new()
    {
        private Coord _gridSize;
        public Coord gridSize { get { return _gridSize; } set { } }
        List<List<Cell<T>>> grid;
        public Grid(Coord param_gridSize)
        {
            _gridSize = param_gridSize;
            grid = new List<List<Cell<T>>>();
            for (int row = 0; row < gridSize.row; row++)
            {
                var gridRow = new List<Cell<T>>();
                for (int col = 0; col < gridSize.col; col++)
                {
                    gridRow.Add(new Cell<T>(new Coord(row, col)));
                }
                grid.Add(gridRow);
            }
        }

        public bool SetCell(Coord loc, T value)
        {
            if (InFrame(loc))
            {
                grid[loc.row][loc.col].value = value;
                return true;
            }
            return false;
        }

        public T GetCell(Coord loc)
        {
            if (InFrame(loc))
            {
                return grid[loc.row][loc.col].value;
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
                    result.SetCell(C, GetCell(offset));
                }
            }     
            return result;
        }

        public List<Cell<T>> EachCell()
        {
            return grid.SelectMany(x => x).ToList();
        }
      
      public List<Cell<T>> EachCellReadingOrder(){
        return grid.SelectMany(x => x).OrderBy(x => x.loc.row).ThenBy(x => x.loc.col).ToList();
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
                    SetCell(offset, grid.GetCell(C));
                }
            }
            return true;
        }
    }

public class ProbabilityVector {
  public double value;
  
  public ProbabilityVector(double val) {
    value = val;
  }
  
  public int resolve(Random rng) {
    int definite = (int)Math.Floor(value);
    double chance = value - definite;
    if (chance >= rng.NextDouble()) {
      definite += 1;
    }
    return definite;
  }
}
