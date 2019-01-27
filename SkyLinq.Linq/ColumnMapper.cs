using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
    public class ColumnMapper
    {
        protected readonly string[] _headers;
        protected readonly IDictionary<string, int> _headersDictionary;
        public ColumnMapper(string[] headers)
        {
            this._headers = headers;
            this._headersDictionary = headers.ToPositionDictionary();
        }

        public virtual int GetColumnNo(string header)
        {
            return _headersDictionary[header];
        }

        public virtual bool TryGetColumnNo(string header, out int columnNo)
        {
            return _headersDictionary.TryGetValue(header, out columnNo);
        }

        public virtual int GetColumnNo(int i)
        {
            return i;
        }

        public virtual bool ContainsColumn(string header)
        {
            return _headersDictionary.ContainsKey(header);
        }

        public virtual IEnumerable<string> Headers
        {
            get
            {
                return _headers;
            }
        }
    }
}