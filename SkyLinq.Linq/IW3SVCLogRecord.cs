using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyLinq.Linq
{
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
}