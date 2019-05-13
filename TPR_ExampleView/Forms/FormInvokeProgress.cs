using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseLibrary;
using System.Threading;

namespace TPR_ExampleView.Forms
{
    public partial class FormInvokeProgress : Form
    {
        public FormInvokeProgress()
        {
            InitializeComponent();
        }

        bool AutoStart { get; }
        IEnumerator<ProgressInfoControl> Enumerator { get; set; }
        ProgressInfoControl curPic;
        int active = 0;
        internal FormInvokeProgress(bool autoStart, MenuMethod.InvParam invParam, params ImgName[] imgs) : this()
        {
            AutoStart = autoStart;
            foreach (var item in imgs)
            {
                var localInvParam = (MenuMethod.InvParam)invParam.Clone();
                if (item.Image.IsDisposedOrNull())
                {
                    localInvParam.ImgFileString = item.ImgPath;
                }
                else
                {
                    localInvParam.Image = item.Image;
                }
                ProgressInfoControl pic;
                localInvParam.TaskID = Program.mainForm.CreateTask(
                    item.Name,
                    invParam.MethodInfo,
                    new Thread(new ParameterizedThreadStart(MenuMethod.InvMethod)) { Name = item.Name },
                    localInvParam,
                    out pic);
                pic.ThreadStarted += new EventHandler((o, e) => this.InvokeFix(() => active++));
                pic.ThreadFinished += new EventHandler((o, e) => this.InvokeFix(() => { active--; Next(); }));
                plc.Add(pic);

                //if(item.Image.IsDisposedOrNull()) 
            }
            
            Enumerator = plc.Items.GetEnumerator();

            if (AutoStart)
            {
                Next();
            }
        }

        private void Next()
        {
            if (Enumerator != null)
            {
                if (numericUpDown1.Value > active)
                {
                    if (Enumerator.MoveNext())
                    {
                        curPic = Enumerator.Current;
                        //if (curPic.Started)
                        //{
                            
                        //}
                        //else
                        //{
                        //    curPic.ThreadStart();
                        //}
                        curPic.ThreadStart();
                        Next();
                    }
                    else Enumerator = null;
                }
            }
        }

        public struct ImgName
        {
            public ImgName(string imgPath, IImage image, string name)
            {
                ImgPath = imgPath;
                Image = image;
                Name = name;
            }

            public string ImgPath { get; set; }
            public IImage Image { get; set; }
            public string Name { get; set; }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Next();
        }
    }
}
