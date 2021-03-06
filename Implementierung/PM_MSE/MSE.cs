﻿using Oqat.PublicRessources.Plugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel.Composition;
using Oqat.PublicRessources.Model;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;

namespace PM_MSE
{
    
    [ExportMetadata("namePlugin", "MSE")]
    [ExportMetadata("type", PluginType.IMetricOqat)]
    [ExportMetadata("threadSafe", false)]
    [Export(typeof(IPlugin))]
    [Serializable()]
    public class MSE : IMetricOqat, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


        private string _namePlugin = "MSE";
        private PluginType _type = PluginType.IMetricOqat;
        public double sum;
        public double sumR;
        VM_PM_MSE propertiesView;

        int _radioButton = 0;
        public int radioButton
        {
            get
            {
                return _radioButton;
            }
            set
            {
                if (value > -1 && value < 4)
                {
                    _radioButton = value;
                    NotifyPropertyChanged("radioButton");
                }
            }
        }

        /// <summary>
        /// Method to generate the analysis data.
        /// </summary>
        public AnalysisInfo analyse(System.Drawing.Bitmap frameRef, System.Drawing.Bitmap frameProc)
        {
            AnalysisInfo analyse = null;
            if (frameRef != null && frameProc != null)
            {
                Bitmap resultFrame = new Bitmap(frameRef.Width, frameRef.Height);
                double sum = 0;
                double sumR = 0;
                double sumG = 0;
                double sumB = 0;

                //int rb = (propertiesView.getRb());
                float[] resultValues = new float[4];

                for (int i = 0; i < frameRef.Height - 1; i++)
                {
                    for (int j = 0; j < frameRef.Width - 1; j++)
                    {
                        int newPixel = 0;



                        //Get Color from Proc
                        Color colorProc = frameProc.GetPixel(j, i);
                        int alphaProc = colorProc.A;
                        int redProc = colorProc.R;
                        int greenProc = colorProc.G;
                        int blueProc = colorProc.B;

                        //Get Color from Ref
                        Color colorRef = frameRef.GetPixel(j, i);
                        int alphaRef = colorRef.A;
                        int rotRef = colorRef.R;
                        int grunRef = colorRef.G;
                        int blauRef = colorRef.B;

                        //RGB
                        int newRed = (int)Math.Pow((redProc - rotRef), 2);
                        int newGreen = (int)Math.Pow((greenProc - grunRef), 2);
                        int newBlue = (int)Math.Pow((blueProc - blauRef), 2);

                        sum += newBlue / 3 + newGreen / 3 + newRed / 3;
                        sumR += newRed;
                        sumG += newGreen;
                        sumB += newBlue;

                        switch (radioButton)
                        {
                            case 0:
                                newPixel = (((alphaProc + alphaRef) / 2) << 24) | (newRed << 16) | (newGreen << 8) | newBlue;
                                break;

                            case 1:
                                newPixel = (((alphaProc + alphaRef) / 2) << 24) | (newRed << 16) | (0 << 8) | 0;
                                break;

                            case 2:
                                newPixel = (((alphaProc + alphaRef) / 2) << 24) | (0 << 16) | (newGreen << 8) | 0;
                                break;

                            case 3:
                                newPixel = (((alphaProc + alphaRef) / 2) << 24) | (0 << 16) | (0 << 8) | newBlue;
                                break;
                        }
                        resultFrame.SetPixel(j, i, Color.FromArgb(newPixel));
                    }
                }
                resultValues[0] = (float)sum / (frameProc.Height * frameProc.Width);
                resultValues[1] = (float)sumR / (frameProc.Height * frameProc.Width);
                resultValues[2] = (float)sumG / (frameProc.Height * frameProc.Width);
                resultValues[3] = (float)sumB / (frameProc.Height * frameProc.Width);
                analyse = new AnalysisInfo(resultFrame, resultValues);
            }
            return analyse;
        }

        public string namePlugin
        {
            get
            {
                return _namePlugin;
            }
            set
            {
                this._namePlugin = value;
            }
        }

        public PluginType type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MSE()
        {
            
        }

        public UserControl propertyView
        {
            get
            {
                if (propertiesView == null)
                {
                    propertiesView = new VM_PM_MSE();
                    propertiesView.DataContext = this;

                    localize(_namePlugin + "_" + Thread.CurrentThread.CurrentCulture + ".xml");
                }
                return this.propertiesView;
            }
        }

        public Dictionary<EventType, List<Delegate>> getEventHandlers()
        {
            Dictionary<EventType, List<Delegate>> handlers = new Dictionary<EventType, List<Delegate>>();
            return handlers;
        }

        /// <summary>
        /// Return a Memento with the current state of this.
        /// </summary>
        public Oqat.PublicRessources.Model.Memento getMemento()
        {
            Memento mem = new Memento(this.namePlugin, radioButton);
            return mem;
        }

        /// <summary>
        /// Sets the memento to the current state of this.
        /// </summary>
        public void setMemento(Oqat.PublicRessources.Model.Memento memento)
        {
            if (memento == null)
                throw new ArgumentNullException("Given memento is null.");

            if (memento.state!=null && memento.state is int)
            {
                Object obj = memento.state;
                radioButton = (int)obj;
            }
        }

        /// <summary>
        /// Helper method to fit the language. String s is the Name of the Language file.
        /// </summary>
        private void localize(String s)
        {
            if(propertiesView != null)
                propertiesView.local(s);
        }

        public IPlugin createExtraPluginInstance()
        {
            return new MSE();
        }
        public bool threadSafe
        {
            get { return false; }
        }
    }
}

