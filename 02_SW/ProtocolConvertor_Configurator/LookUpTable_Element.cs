using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolConvertor_Configurator
{
    public enum LIN_PIDTypes { Publisher, Subscriber };
    public class LookUpTable_Element
    {
        public int sourceAddress { get; set; }
        public int destinationAddress { get; set; }
        public ProtocolsList destinationProtocol { get; set; }
        public String destinationProtocolText { get; set; }

        public byte LIN_PIDTypeValue { get; set; }
        public String LIN_PIDTypeText { get; set; }
        public byte LIN_PIDBytes { get; set; }

        public LookUpTable_Element(int sa, int da, ProtocolsList dp)
        {
            sourceAddress = sa;
            destinationAddress = da;
            destinationProtocol = dp;
            switch (destinationProtocol)
            {
                case ProtocolsList.CAN:
                    destinationProtocolText = "CAN";
                    break;
                case ProtocolsList.I2C:
                    destinationProtocolText = "I2C";
                    break;
                case ProtocolsList.LIN:
                    destinationProtocolText = "LIN";
                    break;
                case ProtocolsList.RS232:
                    destinationProtocolText = "RS232";
                    break;
            }
        }

        public LookUpTable_Element(int sa, int da, ProtocolsList dp, byte PIDType, byte PIDBytes)
        {
            sourceAddress = sa;
            destinationAddress = da;
            destinationProtocol = dp;
            LIN_PIDTypeValue = PIDType;
            switch (LIN_PIDTypeValue)
            {
                case 0:
                    LIN_PIDTypeText = "Publisher";
                    break;
                case 1:
                    LIN_PIDTypeText = "Subscriber";
                    break;
            }
                    LIN_PIDBytes = PIDBytes;
            switch (destinationProtocol)
            {
                case ProtocolsList.CAN:
                    destinationProtocolText = "CAN";
                    break;
                case ProtocolsList.I2C:
                    destinationProtocolText = "I2C";
                    break;
                case ProtocolsList.LIN:
                    destinationProtocolText = "LIN";
                    break;
                case ProtocolsList.RS232:
                    destinationProtocolText = "RS232";
                    break;
            }
        }
    }
}
