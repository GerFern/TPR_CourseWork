using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    public partial class EnumComboBox<TEnum> : ComboBox where TEnum : Enum
    {
        public struct EnumName
        {
            public object EnumValue { get; }
            public string Name { get; }
            public EnumName(object @enum)
            {
                EnumValue = @enum;
                Name = @enum.DescriptionAttr();
            }
        }
        public EnumComboBox() : base()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            Array t1 = GetType().GetGenericArguments()[0].GetEnumValues();
            EnumName[] t2 = new EnumName[t1.Length];
            for (int i = 0; i < t1.Length; i++)
            {
                t2[i] = new EnumName(t1.GetValue(i));
            }
            DataSource = t2;
            DisplayMember = "Name";
            ValueMember = "EnumValue";
            //_typeEnum = value;
        }

        public TEnum EnumValue
        {
            get => (TEnum)SelectedValue;
            set => SelectedValue = value;
        }

        //Type _typeEnum;

        //public Type TypeEnum
        //{
        //    get => _typeEnum;
        //    set
        //    {
        //        if(value.IsEnum)
        //        {
        //            Enum[] t1 = (Enum[])value.GetEnumValues();
        //            EnumName[] t2 = new EnumName[t1.Length];
        //            for (int i = 0; i < t1.Length; i++)
        //            {
        //                t2[i] = new EnumName(t1[i]);
        //            }
        //            DataSource = t2;
        //            DisplayMember = "Name";
        //            ValueMember = "EnumValue";
        //            _typeEnum = value;
        //        }
        //    }
        //}
    }
}
