using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
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
