using System;
using System.Collections.Generic;
using System.Text;

namespace ClientLibrary
{
    class ClientLibraryException:Exception
    {
        public ClientLibraryException(String message) : base(message)
        {

        }
    }
}
