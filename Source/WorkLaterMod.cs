using HugsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CardboardBread.WorkLater
{
    public class WorkLaterMod : ModBase
    {
        public override string ModIdentifier => GetType().Namespace;

        public WorkLaterMod()
        {
        }

        public override void DefsLoaded()
        {
            base.DefsLoaded();
            // Setup settings here.
        }
    }
}
