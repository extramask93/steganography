using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steg_2.lib
{
    interface IBmpHider
    {
        unsafe  byte[] get_message(Bitmap bm);
        unsafe Bitmap embed_message(Bitmap bm, byte[] message);
    }
}
