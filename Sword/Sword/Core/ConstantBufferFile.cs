using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSMT_Core;

namespace SSMT
{
    public class ConstantBufferFile
    {



        public ConstantBufferFile(string ConstantBufferFilePath)
        {
            //byte[] CSCB0BufData = File.ReadAllBytes(ConstantBufferFilePath);

            List<UInt32> CSCB0List = DBMTBinaryUtils.ReadAsR32_UINT(ConstantBufferFilePath);

        }

    }
}
