using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Number_Renamer.Models
{
    class FileModel
    {      
            private string _name;
            private string _filepath;

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                }
            }
            public string FilePath
            {
                get => _filepath;
                set
                {
                    _filepath = value;
                }
            }
        
    }
}
