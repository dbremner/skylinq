using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
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
}