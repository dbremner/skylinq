using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
    public class Record : IReadOnlyDictionary<string, string>
    {
        protected readonly ColumnMapper _mapper;
        protected readonly string[] _fields;

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

        public bool ContainsKey(string key)
        {
            return _mapper.ContainsColumn(key);
        }

        public IEnumerable<string> Keys
        {
            get { return _mapper.Headers; }
        }

        public bool TryGetValue(string key, out string value)
        {
            if (!_mapper.TryGetColumnNo(key, out int columnNo))
            {
                value = null;
                return false;
            }

            value = _fields[columnNo];
            return true;
        }

        public IEnumerable<string> Values
        {
            get { return _fields; }
        }

        public int Count
        {
            get { return _fields.Length; }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _mapper.Headers.Zip(_fields, (h, v) => new KeyValuePair<string, string>(h, v)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}