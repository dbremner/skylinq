using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
    public sealed class W3SVCLogColumnMapper : ColumnMapper
    {
        private readonly List<int> masterToActualMap = Enumerable.Repeat(-1, LinqToW3SVCLog.masterDict.Keys.Count).ToList();

        public W3SVCLogColumnMapper(string[] headers)
            : base(headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                masterToActualMap[LinqToW3SVCLog.masterDict[headers[i]]] = i;
            }
        }

        public override int GetColumnNo(int i)
        {
            int mappedColumnNo = masterToActualMap[i];
            if (mappedColumnNo < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Column {0} is not logged.", LinqToW3SVCLog.masterHeaders[i]));
            }

            return mappedColumnNo;
        }
    }
}