using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
    public class W3SVCLogColumnMapper : ColumnMapper
    {
        protected int[] masterToActualMap = Enumerable.Repeat(-1, LinqToW3SVCLog.masterDict.Keys.Count).ToArray();

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
                throw new ArgumentOutOfRangeException(string.Format("Column {0} is not logged.", LinqToW3SVCLog.masterHeaders[i]));

            return mappedColumnNo;
        }
    }

    public class W3SVCLogRecord : Record
    {
        public W3SVCLogRecord(W3SVCLogColumnMapper mapper, string[] fields) : base(mapper, fields) { }

        public DateTime DateTime
        {
            get { return DateTime.Parse(_fields[_mapper.GetColumnNo(0)] + ' ' + _fields[_mapper.GetColumnNo(1)]); }
        }

        public string ServiceName
        {
            get { return _fields[_mapper.GetColumnNo(2)]; }
        }

        public string ServerName
        {
            get { return _fields[_mapper.GetColumnNo(3)]; }
        }

        public string ServerIP
        {
            get { return _fields[_mapper.GetColumnNo(4)]; }
        }

        public string Method
        {
            get { return _fields[_mapper.GetColumnNo(5)]; }
        }

        public string URIStem
        {
            get { return _fields[_mapper.GetColumnNo(6)]; }
        }

        public string URIQuery
        {
            get { return _fields[_mapper.GetColumnNo(7)]; }
        }

        public int ServerPort
        {
            get { return Convert.ToInt32(_fields[_mapper.GetColumnNo(8)]); }
        }

        public string UserName
        {
            get { return _fields[_mapper.GetColumnNo(9)]; }
        }

        public string ClientIP
        {
            get { return _fields[_mapper.GetColumnNo(10)]; }
        }

        public string UserAgent
        {
            get { return _fields[_mapper.GetColumnNo(11)]; }
        }

        public string Referer
        {
            get { return _fields[_mapper.GetColumnNo(12)]; }
        }

        public int Status
        {
            get { return Convert.ToInt32(_fields[_mapper.GetColumnNo(13)]); }
        }

        public int SubStatus
        {
            get { return Convert.ToInt32(_fields[_mapper.GetColumnNo(14)]); }
        }

        public int Win32Status
        {
            get { return Convert.ToInt32(_fields[_mapper.GetColumnNo(15)]); }
        }

        public int BytesSent
        {
            get { return Convert.ToInt32(_fields[_mapper.GetColumnNo(16)]); }
        }

        public int BytesReceived
        {
            get { return Convert.ToInt32(_fields[_mapper.GetColumnNo(17)]); }
        }

        public int TimeTaken
        {
            get { return Convert.ToInt32(_fields[_mapper.GetColumnNo(18)]); }
        }

        public string ProtocolVersion
        {
            get { return _fields[_mapper.GetColumnNo(19)]; }
        }

        public string Host
        {
            get { return _fields[_mapper.GetColumnNo(20)]; }
        }

        public string Cookie
        {
            get { return _fields[_mapper.GetColumnNo(21)]; }
        }

    }
    
    public static class LinqToW3SVCLog
    {
        internal static string[] masterHeaders = "date time s-sitename s-computername s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status sc-bytes cs-bytes time-taken cs-version cs-host cs(Cookie)"
            .Split(' ');
        internal static IDictionary<string, int> masterDict = masterHeaders.ToPositionDictionary();

        public static IEnumerable<W3SVCLogRecord> EnumW3SVCLogRecords(this IEnumerable<string> lines)
        {
            return lines.EnumRecords(
                (n, s) =>
                {
                    if (s.StartsWith("#Fields:"))
                    {
                        return new Tuple<LineType, string[]>(LineType.Header, s.Substring(9).Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else if (s.StartsWith("#"))
                    {
                        return new Tuple<LineType, string[]>(LineType.Comments, new string[] { s });
                    }
                    else
                    {
                        return new Tuple<LineType, string[]>(LineType.Data, s.Split(' '));
                    }
                },
                headers => new W3SVCLogColumnMapper(headers),
                (mapper, fields) => new W3SVCLogRecord(mapper, fields)
                );
        }
    }
}
