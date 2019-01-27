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

    public interface IW3SVCLogRecord
    {
        DateTime dateTime { get; }
        string c_ip { get; }
        string cs_bytes { get; }
        string cs_Cookie { get; }
        string cs_host { get; }
        string cs_method { get; }
        string cs_Referer { get; }
        string cs_uri_query { get; }
        string cs_uri_stem { get; }
        string cs_User_Agent { get; }
        string cs_username { get; }
        string cs_version { get; }
        string s_computername { get; }
        string s_ip { get; }
        string s_port { get; }
        string s_sitename { get; }
        string sc_bytes { get; }
        string sc_status { get; }
        string sc_substatus { get; }
        string sc_win32_status { get; }
        string time_taken { get; }
    }


    public sealed class W3SVCLogRecord : Record, IW3SVCLogRecord
    {
        public W3SVCLogRecord(W3SVCLogColumnMapper mapper, string[] fields) : base(mapper, fields) { }

        public DateTime dateTime
        {
            get { return DateTime.Parse(_fields[_mapper.GetColumnNo(0)] + ' ' + _fields[_mapper.GetColumnNo(1)]); }
        }

        public string s_sitename
        {
            get { return _fields[_mapper.GetColumnNo(2)]; }
        }

        public string s_computername
        {
            get { return _fields[_mapper.GetColumnNo(3)]; }
        }

        public string s_ip
        {
            get { return _fields[_mapper.GetColumnNo(4)]; }
        }

        public string cs_method
        {
            get { return _fields[_mapper.GetColumnNo(5)]; }
        }

        public string cs_uri_stem
        {
            get { return _fields[_mapper.GetColumnNo(6)]; }
        }

        public string cs_uri_query
        {
            get { return _fields[_mapper.GetColumnNo(7)]; }
        }

        public string s_port
        {
            get { return _fields[_mapper.GetColumnNo(8)]; }
        }

        public string cs_username
        {
            get { return _fields[_mapper.GetColumnNo(9)]; }
        }

        public string c_ip
        {
            get { return _fields[_mapper.GetColumnNo(10)]; }
        }

        public string cs_User_Agent
        {
            get { return _fields[_mapper.GetColumnNo(11)]; }
        }

        public string cs_Referer
        {
            get { return _fields[_mapper.GetColumnNo(12)]; }
        }

        public string sc_status
        {
            get { return _fields[_mapper.GetColumnNo(13)]; }
        }

        public string sc_substatus
        {
            get { return _fields[_mapper.GetColumnNo(14)]; }
        }

        public string sc_win32_status
        {
            get { return _fields[_mapper.GetColumnNo(15)]; }
        }

        public string sc_bytes
        {
            get { return _fields[_mapper.GetColumnNo(16)]; }
        }

        public string cs_bytes
        {
            get { return _fields[_mapper.GetColumnNo(17)]; }
        }

        public string time_taken
        {
            get { return _fields[_mapper.GetColumnNo(18)]; }
        }

        public string cs_version
        {
            get { return _fields[_mapper.GetColumnNo(19)]; }
        }

        public string cs_host
        {
            get { return _fields[_mapper.GetColumnNo(20)]; }
        }

        public string cs_Cookie
        {
            get { return _fields[_mapper.GetColumnNo(21)]; }
        }

    }

    public static class LinqToW3SVCLog
    {
        internal static readonly string[] masterHeaders = "date time s-sitename s-computername s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status sc-bytes cs-bytes time-taken cs-version cs-host cs(Cookie)"
            .Split(' ');
        internal static readonly IDictionary<string, int> masterDict = masterHeaders.ToPositionDictionary();

        /// <summary>
        /// Convert W3SVC log lines to W3SVCLogRecord objects
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<W3SVCLogRecord> AsW3SVCLogRecords(this IEnumerable<string> lines)
        {
            return lines.EnumRecords(
                (n, s) =>
                {
                    if (s.StartsWith("#Fields:"))
                    {
                        return Tuple.Create(LineType.Header, s.Substring(9).Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else if (s.StartsWith("#"))
                    {
                        return Tuple.Create(LineType.Comments, new string[] { s });
                    }
                    else
                    {
                        return Tuple.Create(LineType.Data, s.Split(' '));
                    }
                },
                headers => new W3SVCLogColumnMapper(headers),
                (mapper, fields) => new W3SVCLogRecord(mapper, fields)
                );
        }
    }
}
