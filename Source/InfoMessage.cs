using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICities;
using ColossalFramework;
using UnityEngine;

namespace CitiesConext.Source
{
    public class InfoMessage : MessageBase
    {
        private string m_author;
        private string m_text;

        public InfoMessage(string author, string text)
        {
            this.m_author = author;
            this.m_text = text;
        }

        public override string GetSenderName()
        {
            return this.m_author;
        }
        public override string GetText()
        {
            return this.m_text;
        }
    }
}
