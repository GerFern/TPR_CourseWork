//#define test
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseLibrary;

namespace TPR_ExampleView
{
    internal class MyMethodInfo
    {
        public bool IsAutoForm { get; }
        public bool IsInputImage { get; }
        public bool CanBeDisposedOrNull { get; }
        public MethodInfo MethodInfo { get; }
        public string[] Hierarchy { get => ImgMethod.Hierarchy; }
        public CustomFormAttribute CustomForm { get; }
        public ControlFormAttribute[] ControlForms { get; }
        public ControlPropertyAttribute[] ControlProperties { get; }
        public Dictionary<int, List<ControlPropertyAttribute>> DictControlProperties { get; }
        public AutoFormAttribute[] AutoForms { get; }
        public TPRFormAttribute[] AutoAndControlForms { get; }
        public ImgMethodAttribute ImgMethod { get; }
        public Assembly Assembly { get => MethodInfo.Module.Assembly; }
        public Module Module { get => MethodInfo.Module; }
        public string MethodName { get; }
        public string FullName => $"{Module.Name}_{MethodName}";
        public MyMethodInfo(MethodInfo methodInfo, bool isInputImage)
        {
            MethodInfo = methodInfo;
            MethodNameAttribute name = methodInfo.GetCustomAttribute<MethodNameAttribute>();
            MethodName = name == null ? methodInfo.Name : name.Name;
            CanBeDisposedOrNull = methodInfo.GetCustomAttribute<ImgCanBeDisposedOrNullAttribute>() != null;
            ImgMethod = methodInfo.GetCustomAttribute<ImgMethodAttribute>();
            CustomForm = methodInfo.GetCustomAttribute<CustomFormAttribute>();
            AutoForms = methodInfo.GetCustomAttributes<AutoFormAttribute>().ToArray();
            ControlForms = methodInfo.GetCustomAttributes<ControlFormAttribute>().ToArray();
            ControlProperties = methodInfo.GetCustomAttributes<ControlPropertyAttribute>().ToArray();
            IsInputImage = isInputImage;
            IsAutoForm = AutoForms.Length > 0 || ControlForms.Length > 0;
            DictControlProperties = new Dictionary<int, List<ControlPropertyAttribute>>();
            if(IsAutoForm)
            {
                Type typeAutoForm = typeof(AutoFormAttribute);
                Type typeControlForm = typeof(ControlFormAttribute);
                AutoAndControlForms = new TPRFormAttribute[AutoForms.Length + ControlForms.Length];
                IEnumerator<CustomAttributeData> e = methodInfo.CustomAttributes.GetEnumerator();
                for (int i = 0; i < AutoAndControlForms.Length; i++)
                {
                    e.MoveNext();
                    while (!(e.Current.AttributeType == typeAutoForm || e.Current.AttributeType == typeControlForm))
                    {
                        e.MoveNext();
                    }
                    AutoAndControlForms[i] = (TPRFormAttribute)e.Current.Constructor.Invoke(e.Current.ConstructorArguments.Select(a=>a.Value).ToArray());
                }
                foreach (var item in ControlProperties)
                {
                    if(!DictControlProperties.ContainsKey(item.ParamIndex))
                        DictControlProperties.Add(item.ParamIndex, new List<ControlPropertyAttribute>());
                    DictControlProperties[item.ParamIndex].Add(item);
                }
            }
        }
    }
}
