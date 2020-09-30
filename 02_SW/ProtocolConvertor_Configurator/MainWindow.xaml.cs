using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProtocolConvertor_Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public enum ProtocolsList { CAN, I2C, LIN, RS232, UNKNOWN = 4 };
    public partial class MainWindow : Window
    {
        private static List<byte> configurationString = new List<byte>();

        private static byte Protocol_CheckEnable(CheckBox c)
        {
            byte result = (byte)0x00;
            if (c.IsChecked == true)
            {
                result = (byte)0x01;
            }
            return result;
        }

        private static void Components_Display(int protocolID, Visibility visibility)
        {
            switch (protocolID)
            {
                case 0: // CAN

                    break;
            }
        }

        private static ProtocolsList getProtocolFromID(object id)
        {
            switch (id)
            {
                case "CAN":
                    return ProtocolsList.CAN;
                case "I2C":
                    return ProtocolsList.I2C;
                case "LIN":
                    return ProtocolsList.LIN;
                case "RS232":
                    return ProtocolsList.RS232;
                default:
                    return ProtocolsList.UNKNOWN;
            }
        }

        ObservableCollection<LookUpTable_Element> LKTS_list = new ObservableCollection<LookUpTable_Element>();

        private void Grid_LINSpecificVisibility(bool state)
        {
            Thickness newThickness;
            if (true == state)
            {
                Grid_LINSpecific.Visibility = Visibility.Visible;
                /* Get current margin */
                newThickness = Grid_MovableItems_LKTS_Area.Margin;
                /* Shift to the right with 205 */
                newThickness.Left = 315;
                /* Set new margin */
                Grid_MovableItems_LKTS_Area.Margin = newThickness;
                DataGrid_PIDBytes.Visibility = Visibility.Visible;
                DataGrid_PIDType.Visibility = Visibility.Visible;
            }
            else
            {
                Grid_LINSpecific.Visibility = Visibility.Hidden;
                /* Get current margin */
                newThickness = Grid_MovableItems_LKTS_Area.Margin;
                /* Shift to the right with 205 */
                newThickness.Left = 110;
                /* Set new margin */
                Grid_MovableItems_LKTS_Area.Margin = newThickness;
                DataGrid_PIDBytes.Visibility = Visibility.Hidden;
                DataGrid_PIDType.Visibility = Visibility.Hidden;
            }
        }

        private void Grid_SetVisibility()
        {
            Visibility v = Visibility.Hidden;
            for (int i = 0; i < ListBox_ProtocolsList.Items.Count; i++)
            {
                if (ListBox_ProtocolsList.SelectedIndex >= 0)
                {
                    Grid_Content.Visibility = Visibility.Visible;
                    if (i == ListBox_ProtocolsList.SelectedIndex)
                    {
                        v = Visibility.Visible;
                    }
                    else
                    {
                        v = Visibility.Hidden;
                    }
                }
                else
                {
                    Grid_Content.Visibility = Visibility.Hidden; ;
                }
                switch (i)
                {
                    case 0:
                        Grid_ProtocolContent_CAN.Visibility = v;
                        TextArea_SourceAddress.IsEnabled = true;
                        break;
                    case 1:
                        Grid_ProtocolContent_I2C.Visibility = v;
                        /* I2C Slave was selected */
                        if ((RadioBtn_I2C_Master.IsChecked == false) && (RadioBtn_I2C_Slave.IsChecked == true))
                        {
                            Grid_I2C_Slave.Visibility = Visibility.Visible;
                            Grid_I2C_Master.Visibility = Visibility.Hidden;
                            TextArea_SourceAddress.IsEnabled = false;
                        }
                        /* I2C Master was selected */
                        else if ((RadioBtn_I2C_Master.IsChecked == true) && (RadioBtn_I2C_Slave.IsChecked == false))
                        {
                            Grid_I2C_Slave.Visibility = Visibility.Hidden;
                            Grid_I2C_Master.Visibility = Visibility.Visible;
                            TextArea_SourceAddress.IsEnabled = true;
                        }
                        break;
                    case 2:
                        Grid_ProtocolContent_LIN.Visibility = v;
                        if (i == ListBox_ProtocolsList.SelectedIndex)
                        {
                            Grid_LINSpecificVisibility(true);
                        }
                        else
                        {
                            Grid_LINSpecificVisibility(false);
                        }
                        break;
                    case 3:
                        Grid_ProtocolContent_RS232.Visibility = v;
                        if (i == ListBox_ProtocolsList.SelectedIndex)
                        {
                            TextArea_SourceAddress.IsEnabled = false;
                            TextArea_SourceAddress.Text = "0";
                        }
                        else
                        {
                            TextArea_SourceAddress.IsEnabled = true;
                            TextArea_SourceAddress.Text = "";
                        }
                        break;
                }
            }

        }

        public MainWindow()
        {
            InitializeComponent();
            Grid_SetVisibility();
            
            DataGrid_LKTS.DataContext = LKTS_list;
        }

        private void LKTS_Insert_Area_Init(int selectedIndex)
        {
            LKTS_list.Clear();
            ComboBox_DestinationProtocol.Items.Clear();
            ComboBox_DestinationProtocol.Items.Add(new ComboBoxItem().Name = "CAN");
            ComboBox_DestinationProtocol.Items.Add(new ComboBoxItem().Name = "I2C");
            ComboBox_DestinationProtocol.Items.Add(new ComboBoxItem().Name = "LIN");
            ComboBox_DestinationProtocol.Items.Add(new ComboBoxItem().Name = "RS232");
            ComboBox_DestinationProtocol.Items.RemoveAt(selectedIndex);
        }

        private void ListBox_ProtocolsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedProtocol_ID = ListBox_ProtocolsList.SelectedIndex;
            Label_ConfigurationProtocolName.Content = Label_ConfigurationProtocolName.Content.ToString().Remove(0, Label_ConfigurationProtocolName.Content.ToString().IndexOf(' '));
            Label_ConfigurationProtocolName.Content = Label_ConfigurationProtocolName.Content.ToString().Insert(0, ((ListBoxItem)ListBox_ProtocolsList.SelectedItem).Content.ToString());
            LKTS_Insert_Area_Init(selectedProtocol_ID);
            Grid_SetVisibility();

        }

        private void Button_Insert_LKT_Click(object sender, RoutedEventArgs e)
        {

            if ((ComboBox_DestinationProtocol.SelectedIndex >= 0) && (TextArea_DestinationAddress.Text.Length > 0) && (TextArea_DestinationAddress.Text.Length > 0))
            {
                ProtocolsList destinationProtocol = getProtocolFromID(ComboBox_DestinationProtocol.SelectedItem);
                if (ListBox_ProtocolsList.SelectedIndex == 2)
                {
                    LKTS_list.Add(new LookUpTable_Element(int.Parse(TextArea_SourceAddress.Text), int.Parse(TextArea_DestinationAddress.Text), destinationProtocol, Convert.ToByte(ComboBox_LIN_PIDType.SelectedIndex), byte.Parse(TextArea_LIN_BytesOnPID.Text)));
                }
                else
                {
                    LKTS_list.Add(new LookUpTable_Element(int.Parse(TextArea_SourceAddress.Text), int.Parse(TextArea_DestinationAddress.Text), destinationProtocol));
                }
            }
        }
        private void TextArea_SourceAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox source = (TextBox)sender;
            Regex regex = new Regex("[^0-9]+");
            if (regex.IsMatch(source.Text) == true)
            {
                if (source.Text.Length > 0)
                {
                    source.Text = regex.Replace(source.Text, "");
                    source.CaretIndex = source.Text.Length;
                }
            }
            if (source == TextArea_I2C_SourceAddress)
            {
                TextArea_SourceAddress.Text = source.Text;
            }
        }

        private void RadioBtn_I2C_Operation_Click(object sender, RoutedEventArgs e)
        {
            Grid_SetVisibility();
        }

        private void ComboBox_DestinationProtocol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox source = (ComboBox)sender;
            if (source.SelectedItem != null)
            {
                if (source.SelectedItem.Equals("RS232"))
                {
                    TextArea_DestinationAddress.Text = "0";
                    TextArea_DestinationAddress.IsEnabled = false;
                }
                else
                {
                    TextArea_DestinationAddress.Text = "";
                    TextArea_DestinationAddress.IsEnabled = true;
                }
            }
        }

        private void Configuration_AddProtocolSpecificBytes(ref List<byte> configurationString)
        {
            byte byte3 = 0;
            byte byte4 = 0;
            switch (ListBox_ProtocolsList.SelectedIndex)
            {
                case (int)ProtocolsList.CAN:
                    configurationString.Add((byte)ComboBox_CAN_Baud.SelectedIndex);
                    break;
                case (int)ProtocolsList.I2C:
                    /* I2C Slave was selected */
                    if ((RadioBtn_I2C_Master.IsChecked == false) && (RadioBtn_I2C_Slave.IsChecked == true))
                    {
                        byte3 = Convert.ToByte(TextArea_I2C_SourceAddress.Text);
                    }
                    /* I2C Master was selected */
                    else if ((RadioBtn_I2C_Master.IsChecked == true) && (RadioBtn_I2C_Slave.IsChecked == false))
                    {
                        /* Set MSb */
                        byte3 = 0x80;
                        byte3 = Convert.ToByte(byte3 | ComboBox_I2C_Baud.SelectedIndex);
                    }
                    configurationString.Add(byte3);
                    break;
                case (int)ProtocolsList.LIN:
                    /* Byte 3:
                    LSB Half – 0xB LIN Slave
                    -	0xC LIN Master
                    MSB Half – Baud Rate
                   */
                    byte3 = Convert.ToByte(ComboBox_LIN_Mode.SelectedIndex + 0x0B);
                    byte3 |= Convert.ToByte(ComboBox_LIN_Baudrate.SelectedIndex << 4);
                    /*Byte 4:
	                    [1:0] StopBit */
                    /*  [2] CheckSUm Mode */
                    /*  [3] Polarity */
                    /*  [4] Baud Gen Speed */
                    byte4 = Convert.ToByte(ComboBox_LIN_StopBit.SelectedIndex);
                    byte4 |= Convert.ToByte(ComboBox_LIN_Checksum.SelectedIndex << 2);
                    byte4 |= Convert.ToByte(ComboBox_LIN_Polarity.SelectedIndex << 3);
                    byte4 |= Convert.ToByte(ComboBox_LIN_SpeedMode.SelectedIndex << 4);
                    configurationString.Add(byte3);
                    configurationString.Add(byte4);
                    break;
                case (int)ProtocolsList.RS232:
                    /*  bits[6:3] communicationDesiredBaud
                        bits[2:0] communicationUartMode     */
                    byte3 = Convert.ToByte(ComboBox_RS232_Mode.SelectedIndex | (ComboBox_RS232_Baudrate.SelectedIndex << 3));
                    /* [1:0] StopBit */
                    /* [3:2] HSFC */
                    /* [4] Polarity */
                    /* [5] Baud Gen Speed */
                    byte4 = Convert.ToByte(ComboBox_RS232_StopBit.SelectedIndex | (ComboBox_RS232_HFC.SelectedIndex << 2) | (ComboBox_RS232_Polarity.SelectedIndex << 4) | (ComboBox_RS232_Polarity.SelectedIndex << 5));
                    configurationString.Add(byte3);
                    configurationString.Add(byte4);
                    break;
            }
        }

        private void Button_SendCfg_Click(object sender, RoutedEventArgs e)
        {
            byte temp;
            configurationString.Clear();
            /* START BYTE + PROTOCOL ID + ENABLE  */
            configurationString.Add((byte)((byte)0xF8 | (((byte)ListBox_ProtocolsList.SelectedIndex << 1)) | Protocol_CheckEnable(CheckBox_ProtocolEnable)));

            /* Number of LKTS to be sent */
            configurationString.Add((byte)LKTS_list.Count);

            /* Adding protocol specific bytes */
            Configuration_AddProtocolSpecificBytes(ref configurationString);

            foreach (LookUpTable_Element lkt in LKTS_list)
            {
                /* Byte 1 - LSB Receiver */
                configurationString.Add((byte)(lkt.sourceAddress & 0xFF));
                if (ListBox_ProtocolsList.SelectedIndex == 2) //LIN
                {
                    temp = lkt.LIN_PIDBytes;
                    temp |= Convert.ToByte(lkt.LIN_PIDTypeValue << 4);
                    /* Byte 2 – MSB Receiver */
                    configurationString.Add(temp);
                }
                else // Other protocols
                {
                    /* Byte 2 – MSB Receiver */
                    configurationString.Add((byte)(lkt.sourceAddress >> 0x08));
                }
                /* Byte 3 – LSB Destination */
                configurationString.Add((byte)(lkt.destinationAddress & 0xFF));
                /* Byte 4 – MSB Destination */
                configurationString.Add((byte)(lkt.destinationAddress >> 0x08));
                /* Byte 5 – Target protocol (bit [1:0]) + 0xA8 if retransmission is needed or 0x58 if end of configuration. */
                /* Todo: To add a function that knows if we have to add 0xA8 or 0x58 instead of configurationStatus */

                configurationString.Add(Convert.ToByte(Convert.ToByte(lkt.destinationProtocol)));
            }
            byte configurationStatus = (CheckBox_LastCfg.IsChecked == true) ? (byte)0x58 : (byte)0x00;


            configurationString[configurationString.Count - 1] = Convert.ToByte(configurationString[configurationString.Count - 1] | configurationStatus);
            e.Handled = true;
            FTDI_FT4222.SingleSend(ref configurationString);
        }
    }
}
