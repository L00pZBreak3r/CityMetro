using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CityMetro
{
  class Program
  {

    private static List<CityMetroMap.MetroLinkInfo> ParseInputFile(string fileName, out int c)
    {
      List<CityMetroMap.MetroLinkInfo> res = null;
      c = 0;
      if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
      {
        using (var sr = File.OpenText(fileName))
        {
          string s = "";
          while ((s = sr.ReadLine()) != null)
          {
            if (!string.IsNullOrWhiteSpace(s))
            {
              var a = s.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
              if (a.Length > 0)
                if (c > 0)
                {
                  if (a.Length > 1)
                    res.Add(new CityMetroMap.MetroLinkInfo(a[0].Trim(), a[1].Trim()));
                }
                else
                {
                  if (int.TryParse(a[0].Trim(), out c) && (c > 0))
                    res = new List<CityMetroMap.MetroLinkInfo>();
                  else
                    break;
                }
            }
          }
        }
      }
      return res;
    }

    static void Main(string[] args)
    {
      string inpfile = "input.txt";
      string outfile = "output.txt";
      if (args.Length > 0)
      {
        inpfile = args[0];
        if (args.Length > 1)
          outfile = args[1];
      }

      int c;
      var li = ParseInputFile(inpfile, out c);
      string r = "Cannot read input file";
      if (li != null)
      {
        var m = new CityMetroMap();
        m.Remap(c, li);
        r = m.Result;
      }
      File.WriteAllText(outfile, r);
    }
  }
}
