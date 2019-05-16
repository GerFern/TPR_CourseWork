using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    public static class Extensions
    {
        public const string bmpName = "Windows bitmaps";
        public const string jpgName = "JPEG files";
        public const string pngName = "Portable Network Graphics";
        public const string pifName = "Portable image format";
        public const string sunName = "Sun rasters";
        public const string tifName = "TIFF files";
        public const string bmpExt = ".BMP";
        public const string dibExt = ".DIB";
        public const string jpegExt = ".JPEG";
        public const string jpgExt = ".JPG";
        public const string jpeExt = ".JPE";
        public const string pngExt = ".PNG";
        public const string pbmExt = ".PBM";
        public const string pgmExt = ".PGM";
        public const string ppmExt = ".PPM";
        public const string srExt = ".SR";
        public const string rasExt = ".RAS";
        public const string tiffExt = ".TIFF";
        public const string tifExt = ".TIF";

        public static string GetFilterOpenFileDialog(bool includeAllFiles)
        {
            string str = string.Empty;
            var nameExts = NameToExtSupport;
            string[] allext = nameExts.SelectMany(a => a.Value).ToArray();
            nameExts.Add("Все изображения", allext);
            if(includeAllFiles)
                nameExts.Add("Все файлы", new string[] { ".*" });
            string[] vs;
            foreach (var item in nameExts.Take(nameExts.Count - 1))
            {
                str += $"{item.Key}|";
                vs = item.Value;
                foreach (var item2 in vs.Take(vs.Length - 1))
                {
                    str += $"*{item2};";
                }
                str += $"*{vs.Last()}|";
            }
            str += $"{nameExts.Last().Key}|";
            vs = nameExts.Last().Value;
            foreach (var item2 in vs.Take(vs.Length - 1))
            {
                str += $"*{item2};";
            }
            str += $"*{vs.Last()}";
            return str;
        }

        public static string GetFilterSaveFileDialog()
        {
            string str = string.Empty;
            var extNames = ExtToNameSupport;
            string s;
            foreach (var item in extNames.Take(extNames.Count - 1))
            {
                str += $"{item.Value}|";
                s = item.Key;
                str += $"*{s}|";
            }
            str += $"{extNames.Last().Value}|";
            s = extNames.Last().Key;
            str += $"*{s}";
            return str;
        }

        public static Dictionary<string, string[]> NameToExtSupport => new Dictionary<string, string[]>
        {
            { bmpName, new string[] { bmpExt , dibExt } },
            { jpgName, new string[] { jpegExt, jpgExt, jpeExt } },
            { pngName, new string[] { pngExt } },
            { pifName, new string[] { pbmExt, pgmExt, ppmExt } },
            { sunName, new string[] { srExt, rasExt } },
            { tifName, new string[] { tiffExt, tifExt } },
        };

        public static Dictionary<string, string> ExtToNameSupport => new Dictionary<string, string>()
        {
            { bmpExt,  bmpName  },
            { dibExt,  bmpName  },
            { jpegExt, jpgName },
            { jpgExt,  jpgName },
            { jpeExt,  jpgName },
            { pngExt,  pngName },
            { pbmExt,  pifName },
            { pgmExt,  pifName },
            { ppmExt,  pifName },
            { srExt,   sunName },
            { rasExt,  sunName },
            { tiffExt, tifName },
            { tifExt,  tifName },
        };
        /// <summary>
        /// Является ли данный путь ссылкой на изображение
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool PathIsImage(this string s) => ExtToNameSupport.ContainsKey(Path.GetExtension(s).ToUpper());
        /// <summary>
        /// Получить описание [<see cref="DescriptionAttribute"/>]. Если описание остутсвует, то вернет результат <typeparamref name="T"/>.ToString(). Подойдет для перечислений
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
        /// <summary>
        /// Является ли изображение уничтоженным или пустым
        /// </summary>
        /// <param name="image">Изображение, которое нужно проверить</param>
        /// <returns></returns>
        public static bool IsDisposedOrNull(this IImage image) => image == null || image.Ptr == IntPtr.Zero;

        /// <summary>
        /// Вызывает метод <see cref="Control.Invoke(Delegate, object[])"/> тогда, когда это нужно
        /// </summary>
        /// <param name="control"></param>
        /// <param name="delegate"></param>
        /// <param name="args"></param>
        public static void InvokeFix(this Control control, Action @delegate, params object[] args)
        {
            if (control.InvokeRequired)
                control.Invoke(@delegate, args);
            else @delegate.Method.Invoke(@delegate.Target, args);
        }

        //public static void InvokeFix(this Control control)

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);

        public enum ProgressBarState
        {
            Green = 1,
            Red,
            Yellow
        }
        public static void SetState(this ProgressBar pBar, ProgressBarState state, EventWaitHandle waitHandle, int value = -1)
        {
            new Thread(() =>
            {
                //waitHandle?.WaitOne();
                Thread.Sleep(500);
                pBar.InvokeFix(() =>
                {
                    if (value >= 0)
                        pBar.Value = value;
                    SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
                });
                pBar.Invalidate();
                waitHandle?.Set();
            })
            { Name = $"StateChangeTo{state.ToString()}" }.Start();
        }

        public static bool LoadSettings<T>(this T sender, string ID)
        {
            Type typeAttribute = typeof(SaveParamAttribute);
            Type type = sender.GetType();
            const string paramsdir = "Params";
            if (!Directory.Exists(paramsdir))
                Directory.CreateDirectory(paramsdir);
            string path = $"{paramsdir}\\{type.Module.Name}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += $"\\{ID}.json";
            //Если файл существует
            if (File.Exists(path))
            {
                //Список свойств
                var properties = type.GetProperties().Where(a => a.GetCustomAttributes(typeAttribute, false).Length != 0);
                //Список полей
                var fields = type.GetFields().Where(a => a.GetCustomAttributes(typeAttribute, false).Length != 0);
                Dictionary<string, string> dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
                foreach (var item in dict)
                {
                    object nVal;
                    var property = properties.FirstOrDefault(a => a.Name == item.Key);
                    if (property != null)
                    {
                        //Если свойство найдено
                        //Конвертация строки в правильный тип
                        nVal = ((IConvertible)item.Value).ToType(property.PropertyType, System.Globalization.CultureInfo.CurrentCulture);
                        //Установить свойство для sender
                        property.SetValue(sender, nVal);
                    }
                    else
                    {
                        //Если поле найдено
                        //Конвертация строки в правильный тип
                        var field = fields.FirstOrDefault(a => a.Name == item.Key);
                        if (field != null)
                        {
                            nVal = ((IConvertible)item.Value).ToType(field.FieldType, System.Globalization.CultureInfo.CurrentCulture);
                            //Установить поле для sender
                            field.SetValue(sender, nVal);
                        }
                    }
                }
                return true;
            }
            else return false;

        }

        public static void SaveSetting<T>(this T sender, string ID)
        {
            Type typeAttribute = typeof(SaveParamAttribute);
            Type type = sender.GetType();
            const string paramsdir = "Params";
            if (!Directory.Exists(paramsdir))
                Directory.CreateDirectory(paramsdir);
            string path = $"{paramsdir}\\{type.Module.Name}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += $"\\{ID}.json";

            Dictionary<string, string> dict = new Dictionary<string, string>();

            //Список свойств
            var properties = type.GetProperties().Where(a => a.GetCustomAttributes(typeAttribute, false).Length != 0);
            //Список полей
            var fields = type.GetFields().Where(a => a.GetCustomAttributes(typeAttribute, false).Length != 0);

            foreach (var item in properties)
            {
                string value = Convert.ToString(item.GetValue(sender));
                dict.Add(item.Name, value);
            }

            foreach (var item in fields)
            {
                string value = Convert.ToString(item.GetValue(sender));
                dict.Add(item.Name, value);
            }

            File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(dict));
        }

    }
}
