using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.Utils
{
    interface ICancelDialog
    {
        void ShowUntilCompleteOrCancelled();
    }
}
