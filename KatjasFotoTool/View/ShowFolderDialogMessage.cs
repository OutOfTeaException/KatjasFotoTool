using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KatjasFotoTool.View
{
    public class ShowFolderDialogMessage
    {
        public string Text { get; set; }
        public Action<string> ResultCallback { get; set; }

        public ShowFolderDialogMessage(string text, Action<String> resultCallback)
        {
            Text = text;
            ResultCallback = resultCallback;
        }
    }
}
