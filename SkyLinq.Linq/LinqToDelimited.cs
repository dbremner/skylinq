using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
    public enum LineType
    {
        Comments,
        Header,
        Data
    }

    public class ColumnMapper
    {
        protected string[] _headers;
        protected IDictionary<string, int> _headersDictionary;
        public ColumnMapper(string[] headers)
        {
            this._headers = headers;
            this._headersDictionary = headers.ToPositionDictionary();
        }

        public virtual int GetColumnNo(string header)
        {
            return _headersDictionary[header];
        }

        public virtual int GetColumnNo(int i)
        {
            return i;
        }
    }

    public class Record
    {
        protected ColumnMapper _mapper;
        protected string[] _fields;

        public Record(ColumnMapper mapper, string[] fields)
        {
            _mapper = mapper;
            _fields = fields;
        }

        public string this[int i]
        {
            get { return _fields[_mapper.GetColumnNo(i)]; }
        }

        public string this[string header]
        {
            get { return _fields[_mapper.GetColumnNo(header)]; }
        }
    }

    public static class LinqToDelimited
    {
        /// <summary>
        /// Split line into fields. The first line contains the column header.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static IEnumerable<Record> EnumRecords(this IEnumerable<string> lines, char delimiter)
        {
            return lines.EnumRecords(
                (i, s) =>
                {
                    if (i == 1)
                        return new Tuple<LineType,string[]>(LineType.Header, s.Split(delimiter));
                    else
                        return new Tuple<LineType,string[]>(LineType.Data, s.Split(delimiter));
                });
        }

        /// <summary>
        /// Split line into fields. Headers are supplied externally.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="headers"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static IEnumerable<Record> EnumRecords(this IEnumerable<string> lines, string[] headers, char delimiter)
        {
            return lines.EnumRecords(headers, delimiter,
                hds => new ColumnMapper(hds),
                (mapper, fields) => new Record(mapper, fields)
                );
        }

        /// <summary>
        /// Split line into fields. LineParser function is responsible for parsing the line and extract the header
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineParser">The line parser function returns a Duple of line type and string array of fiields. The line type 
        /// could be Comment, Header or Data.</param>
        /// <returns></returns>
        public static IEnumerable<Record> EnumRecords(this IEnumerable<string> lines, Func<int, string, Tuple<LineType, string[]>> lineParser)
        {
            return lines.EnumRecords(lineParser,
                headers => new ColumnMapper(headers),
                (mapper, fields) => new Record(mapper, fields));
        }

        /// <summary>
        /// Split line into fields. LineParser function is responsible for parsing the line and extract the header. MapperFactory
        /// is responsible to generate the mapper object. RecordFactory is responsible to generate the mapper object.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineParser"></param>
        /// <param name="mapperFactory"></param>
        /// <param name="recordFactory"></param>
        /// <returns></returns>
        public static IEnumerable<TRecord> EnumRecords<TMapper, TRecord>(this IEnumerable<string> lines, Func<int, string, Tuple<LineType, string[]>> lineParser,
            Func<string[], TMapper> mapperFactory, Func<TMapper, string[], TRecord> recordFactory) where TMapper:ColumnMapper where TRecord : Record
        {
            TMapper mapper = null;
            int lineNo = 0;
            foreach (string line in lines)
            {
                lineNo++;
                var results = lineParser(lineNo, line);
                switch (results.Item1)
                {
                    case LineType.Data:
                        yield return recordFactory(mapper, results.Item2);
                        break;
                    case LineType.Header:
                        mapper = mapperFactory(results.Item2);
                        break;
                    case LineType.Comments:
                        break;
                }
            }
        }

        /// <summary>
        /// Split line into fields. The headers are supplied externally. MapperFactory
        /// is responsible to generate the mapper object. RecordFactory is responsible to generate the mapper object.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="headers"></param>
        /// <param name="delimiter"></param>
        /// <param name="mapperFactory"></param>
        /// <param name="recordFactory"></param>
        /// <returns></returns>
        public static IEnumerable<TRecord> EnumRecords<TMapper, TRecord>(this IEnumerable<string> lines, string[] headers, char delimiter,
    Func<string[], TMapper> mapperFactory, Func<TMapper, string[], TRecord> recordFactory)
            where TMapper : ColumnMapper
            where TRecord : Record
        {
            TMapper mapper = mapperFactory(headers);
            foreach (string line in lines)
            {
                yield return recordFactory(mapper, line.Split(delimiter));
            }
        }
    }
}
