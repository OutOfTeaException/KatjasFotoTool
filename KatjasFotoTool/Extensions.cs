using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KatjasFotoTool
{
    public static class Extensions
    {
        public static DateTime Min(DateTime dateTime1, DateTime dateTime2)
        {
            if (dateTime1 > dateTime2)
                return dateTime2;
            else
                return dateTime1;
        }
    }
}
