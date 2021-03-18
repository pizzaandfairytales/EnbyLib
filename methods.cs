using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public static class Methods
    {
        public static int Clamp(this int val, int min, int max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static int FloorDivide(this int numerator, int denominator)
        {
            return (int)(Math.Floor(numerator / (double)denominator));
        }

        public static int FloorDivide(this int numerator, double denominator)
        {
            return (int)(Math.Floor(numerator / denominator));
        }

        public static int FloorDivide(this double numerator, int denominator)
        {
            return (int)(Math.Floor(numerator / denominator));
        }

        public static int FloorDivide(this double numerator, double denominator)
        {
            return (int)(Math.Floor(numerator / denominator));
        }

        public static double Divide(this int numerator, int denominator)
        {
            return (double)numerator / denominator;
        }

        public static int FloorTimes(this int num, double factor)
        {
            return (int)(Math.Floor(num * factor));
        }
      
      public static void ForEach<T>(
    this IEnumerable<T> source,
    Action<T> action)
{
    foreach (T element in source) 
        action(element);
}
      
      public static void Times(this int times, Action action){
        for (int x = 0; x < times; x++){
          action();
        }
      }

        public static List<Coord> EachPoint(this Coord range, bool horizontal = true)
        {
            var result = new List<Coord>();
          if (horizontal){
            for (int row = 0; row < range.row; row++)
            {
                for (int col = 0; col < range.col; col++)
                {
                    result.Add(new Coord(row, col));
                }
            }
          } else {
            for (int col = 0; col < range.col; col++)
            {
                for (int row = 0; row < range.row; row++)
                {
                    result.Add(new Coord(row, col));
                }
            }
          }
            
            return result;
        }

        public static List<List<T>> InitializeRect<T>(this List<List<T>> rect, Coord size) where T : new()
        {
            var result = new List<List<T>>();
            for (int row = 0; row < size.row; row++)
            {
                var r = new List<T>();
                for (int col = 0; col < size.col; col++)
                {
                    r.Add(new T());
                }
                result.Add(r);
            }
            return result;
        }

        public static Grid<T> ToGrid<T>(this List<T> list, Coord size) where T : new()
        {
            var grid = new Grid<T>(size);
            foreach (var C in grid.EachCell())
            {
                C.value = list[(C.loc.row * size.col) + C.loc.col];
            }
            return grid;
        }
      
        public static List<T> Draw<T>(this List<T> list, int amount, Random rng) {
          var result = new List<T>(list);
          result.Shuffle(rng);
          return result.GetRange(0, amount);
        }
      
        public static void Shuffle<T>(this List<T> list, Random rng)  {  
          int n = list.Count;  
          while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
     }
      
      public static List<T> InterpolatePattern<T>(this List<T> list, Func<T> patternGenerator, int period){
        var result = new List<T>();  
        for (int x = 0; x < list.Count; x++){
            if (x % period == 0){
              result.Add(patternGenerator());
            }
          result.Add(list[x]);
          }
        return result;
        }
      
      public static bool IsSubsetOf<T>(this List<T> list, List<T> masterList){
        return !list.Except(masterList).Any();
      }
  }
