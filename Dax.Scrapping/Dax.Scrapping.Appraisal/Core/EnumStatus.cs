using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Scrapping.Appraisal.Core
{
    public enum  Status
    {
        [Description("Paused")]
        Paused = 0,
        [Description("loading login screen")]
        Loading,
        [Description("Loging user")]
        Loggin,
        [Description("Searching Conections")]
        Searching,
        [Description("Parsing Conections page")]
        Parsing,

        [Description("Getting Orders Info")]
        GetOrders,

        [Description("Completed")]
        Completed
    }
}
