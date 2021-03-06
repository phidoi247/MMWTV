﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Oqat.ViewModel;
using Oqat.PublicRessources.Model;
using Oqat.Model;
using System.Collections;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OQAT_Tests
{
    /// <summary>
    /// Class for testing the Welcome View.
    /// Missing some minor cases, like double click selection or localisation.
    /// </summary>
    [TestClass]
    public class WelcomeViewTest
    {
        /// <summary>
        /// Creates a new welcome view and tests whether the displayed
        /// project listbox matches the array of recently used projects.
        /// </summary>
        [TestMethod]
        public void constructorTest()
        {
            VM_Welcome_Accessor welcome = new VM_Welcome_Accessor();
            Assert.IsNotNull(welcome.projects);
            Assert.AreEqual(welcome.listBox1.Items.Count, welcome.projects.Count);
            welcome.projects.Clear();
            welcome.projects.Add("1");
            welcome.projects.Add("2");
            welcome.projects.Add("3");
            welcome.updateListBox();
            Assert.AreEqual("3", (string)welcome.listBox1.Items[0]);
            Assert.AreEqual("2", (string)welcome.listBox1.Items[1]);
            Assert.AreEqual("1", (string)welcome.listBox1.Items[2]);
        }


        /// <summary>
        /// Memento test
        /// </summary>
        [TestMethod]
        public void mementoTest()
        {
            VM_Welcome_Accessor welcome = new VM_Welcome_Accessor();
            ArrayList state = new ArrayList();
            for (int i = 0; i < 17; i++)
            {
                state.Add(i.ToString());
            }
            Memento mem = new Memento("testmem", state);
            welcome.setMemento(mem);
            ArrayList projectList = (ArrayList)welcome.getMemento().state;
            Assert.AreEqual(15, projectList.Count);
            // if project list contains more than 15 projects,
            // first in first out is done until project list contains 15
            for (int i = 0; i < 15; i++)
            {
                Assert.AreEqual((i+2).ToString(), projectList[i]);
            }
        }
    }
}
