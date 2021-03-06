﻿using AForge;
using Oqat;
using Oqat.PublicRessources.Model;
using Oqat.PublicRessources.Plugin;
using System.Drawing;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using AForge.Imaging.Filters;
using AForge.Math.Random;
using System.Threading;
using System.Windows.Controls;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.ComponentModel;


namespace PF_NoiseGenerator
{
    
    [ExportMetadata("namePlugin", "NoiseGenerator")]
    [ExportMetadata("type", PluginType.IFilterOqat)]
    [ExportMetadata("threadSafe", false)]
    [Export(typeof(IPlugin))]
    [Serializable()]
    public class NoiseGenerator : IFilterOqat, INotifyPropertyChanged
    {


        private string _namePlugin = "NoiseGenerator";
        private PluginType _type = PluginType.IFilterOqat;
        VM_NoiseGenerator propertiesView;

        public bool threadSafe
        {
            get { return false; }
        }
        /// <summary>
        /// Generates the filtered Image.
        /// </summary>

        public Bitmap process(Bitmap frame)
        {
           
            IRandomNumberGenerator generator = new UniformGenerator(new Range(-1*noise, noise), System.DateTime.Now.Millisecond);
           
            AdditiveNoise filter = new AdditiveNoise(generator);
            Bitmap test = new Bitmap(frame.Width, frame.Height, PixelFormat.Format24bppRgb);

            for (int i = 0; i < frame.Width; i++)
            {
                for (int j = 0; j < frame.Height; j++)
                {
                    test.SetPixel(i, j, frame.GetPixel(i, j));
                }
            }

        /*    Graphics g = Graphics.FromImage(test);
            g.DrawImage(frame, 0, 0);
            g.Dispose();

            frame = test;*/
            filter.ApplyInPlace(test);
            return test;
        }

        public string namePlugin
        {
            get
            {
                return this._namePlugin;

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

        public UserControl propertyView
        {
            get
            {
                if (propertiesView == null)
                {
                        this.propertiesView = new VM_NoiseGenerator();
                        this.propertyView.DataContext = this;
                        localize(_namePlugin + "_" + Thread.CurrentThread.CurrentCulture + ".xml");   
                }
                return propertiesView;
            }
        }

        public Dictionary<EventType, List<Delegate>> getEventHandlers()
        {
            Dictionary<EventType, List<Delegate>> handlers = new Dictionary<EventType, List<Delegate>>();
            return handlers;
        }

        /// <summary>
        /// Returns a Memento with the current state of the Object.
        /// </summary>

    public Oqat.PublicRessources.Model.Memento getMemento()
        {
            Memento mem = new Memento(this.namePlugin, noise);

            return mem;
        }

    /// <summary>
    /// Sets a Memento as the current state of the Object.
    /// </summary>
        public void setMemento(Oqat.PublicRessources.Model.Memento memento)
        {
            
            Object obj = memento.state;
            noise = (float)obj;
        }

        private float _noise;
        public float noise
        {
            get {
                return _noise;
            }
            set {
                _noise = value;
                NotifyPropertyChanged("noise");
            }
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        } 

        /// <summary>
        /// Helper Method to localize the Object. String s is the name of the language file.
        /// </summary>
        private void localize(String s)
        {
            propertiesView.local(s);

        }

        public IPlugin createExtraPluginInstance()
        {
            return new NoiseGenerator();
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}

