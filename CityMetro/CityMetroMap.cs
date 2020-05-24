using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityMetro
{
  class CityMetroMap
  {
    public class MetroLinkInfo
    {
      public readonly string Station1;
      public readonly string Station2;
      public int Index1 = -1;
      public int Index2 = -1;

      public MetroLinkInfo(string station1, string station2)
      {
        Station1 = station1;
        Station2 = station2;
      }
    }

    private class MetroStationInfo
    {
      public readonly string Station;
      public readonly int Index;
      public int Marker;
      public int Links;

      public MetroStationInfo(string station, int index)
      {
        Station = station;
        Index = index;
      }
    }

    private int[,] mMap;
    private readonly List<MetroStationInfo> mStationList = new List<MetroStationInfo>();
    private readonly List<MetroStationInfo> mResultList = new List<MetroStationInfo>();
    private List<MetroLinkInfo> mLinkInfo;
    private int mIsChecked;
    private int mMinLinks;

    private bool CheckMap(int stationCount)
    {
      if ((mIsChecked == 0) && (mMap != null) && (mStationList.Count > 0))
      {
        int c = mStationList.Count;
        mStationList[0].Marker = 1;
        mMinLinks = int.MaxValue;
        while (mStationList.Exists(s => s.Marker == 1))
        {
          foreach (var v in mStationList)
            if (v.Marker == 1)
            {
              int lc = 0;
              v.Marker = 2;
              for (int i = 0; i < c; i++)
                if ((i != v.Index) && (mMap[v.Index, i] > 0))
                {
                  lc++;
                  var v2 = mStationList[i];
                  if (v2.Marker == 0)
                    v2.Marker = 1;
                }
              v.Links = lc;
              if (mMinLinks > lc)
                mMinLinks = lc;
            }
        }
        mIsChecked = ((mStationList.Count == stationCount) && (mStationList.All(s => s.Marker == 2))) ? 1 : -1;
        if (mMinLinks == int.MaxValue)
          mMinLinks = 0;
      }
      else
        mIsChecked = -1;
      return mIsChecked > 0;
    }

    private void UpdateLinks()
    {
      int c = mStationList.Count;
      mMinLinks = int.MaxValue;
      foreach (var v in mStationList)
        if (v.Marker != 3)
        {
          int lc = 0;
          for (int i = 0; i < c; i++)
            if ((i != v.Index) && (mMap[v.Index, i] > 0))
            {
              var v2 = mStationList[i];
              if (v2.Marker != 3)
                lc++;
            }
          v.Links = lc;
          if (mMinLinks > lc)
            mMinLinks = lc;
        }
      if (mMinLinks == int.MaxValue)
        mMinLinks = 0;
    }

    private void CheckSingleLink()
    {
      if (mMinLinks == 1)
      {
        foreach (var v in mStationList)
          if ((v.Marker != 3) && (v.Links == mMinLinks))
          {
            v.Marker = 3;
            mResultList.Add(v);
          }
        UpdateLinks();
      }
    }

    private void CreateResultList()
    {
      if (mMinLinks > 0)
      {
        CheckSingleLink();
        while (mMinLinks > 0)
        {
          CheckSingleLink();
          if (mMinLinks > 0)
          {
            foreach (var v in mStationList)
              if ((v.Marker != 3) && (v.Links == mMinLinks))
              {
                v.Marker = 3;
                mResultList.Add(v);
                break;
              }
            UpdateLinks();
          }
        }
        foreach (var v in mStationList)
          if (v.Marker != 3)
          {
            v.Marker = 3;
            mResultList.Add(v);
          }
      }
      else
        mError = "weird metro map";
    }

    public void Remap(int stationCount, List<MetroLinkInfo> linkInfo)
    {
      mMap = null;
      mResultList.Clear();
      mStationList.Clear();
      mIsChecked = 0;
      mMinLinks = 0;
      mError = "";
      if ((stationCount > 0) && (linkInfo != null))
      {
        mMap = new int[stationCount, stationCount];
        mLinkInfo = linkInfo;
        foreach (var li in mLinkInfo)
          if (!string.IsNullOrWhiteSpace(li.Station1) && !string.IsNullOrWhiteSpace(li.Station2))
          {
            int i1 = mStationList.FindIndex(s => s.Station.Equals(li.Station1, StringComparison.CurrentCultureIgnoreCase));
            if (i1 < 0)
            {
              i1 = mStationList.Count;
              mStationList.Add(new MetroStationInfo(li.Station1, i1));
            }
            li.Index1 = i1;
            int i2 = mStationList.FindIndex(s => s.Station.Equals(li.Station2, StringComparison.CurrentCultureIgnoreCase));
            if (i2 < 0)
            {
              i2 = mStationList.Count;
              mStationList.Add(new MetroStationInfo(li.Station2, i2));
            }
            li.Index2 = i2;
            if ((i1 < stationCount) && (i2 < stationCount))
            {
              mMap[i1, i2] = 1;
              mMap[i2, i1] = 1;
            }
          }
        if (CheckMap(stationCount))
        {
          CreateResultList();
        }
        else
          mError = "Not all stations connected";
      }
    }


    #region Error

    private string mError = "";

    public string Error
    {
      get
      {
        return mError;
      }
    }

    #endregion

    #region Result

    public string Result
    {
      get
      {
        string res = mError;
        if (string.IsNullOrEmpty(res))
          res = string.Join("\n", mResultList.Select(st => st.Station));
        return res;
      }
    }

    #endregion
  }
}
