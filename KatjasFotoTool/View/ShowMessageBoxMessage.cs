using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KatjasFotoTool.View
{
    public class ShowMessageBoxMessage
    {
        public String Text { get; private set; }
        public bool IsError { get; private set; }

        public ShowMessageBoxMessage(string text, bool isError)
        {
            Text = text;
            IsError = isError;
        }
        
        public ShowMessageBoxMessage(string text) : this(text, false)
        { 
        }
    }
}
