﻿using System;
using System.Collections.Generic;
using SS13_Shared;
using SS13_Shared.GO;
using System.Xml.Linq;

namespace CGO
{
    public class ContextMenuComponent : GameObjectComponent
    {
        private readonly List<ContextMenuEntry> _entries = new List<ContextMenuEntry>();

        public override ComponentFamily Family
        {
            get { return ComponentFamily.ContextMenu; }
        }
        
        public override ComponentReplyMessage RecieveMessage(object sender, ComponentMessageType type, params object[] list)
        {
            ComponentReplyMessage reply = base.RecieveMessage(sender, type, list);

            if (sender == this) //Don't listen to our own messages!
                return ComponentReplyMessage.Empty;

            switch (type)
            {
                case ComponentMessageType.ContextAdd:
                    AddEntry((ContextMenuEntry)list[0]);
                    break;

                case ComponentMessageType.ContextRemove:
                    RemoveEntryByName((string)list[0]);
                    break;

                case ComponentMessageType.ContextGetEntries:
                    reply = new ComponentReplyMessage(ComponentMessageType.ContextGetEntries,_entries);
                    break;
            }

            return reply;
        }

        public void RemoveEntryByName(string name)
        {
            _entries.RemoveAll(x => x.EntryName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public void RemoveEntryByMessage(string message)
        {
            _entries.RemoveAll(x => x.ComponentMessage.Equals(message, StringComparison.InvariantCultureIgnoreCase));
        }

        public void AddEntry(ContextMenuEntry entry)
        {
            _entries.Add(entry);
        }

        public override void HandleExtendedParameters(XElement extendedParameters)
        {
            foreach (var param in extendedParameters.Descendants("ContextEntry"))
            {
                var name = "NULL";
                var icon = "NULL";
                var message = "NULL";

                if (param.Attribute("name") != null)
                    name = param.Attribute("name").Value;

                if (param.Attribute("icon") != null)
                    icon = param.Attribute("icon").Value;

                if (param.Attribute("message") != null)
                    message = param.Attribute("message").Value;

                var newEntry = new ContextMenuEntry
                                   {
                                       EntryName = name,
                                       IconName = icon,
                                       ComponentMessage = message
                                   };

                _entries.Add(newEntry);
            }
        }

        public override void HandleNetworkMessage(IncomingEntityComponentMessage message)
        {
        }
    }
}
